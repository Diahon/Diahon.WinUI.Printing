using System.Windows.Documents;
using Windows.Win32;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Printing;
using Windows.Win32.Storage.Xps.Printing;

namespace Diahon.WinUI.Printing.Wpf;

internal static class RenderCompat
{
    public static void Preview(DocumentPaginator paginator, IPrintDocumentPackageTarget docTarget)
    {
        var target = GetPreviewTarget(docTarget);
        target.SetJobPageCount(PageCountType.FinalPageCount, 1);

        // ToDo: Preview

        static IPrintPreviewDxgiPackageTarget GetPreviewTarget(IPrintDocumentPackageTarget documentPackageTarget)
        {
            Guid iid = typeof(IPrintPreviewDxgiPackageTarget).GUID;
            documentPackageTarget.GetPackageTarget(in iid, in iid, out var target).ThrowOnFailure();
            return (IPrintPreviewDxgiPackageTarget)target;
        }

        static ID2D1Factory GetD2DFactory()
        {
            Guid iid = typeof(ID2D1Factory).GUID;
            PInvoke.D2D1CreateFactory(D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_SINGLE_THREADED, in iid, null, out var factory).ThrowOnFailure();
            return (ID2D1Factory)factory;
        }
    }
}
