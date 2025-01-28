using Windows.Win32;
using Windows.Win32.Graphics.Printing;
using Windows.Win32.Storage.Xps.Printing;

namespace Diahon.WinUI.Printing.Wpf;

internal static class Extensions
{
    public static IPrintPreviewDxgiPackageTarget GetPreviewTarget(this IPrintDocumentPackageTarget documentPackageTarget)
    {
        Guid iid = typeof(IPrintPreviewDxgiPackageTarget).GUID;

        documentPackageTarget.GetPackageTarget(
            iid,
            iid,
            out var target
        ).ThrowOnFailure();

        return (IPrintPreviewDxgiPackageTarget)target;
    }
}
