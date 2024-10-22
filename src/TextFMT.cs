using System.Text;

namespace Achaia;

internal static class TextFMT {
    public static string Colours(Colour bg, Colour fg) => BGColour(bg) + FGColour(fg);
    public static string FGColour(Colour c) => Foreground(c.R, c.G, c.B);
    public static string BGColour(Colour c) => Background(c.R, c.G, c.B);
    public static string Foreground(byte r, byte g, byte b) => $"\x1b[38;2;{r};{g};{b}m";
    public static string Background(byte r, byte g, byte b) => $"\x1b[48;2;{r};{g};{b}m";
    public static string Reset() => "\x1b[0m";
    public static string Bold() => "\x1b[1m";
    public static string FormatMethod(string method) {
        return $"{MethodColour(method)} {PadRight(method, 7)}{Reset()}";
    }
    public static string FormatStatus(int status) {
        return $"{ResponseStatusColour(status)} {status} {Reset()}";
    }
    public static string MethodColour(string method) => method switch {
        Server.METHOD_POST => Colours(Colour.BLUE, Colour.BLACK),
        Server.METHOD_GET => Colours(Colour.GREEN, Colour.BLACK),
        Server.METHOD_PUT => Colours(Colour.PURPLE, Colour.BLACK),
        Server.METHOD_PATCH => Colours(Colour.YELLOW, Colour.BLACK),
        Server.METHOD_DELETE => Colours(Colour.RED, Colour.BLACK),
        _ => ""
    };

    public static string ResponseStatusColour(int status) => status switch {
        >= 100 and < 200 => Colours(Colour.BLUE, Colour.BLACK),
        >= 200 and < 300 => Colours(Colour.GREEN, Colour.BLACK),
        >= 300 and < 400 => Colours(Colour.YELLOW, Colour.BLACK),
        >= 400 and < 500 => Colours(Colour.RED, Colour.BLACK),
        >= 500 and < 600 => Colours(Colour.DARKRED, Colour.BLACK),
        _ => Reset(),
    };

    public static string PadRight(string original, int targetLength) {
        StringBuilder builder = new(original);
        for (int i = 0; i < targetLength - original.Length; i++) {
            builder.Append(' ');
        }
        return builder.ToString();
    }
}