using HanamikojiConsoleVersion.Types;
using Spectre.Console;
using System.Linq;

namespace HanamikojiConsoleVersion;

public static class ConsoleWrapper
{
    public static int ConsoleReadLineUntilProperIndexIsNotSelected(int maxValidIndex, Action errorAction)
        => ConsoleReadLineUnitProperCountOfIndiecesAreSelected(maxValidIndex, 1, errorAction).Single();

    public static List<int> ConsoleReadLineUnitProperCountOfIndiecesAreSelected(int maxValidIndex, int countOfIndices, Action errorAction)
    {
        var selectedIndices = new List<int>();

        while (selectedIndices.Count != countOfIndices)
        {
            var userInput = Console.ReadLine();
            var converted = int.TryParse(userInput, out var selectedIndex);
            if (!converted || selectedIndex < 0 || selectedIndex > maxValidIndex || selectedIndices.Contains(selectedIndex))
            {
                errorAction();
            }
            else
            {
                selectedIndices.Add(selectedIndex);
            }
        }

        return selectedIndices;
    }

    public static void ConsoleWritePossibleMoves()
    {
        Console.WriteLine("1. [MakeGiveOneCardMove]");
        Console.WriteLine("2. [EliminateMove]");
        Console.WriteLine("3. [SecretMove]");
        Console.WriteLine("4. [CompromiseMove]");
    }

    public static void ConsoleWriteCards(IReadOnlyList<GiftCard> cards)
    {
        for(int i = 0; i < cards.Count; i++)
        {
            Console.Write($"{i}. ");
            cards[i].Draw();
        }
    }

    public static void ConsoleWriteGameState(PlayerData playerOneData, PlayerData playerTwoData, string playerOneName, string playerTwoName)
    {
        AnsiConsole.Clear();        
        var table = new Table();

        table.AddColumn(new TableColumn("[green]Geisha1[/]").Centered());
        table.AddColumn(new TableColumn("[blue]Geisha2[/]").Centered());
        table.AddColumn(new TableColumn("[red]Geisha3[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Geisha4[/]").Centered());

        table.AddRow(
            new Panel($"{playerOneName}: {CountPointsForGeisha(playerOneData, GeishaType.Geisha1)}"),
            new Panel($"{playerOneName}: {CountPointsForGeisha(playerOneData, GeishaType.Geisha2)}"),
            new Panel($"{playerOneName}: {CountPointsForGeisha(playerOneData, GeishaType.Geisha3)}"),
            new Panel($"{playerOneName}: {CountPointsForGeisha(playerOneData, GeishaType.Geisha4)}")
            );

        table.AddRow(
            new Panel($"{playerTwoName}: {CountPointsForGeisha(playerTwoData, GeishaType.Geisha1)}"),
            new Panel($"{playerTwoName}: {CountPointsForGeisha(playerTwoData, GeishaType.Geisha2)}"),
            new Panel($"{playerTwoName}: {CountPointsForGeisha(playerTwoData, GeishaType.Geisha3)}"),
            new Panel($"{playerTwoName}: {CountPointsForGeisha(playerTwoData, GeishaType.Geisha4)}")
        );

        AnsiConsole.Write(table);
    }

    private static int CountPointsForGeisha(PlayerData playerData, GeishaType geishaType) =>
        playerData.GiftsFromPlayer.Where(card => card.Type == geishaType).Count();
}
