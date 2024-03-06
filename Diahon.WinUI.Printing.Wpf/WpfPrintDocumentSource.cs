using Diahon.WinUI.Printing.Wpf.Interop;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Documents;
using System.Windows.Threading;
using Windows.Graphics.Printing;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.Xps.Printing;

namespace Diahon.WinUI.Printing.Wpf;

[ComVisible(true)]
public sealed class WpfPrintDocumentSource(Dispatcher? dispatcher = null) : IPrintDocumentPageSource, IPrintPreviewPageCollection, IPrintDocumentSource, ICustomQueryInterface
{
    readonly Dispatcher _dispatcher = dispatcher ?? Dispatcher.CurrentDispatcher;
    public required Func<PrintTaskOptions, IDocumentPaginatorSource> PaginatorSource { get; set; }

    IPrintDocumentPackageTarget? previewPackageTarget;
    HRESULT IPrintDocumentPageSource.GetPreviewPageCollection(IPrintDocumentPackageTarget docPackageTarget, out IPrintPreviewPageCollection docPageCollection)
    {
        previewPackageTarget = docPackageTarget;
        docPageCollection = this;
        return default;
    }

    /// <summary>
    /// Called by PrintManager when the Print button on Modern Print Dialog is clicked.
    /// https://github.com/microsoft/microsoft-ui-xaml/blob/c8bd154c015b914e171a326481fef6532bc943de/dxaml/xcp/dxaml/lib/PrintDocument_Partial.cpp#L158
    /// </summary>
    HRESULT IPrintDocumentPageSource.MakeDocument(nint pPrintTaskOptions, IPrintDocumentPackageTarget docPackageTarget)
    {
        // PrintDocument.GetPages
        var options = PrintTaskOptions.FromAbi(pPrintTaskOptions);
        _dispatcher.Invoke(() => PrintCompat.Print(PaginatorSource(options).DocumentPaginator, docPackageTarget, options));
        return default;
    }

    /// <summary>
    /// Callback to indicate that a preview setting has changed that requires the application to repaginate its printing content.
    /// </summary>
    HRESULT IPrintPreviewPageCollection.MakePage(uint desiredJobPage, float width, float height)
    {
        // PrintDocument.GetPreviewPage
        return default;
    }

    /// <summary>
    /// Callback to indicate that a preview setting has changed that requires the application to repaginate its printing content.
    /// </summary>
    HRESULT IPrintPreviewPageCollection.Paginate(uint currentJobPage, nint pPrintTaskOptions)
    {
        var printTaskOptions = PrintTaskOptions.FromAbi(pPrintTaskOptions);
        _dispatcher.Invoke(() => RenderCompat.Preview(
            PaginatorSource(printTaskOptions).DocumentPaginator,
            previewPackageTarget ?? throw new UnreachableException($"No {nameof(previewPackageTarget)}")
        ));
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
        else if (iid == typeof(IPrintPreviewPageCollection).GUID)
        {
            ppv = GetInterface<IPrintPreviewPageCollection>(this);
            return CustomQueryInterfaceResult.Handled;
        }

        ppv = 0;
        return CustomQueryInterfaceResult.NotHandled;

        static nint GetInterface<TInterface>(WpfPrintDocumentSource @this)
            => Marshal.GetComInterfaceForObject(@this, typeof(TInterface), CustomQueryInterfaceMode.Ignore);
    }
}
