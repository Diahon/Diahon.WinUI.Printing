using System.Runtime.InteropServices;
using Windows.Win32.Foundation;

namespace Diahon.WinUI.Printing.Wpf.Interop;

/// <summary>
/// Implemented by the app to provide access to print preview pages.
/// </summary>
[ComVisible(true)]
[ComImport, Guid("0b31cc62-d7ec-4747-9d6e-f2537d870f2b"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IPrintPreviewPageCollection
{
    /// <summary>
    /// This method is called whenever the system detects that the physical dimensions of the page has changed or when the app has requested pagination via a call to IPrintPreviewDxgiPreviewTarget::InvalidatePreview.
    /// </summary>
    /// <param name="currentJobPage">The current page being displayed to the user.</param>
    /// <param name="printTaskOptions">The print options.</param>
    /// <returns></returns>
    [PreserveSig]
    HRESULT Paginate(uint currentJobPage, nint printTaskOptions);

    /// <summary>
    /// Provides the system with a new page to present in print preview.
    /// </summary>
    /// <param name="desiredJobPage">The page number.</param>
    /// <param name="width">The width of the page.</param>
    /// <param name="height">The height of the page.</param>
    /// <returns></returns>
    [PreserveSig]
    HRESULT MakePage(uint desiredJobPage, float width, float height);
}
