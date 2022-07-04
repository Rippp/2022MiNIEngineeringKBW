namespace HanamikojiConsoleVersion.Entities.Constants;

public static class GeishaConstants
{
    public static Dictionary<GeishaType, int> GeishaPoints = new Dictionary<GeishaType, int> {
        { GeishaType.Geisha2_A, 2 },
        { GeishaType.Geisha2_B, 2 },
        { GeishaType.Geisha2_C, 4 },
        { GeishaType.Geisha3_A, 4 },
        { GeishaType.Geisha3_B, 4 },
        { GeishaType.Geisha4_A, 4 },
        { GeishaType.Geisha5_A, 5 },
    };

    public static Dictionary<GeishaType, ConsoleColor> GeishaConsoleColors = new Dictionary<GeishaType, ConsoleColor>
    {
        { GeishaType.Geisha2_A, ConsoleColor.Yellow },
        { GeishaType.Geisha2_B, ConsoleColor.Red },
        { GeishaType.Geisha2_C, ConsoleColor.Gray },
        { GeishaType.Geisha3_A, ConsoleColor.Blue },
        { GeishaType.Geisha3_B, ConsoleColor.Cyan },
        { GeishaType.Geisha4_A, ConsoleColor.Green },
        { GeishaType.Geisha5_A, ConsoleColor.Magenta},
    };
}
