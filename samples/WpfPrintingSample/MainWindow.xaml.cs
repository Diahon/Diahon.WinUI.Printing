using Diahon.WinUI.Printing.Wpf;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interop;
using Windows.Graphics.Printing;
using Windows.Graphics.Printing.OptionDetails;

namespace WpfPrintingSample;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    readonly WindowInteropHelper _interopHelper;
    public MainWindow()
    {
        _interopHelper = new(this);
        InitializeComponent();

        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        var printManager = PrintManagerInterop.GetForWindow(_interopHelper.Handle);
        printManager.PrintTaskRequested += PrintManager_PrintTaskRequested;
    }

    const string DocIndexOption = "sample.doc-index";

    readonly WpfPrintDocumentSource _printSource = new()
    {
        OnPaginatorRequired = (options, details) =>
        {
            return details.Options[DocIndexOption].Value switch
            {
                "0" => new FlowDocument()
                {
                    Blocks =
                    {
                        new Paragraph(new Run("Hello, World!")),
                        new Paragraph(new Run("Page 2")){
                            BreakPageBefore=true
                        }
                    }
                },
                "1" => new FlowDocument()
                {
                    Blocks =
                    {
                        new Paragraph(new Run("""
                        Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.
                        """))
                    }
                },
                _ => throw new UnreachableException(),
            };
        }
    };

    private void PrintManager_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
    {
        var printTask = args.Request.CreatePrintTask("Print Sample", request =>
        {
            request.SetSource(_printSource);
        });

        var options = printTask.Options;
        var details = PrintTaskOptionDetails.GetFromPrintTaskOptions(options);
        details.OptionChanged += (details, args) =>
        {
            _printSource.InvalidatePreview();
        };

        var docIndexList = details.CreateItemListOption(DocIndexOption, "Select Document");
        docIndexList.AddItem("0", "Hello World");
        docIndexList.AddItem("1", "Lorem Ipsum");

        options.DisplayedOptions.Add(DocIndexOption);
    }

    private async void Print_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await PrintManagerInterop.ShowPrintUIForWindowAsync(_interopHelper.Handle);
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message);
        }
    }
}