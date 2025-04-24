using Godot;

public enum ColourPalette
{
    ENEMY,
    ALLY,
    PLAYER,
    NEUTRAL
}

public static class ColourPaletteExtensions
{
    public static string ToHex(this ColourPalette color)
    {
        return color switch
        {
            ColourPalette.ENEMY => "bc0011",
            ColourPalette.ALLY => "00a1f9",
            ColourPalette.PLAYER => "00ba12",
            ColourPalette.NEUTRAL => "A9A9A9",
            _ => "ffffff"
        };
    }

    public static Color ToColor(this ColourPalette color)
    {
        return new Color(color.ToHex());
    }
}