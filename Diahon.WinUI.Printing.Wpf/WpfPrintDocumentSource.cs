using Diahon.WinUI.Printing.Wpf.Interop;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Windows.Graphics.Printing;
using Windows.Graphics.Printing.OptionDetails;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.Xps.Printing;

namespace Diahon.WinUI.Printing.Wpf;

[ComVisible(true)]
public sealed class WpfPrintDocumentSource : IPrintDocumentPageSource, IPrintDocumentSource, ICustomQueryInterface
{
    public Dispatcher Dispatcher { get; init; } = Dispatcher.CurrentDispatcher;
    public required PaginatorRequiredEventHandler OnPaginatorRequired { get; init; }

    readonly List<PreviewCollection> _previewCollections = [];
    HRESULT IPrintDocumentPageSource.GetPreviewPageCollection(IPrintDocumentPackageTarget docPackageTarget, out IPrintPreviewPageCollection docPageCollection)
    {
        var target = docPackageTarget.GetPreviewTarget();
        PreviewCollection preview = new()
        {
            Dispatcher = Dispatcher,
            OnPaginatorRequired = OnPaginatorRequired,
            Target = target
        };
        _previewCollections.Add(preview);

        docPageCollection = preview;
        return default;
    }

    public void InvalidatePreview()
    {
        foreach (var preview in _previewCollections)
            preview.Invalidate();
    }

    /// <summary>
    /// Called by PrintManager when the Print button on Modern Print Dialog is clicked.
    /// https://github.com/microsoft/microsoft-ui-xaml/blob/c8bd154c015b914e171a326481fef6532bc943de/dxaml/xcp/dxaml/lib/PrintDocument_Partial.cpp#L158
    /// </summary>
    HRESULT IPrintDocumentPageSource.MakeDocument(nint pPrintTaskOptions, IPrintDocumentPackageTarget docPackageTarget)
    {
        var options = PrintTaskOptions.FromAbi(pPrintTaskOptions);
        var details = PrintTaskOptionDetails.GetFromPrintTaskOptions(options);
        Dispatcher.Invoke(() =>
        {
            var paginator = OnPaginatorRequired(options, details).DocumentPaginator;
            PrintCompat.Print(paginator, docPackageTarget, options);
        });
        return default;
    }

    // Workaround to use [ComImport] IUnknown with CsWinRT IInspectable 
    CustomQueryInterfaceResult ICustomQueryInterface.GetInterface(ref Guid iid, out nint ppv)
    {
        if (iid == typeof(IPrintDocumentPageSource).GUID)
        {
            ppv = GetInterface<IPrintDocumentPageSource>(this);
            return CustomQueryInterfaceResult.Handled;
        }

        ppv = 0;
        return CustomQueryInterfaceResult.NotHandled;

        static nint GetInterface<TInterface>(WpfPrintDocumentSource @this)
            => Marshal.GetComInterfaceForObject(@this, typeof(TInterface), CustomQueryInterfaceMode.Ignore);
    }
}
