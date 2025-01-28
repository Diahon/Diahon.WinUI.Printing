using Diahon.WinUI.Printing.Wpf.Interop;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Windows.Documents;
using System.Windows.Threading;
using Windows.Graphics.Printing;
using Windows.Graphics.Printing.OptionDetails;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Printing;

namespace Diahon.WinUI.Printing.Wpf;

[ComVisible(true)]
internal sealed class PreviewCollection : IPrintPreviewPageCollection
{
    const uint JOB_PAGE_APPLICATION_DEFINED = uint.MaxValue;

    readonly RenderCompat _renderer = new();

    public required Dispatcher Dispatcher { get; init; }
    public required PaginatorRequiredEventHandler OnPaginatorRequired { get; init; }
    public required IPrintPreviewDxgiPackageTarget Target { get; init; }

    DocumentPaginator? _previewPaginator;
    PrintTaskOptions? _printOptions;
    HRESULT IPrintPreviewPageCollection.Paginate(uint currentJobPage, nint pPrintTaskOptions)
    {
        if (currentJobPage == JOB_PAGE_APPLICATION_DEFINED)
            currentJobPage = 1;

        _printOptions = PrintTaskOptions.FromAbi(pPrintTaskOptions);
        var details = PrintTaskOptionDetails.GetFromPrintTaskOptions(_printOptions);
        var pageCount = Dispatcher.Invoke(() =>
        {
            _previewPaginator = OnPaginatorRequired(_printOptions, details).DocumentPaginator;
            _previewPaginator.ComputePageCount();
            return _previewPaginator.PageCount;
        });
        Target.SetJobPageCount(PageCountType.FinalPageCount, (uint)pageCount).ThrowOnFailure();
        return default;
    }

    HRESULT IPrintPreviewPageCollection.MakePage(uint desiredJobPage, float previewWidth, float previewHeight)
    {
        ArgumentNullException.ThrowIfNull(_previewPaginator);
        ArgumentNullException.ThrowIfNull(_printOptions);

        if (desiredJobPage == JOB_PAGE_APPLICATION_DEFINED)
            desiredJobPage = 1;

        var pageDescription = _printOptions.GetPageDescription(desiredJobPage);

        uint width = (uint)pageDescription.PageSize.Width;
        uint height = (uint)pageDescription.PageSize.Height;

        float dpiX, dpiY;
        dpiX = dpiY = 96;

        var bufferSize = (int)(width * height * 32);
        byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
        try
        {
            Dispatcher.Invoke(() =>
            {
                using var page = _previewPaginator.GetPage((int)desiredJobPage - 1);
                RenderCompat.Render(
                   width,
                   height,
                   dpiX,
                   dpiY,
                   page.Visual,
                   buffer
                );
            });
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        var surface = _renderer.CreateSurface(
            width,
            height,
            dpiX,
            dpiY,
            buffer
        );
        Target.DrawPage(desiredJobPage, surface, (float)dpiX, (float)dpiY).ThrowOnFailure();
        return default;
    }

    public void Invalidate()
        => Target.InvalidatePreview();
}
