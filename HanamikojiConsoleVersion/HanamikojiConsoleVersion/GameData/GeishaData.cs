using HanamikojiConsoleVersion.Types;

namespace HanamikojiConsoleVersion.GameData;

public static class GeishaData
{
    public static Dictionary<GeishaType, int> GeishaPoints = new Dictionary<GeishaType, int> {
        { GeishaType.Geisha1, 2 },
        { GeishaType.Geisha2, 3 },
        { GeishaType.Geisha3, 4 },
        { GeishaType.Geisha4, 5 },
    };
}
