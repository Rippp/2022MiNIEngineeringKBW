using HanamikojiConsoleVersion.Entities.Constants;

namespace HanamikojiConsoleVersion.Entities;

public class GiftCard 
{
    public GeishaType Type { get; set; }

    public GiftCard(GeishaType type)
    {
        Type = type;
    }

    public override string ToString() => $"{Type} - {GeishaConstants.GeishaConsoleColors[Type]}";
}
