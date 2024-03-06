using System.Windows.Documents;
using Windows.Graphics.Printing;
using Windows.Graphics.Printing.OptionDetails;

namespace Diahon.WinUI.Printing.Wpf;
public delegate IDocumentPaginatorSource PaginatorRequiredEventHandler(PrintTaskOptions options, PrintTaskOptionDetails details);
