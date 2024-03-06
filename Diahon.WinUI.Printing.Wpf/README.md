# Diahon.WinUI.Printing.Wpf
Allows printing of a Wpf `IDocumentPaginatorSource` using the modern Windows Print-Dialog / [`PrintManager`](https://learn.microsoft.com/en-us/uwp/api/windows.graphics.printing.printmanager).

> [!NOTE]
> Currently, preview is not implemented

## Usage
Create a `WpfPrintDocumentSource`.
```cs
WpfPrintDocumentSource source = new(
   (options, details) => GetDocumentPaginatorSource()
);
```
Follow the [Uwp Printing Guide](https://learn.microsoft.com/en-us/windows/uwp/devices-sensors/print-from-your-app) but use your `WpfPrintDocumentSource` instead of the WinUI `PrintDocument`.
