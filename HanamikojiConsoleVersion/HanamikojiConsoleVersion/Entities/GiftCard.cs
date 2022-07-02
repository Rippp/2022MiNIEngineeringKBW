namespace HanamikojiConsoleVersion.Entities;

public class GiftCard 
{
    string CardName { get; set; }

    public GeishaType Type { get; set; }

    public GiftCard(string cardName, GeishaType type)
    {
        CardName = cardName;
        Type = type;
    }

    public override string ToString() => $"{CardName} - {Type}";
}
