namespace HanamikojiConsoleVersion.Entities.Constants;

public static class GeishaConstants
{
    public static Dictionary<GeishaType, int> GeishaPoints = new Dictionary<GeishaType, int> {
        { GeishaType.Geisha1, 2 },
        { GeishaType.Geisha2, 3 },
        { GeishaType.Geisha3, 4 },
        { GeishaType.Geisha4, 5 },
    };

    public static Dictionary<GeishaType, ConsoleColor> GeishaConsoleColors = new Dictionary<GeishaType, ConsoleColor>
    {
        { GeishaType.Geisha1, ConsoleColor.Green },
        { GeishaType.Geisha2, ConsoleColor.Blue },
        { GeishaType.Geisha3, ConsoleColor.Red },
        { GeishaType.Geisha4, ConsoleColor.Yellow },
    };
}
