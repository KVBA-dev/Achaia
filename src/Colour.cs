namespace Achaia;

internal struct Colour {
    public byte R;
    public byte G;
    public byte B;

    public Colour(byte r, byte g, byte b) {
        R = r;
        G = g;
        B = b;
    }

    public static readonly Colour RED = new(252, 83, 61);
    public static readonly Colour YELLOW = new(211, 252, 61);
    public static readonly Colour GREEN = new(79, 252, 43);
    public static readonly Colour BLUE = new(3, 182, 252);
    public static readonly Colour PURPLE = new(181, 43, 252);
    public static readonly Colour DARKRED = new(201, 4, 30);
    public static readonly Colour WHITE = new(255, 255, 255);
    public static readonly Colour BLACK = new(0, 0, 0);
}