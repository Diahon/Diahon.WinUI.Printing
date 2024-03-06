using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.Xps.Printing;

namespace Diahon.WinUI.Printing.Wpf.Interop;

[ComImport, Guid("a96bb1db-172e-4667-82b5-ad97a252318f"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IPrintDocumentPageSource
{
    [PreserveSig]
    HRESULT GetPreviewPageCollection(IPrintDocumentPackageTarget docPackageTarget, out IPrintPreviewPageCollection docPageCollection);

    [PreserveSig]
    HRESULT MakeDocument(nint printTaskOptions, IPrintDocumentPackageTarget docPackageTarget);
}
