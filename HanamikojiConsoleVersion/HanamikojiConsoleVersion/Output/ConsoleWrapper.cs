using HanamikojiConsoleVersion.Entities;
using HanamikojiConsoleVersion.Entities.Constants;
using HanamikojiConsoleVersion.GameControl;
using Spectre.Console;

namespace HanamikojiConsoleVersion.Output;

public static class ConsoleWrapper
{
    public static Func<GiftCard, string> GiftCardStyleFunc =
        (card) => $"[{GeishaConstants.GeishaConsoleColors[card.Type]}]{card}[/]";

    public static Func<GeishaType, string> GeishaStyleFunc =
        (geishaType) => $"[{GeishaConstants.GeishaConsoleColors[geishaType]}]{geishaType}[/]";

    public static List<GiftCard> PromptMultipleCardsSelection(IReadOnlyCollection<GiftCard> possibleCards, int cardsToChoose, string? customTitle = null, string? customInstructions = null)
    {
        var title = customTitle ?? $"Select {cardsToChoose} cards: ";
        var instructions = customInstructions ?? "[grey](Press [blue]<space>[/] to toggle a card, [green]<enter>[/] to accept)[/]";
        var multiSelectionCardPrompt = new MultiSelectionPrompt<GiftCard>()
            .Title(title)
            .Required()
            .AddChoices(possibleCards)
            .HighlightStyle(Style.Plain)
            .UseConverter(x => $"[{GeishaConstants.GeishaConsoleColors[x.Type]}]{x}[/]")
            .InstructionsText(instructions);

        var selectedCards = AnsiConsole.Prompt(multiSelectionCardPrompt);
        while (true)
        {
            if (selectedCards.Count != cardsToChoose)
            {
                selectedCards = AnsiConsole.Prompt(multiSelectionCardPrompt.Title(title + $"Selected {selectedCards.Count} cards, you should select {cardsToChoose} cards!"));
            }
            else
            {
                return selectedCards;
            }
        }
    }

    public static T PromptSingleSelection<T>(IReadOnlyCollection<T> possibleValues, string? customTitle = null, string? customInstructions = null, Func<T, string>? optionStyleFunction = null) 
        where T : notnull
    {
        var title = customTitle ?? $"Select value: ";
        var instructions = customInstructions ?? "[grey](Press [blue]<space>[/] to toggle a fruit, [green]<enter>[/] to accept)[/]";
        var selectionPrompt = new SelectionPrompt<T>()
            .Title(title)
            .AddChoices(possibleValues)
            .HighlightStyle(Style.Plain);

        if (optionStyleFunction != null)
        {
            selectionPrompt.UseConverter(optionStyleFunction);
        }

        return AnsiConsole.Prompt(selectionPrompt);
    }

    public static Table GetCardsTable(IReadOnlyList<GiftCard> cards, string title)
    {
        var cardsTable = new Table();
        cardsTable.AddColumn(new TableColumn(title).Centered());
        foreach (var card in cards)
        {
            cardsTable.AddRow(GiftCardStyleFunc(card));
        }

        return cardsTable;
    }

    public static void ConsoleWriteCards(IReadOnlyList<GiftCard> cards, string title) =>
        AnsiConsole.Write(GetCardsTable(cards, title));

    public static void PrintGeishaStates(PlayerData playerOneData, PlayerData playerTwoData, string playerOneName, string playerTwoName)
    {
        AnsiConsole.Clear();        
        var table = new Table();

        foreach (var geishaType in Enum.GetValues<GeishaType>())
        {
            table.AddColumn(new TableColumn(GeishaStyleFunc(geishaType)).Centered());
        }

        var panelsForFirstRow = new List<Panel>();
        var panelsForSecondRow = new List<Panel>();
        foreach (var geishaType in Enum.GetValues<GeishaType>())
        {
            var playerOnePanel = new Panel($"{playerOneName}: {CountPointsForGeisha(playerOneData, geishaType)}");
            var playerTwoPanel = new Panel($"{playerTwoName}: {CountPointsForGeisha(playerTwoData, geishaType)}");

            panelsForFirstRow.Add(playerOnePanel);
            panelsForSecondRow.Add(playerTwoPanel);
        }

        table.AddRow(panelsForFirstRow);
        table.AddRow(panelsForSecondRow);

        AnsiConsole.Write(table);
    }

    private static int CountPointsForGeisha(PlayerData playerData, GeishaType geishaType) =>
        playerData.GiftsFromPlayer.Count(card => card.Type == geishaType);
}
