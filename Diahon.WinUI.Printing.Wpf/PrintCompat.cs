using System.Printing;
using System.Runtime.InteropServices;
using System.Windows.Documents;
using System.Windows.Xps.Serialization;
using Windows.Graphics.Printing;
using Windows.Win32;
using Windows.Win32.Storage.Xps;
using Windows.Win32.Storage.Xps.Printing;

namespace Diahon.WinUI.Printing.Wpf;
static class PrintCompat
{
    static readonly Guid IID_IXpsDocumentPackageTarget = typeof(IXpsDocumentPackageTarget).GUID;

    // https://github.com/dotnet/wpf/blob/089e48a18e84ba199d813b572acd6ca3781be27b/src/Microsoft.DotNet.Wpf/src/System.Printing/CPP/src/XPSDocumentWriter.cpp#L428
    internal static void Print(DocumentPaginator paginator, IPrintDocumentPackageTarget docTarget, PrintTaskOptions options, PrintTicket? ticket = null)
    {
        ticket ??= PrintTicketCompat.CreatePrintTicket(options);

        var pageInfo = options.GetPageDescription(0);
        paginator.PageSize = new(pageInfo.PageSize.Width, pageInfo.PageSize.Height);

        docTarget.GetPackageTarget(in PInvoke.ID_DOCUMENTPACKAGETARGET_MSXPS, in IID_IXpsDocumentPackageTarget, out var target).ThrowOnFailure();

        try
        {
            XpsSerializationHelper.WriteToXpsDocumentPackageTarget(target, paginator, (s, e) =>
            {
                e.PrintTicket = ticket;
            }, (s, e) =>
            {

            });
        }
        catch
        {
            docTarget.Cancel();
            throw;
        }

        Marshal.FinalReleaseComObject(target);
    }
}
