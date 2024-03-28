using System.Printing;
using Windows.Graphics.Printing;

namespace Diahon.WinUI.Printing.Wpf;

internal static class PrintTicketCompat
{
    public static PrintTicket CreatePrintTicket(PrintTaskOptions options)
        => new()
        {
            // Binding
            PageBorderless = options.Bordering switch
            {
                PrintBordering.Bordered => PageBorderless.None,
                PrintBordering.Borderless => PageBorderless.Borderless,
                _ => null
            },
            Collation = options.Collation switch
            {
                PrintCollation.Collated => Collation.Collated,
                PrintCollation.Uncollated => Collation.Uncollated,
                _ => null
            },
            OutputColor = options.ColorMode switch
            {
                PrintColorMode.Color => OutputColor.Color,
                PrintColorMode.Grayscale => OutputColor.Grayscale,
                PrintColorMode.Monochrome => OutputColor.Monochrome,
                _ => null
            },
            // CustomPageRanges
            Duplexing = options.Duplex switch
            {
                PrintDuplex.OneSided => Duplexing.OneSided,
                PrintDuplex.TwoSidedShortEdge => Duplexing.TwoSidedShortEdge,
                PrintDuplex.TwoSidedLongEdge => Duplexing.TwoSidedLongEdge,
                _ => null
            },
            // HolePunch
            PageMediaSize = /* ToDo */ new(PageMediaSizeName.ISOA4),
            // PageMediaType
            CopyCount = (int)options.NumberOfCopies,
            PageOrientation = options.Orientation switch
            {
                PrintOrientation.Portrait => PageOrientation.Portrait,
                PrintOrientation.PortraitFlipped => PageOrientation.ReversePortrait,
                PrintOrientation.Landscape => PageOrientation.Landscape,
                PrintOrientation.LandscapeFlipped => PageOrientation.ReverseLandscape,
                _ => null
            },
            // PageRangeOptions
            // PrintQuality
            // Staple
        };
}
