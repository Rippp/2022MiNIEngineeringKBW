using HanamikojiConsoleVersion.Types;

namespace HanamikojiConsoleVersion;

public class GiftCard
{
    string CardName { get; set; }

    GeishaType Type { get; set; }

    public GiftCard(string cardName, GeishaType type)
    {
        CardName = cardName;
        Type = type;
    }

    public void Draw()
    {
        var color = Type switch
        {
            GeishaType.Geisha1 => ConsoleColor.Green,
            GeishaType.Geisha2 => ConsoleColor.Blue,
            GeishaType.Geisha3 => ConsoleColor.Red,
            GeishaType.Geisha4 => ConsoleColor.Yellow,
            _ => ConsoleColor.White
        };

        Console.ForegroundColor = color;
        Console.WriteLine(this);
        Console.ResetColor();
    }

    public override string ToString() => $"*** Karta: {CardName} typ: {Type} ***";
}
