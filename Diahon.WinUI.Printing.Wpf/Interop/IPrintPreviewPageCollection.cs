using System.Runtime.InteropServices;
using Windows.Win32.Foundation;

namespace Diahon.WinUI.Printing.Wpf.Interop;

[ComVisible(true)]
[ComImport, Guid("0b31cc62-d7ec-4747-9d6e-f2537d870f2b"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IPrintPreviewPageCollection
{
    [PreserveSig]
    HRESULT Paginate(uint currentJobPage, nint printTaskOptions);

    [PreserveSig]
    HRESULT MakePage(uint desiredJobPage, float width, float height);
}
