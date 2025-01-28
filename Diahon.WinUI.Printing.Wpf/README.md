# Diahon.WinUI.Printing.Wpf
Allows printing of a Wpf [`IDocumentPaginatorSource`](https://learn.microsoft.com/de-de/dotnet/api/system.windows.documents.idocumentpaginatorsource) using the modern Windows Print-Dialog / [`PrintManager`](https://learn.microsoft.com/en-us/uwp/api/windows.graphics.printing.printmanager).

## Usage
Follow the [Uwp Printing Guide](https://learn.microsoft.com/en-us/windows/uwp/devices-sensors/print-from-your-app) but use your `WpfPrintDocumentSource` instead of the WinUI `PrintDocument`.

Have a look at the [WpfPrintingSample](https://github.com/Diahon/Diahon.WinUI.Printing/tree/main/samples/WpfPrintingSample) for more infos.

### Simple `WpfPrintDocumentSource`.
```cs
WpfPrintDocumentSource printSource = new()
{
    OnPaginatorRequired = (options, details) =>
    {
        return new FlowDocument()
        {
            Blocks =
            {
                new Paragraph(new Run("Hello, World!")),
                new Paragraph(new Run("Page 2")){
                    BreakPageBefore=true
                }
            }
        };
    }
};
```

### Advanced usage with custom options
```cs
WpfPrintDocumentSource printSource = new()
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
```
