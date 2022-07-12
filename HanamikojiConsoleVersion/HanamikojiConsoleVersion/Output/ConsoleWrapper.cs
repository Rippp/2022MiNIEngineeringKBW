using HanamikojiConsoleVersion.Entities;
using HanamikojiConsoleVersion.Entities.Constants;
using HanamikojiConsoleVersion.GameControl;
using HanamikojiConsoleVersion.InputUI;
using Spectre.Console;

namespace HanamikojiConsoleVersion.Output;

public static class ConsoleWrapper
{
    public static Func<GiftCard, string> GiftCardStyleFunc =
        (card) => $"[{GeishaConstants.GeishaConsoleColors[card.Type]}]{card}[/]";

    public static Func<GeishaType, string> GeishaStyleFunc =
        (geishaType) => $"[{GeishaConstants.GeishaConsoleColors[geishaType]}]{geishaType}[/]";

    public static List<GiftCard> PromptMultipleCardsSelection(IReadOnlyCollection<GiftCard> possibleCards, 
        int cardsToChoose, string? customTitle = null, string? customInstructions = null)
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

    public static T PromptSingleSelection<T>(IReadOnlyCollection<T> possibleValues, string? customTitle = null, 
        string? customInstructions = null, Func<T, string>? optionStyleFunction = null) 
        where T : notnull
    {
        var title = customTitle ?? $"Select value: ";
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

    public static void PrintGeishaStates(PlayerData playerOneData, PlayerData playerTwoData, 
        IDictionary<GeishaType, Player?> convincedToPlayerDict, string playerOneName, string playerTwoName)
    {
        AnsiConsole.Clear();        
        var table = new Table();

        table.AddColumn(new TableColumn("Player Name"));
        foreach (var geishaType in Enum.GetValues<GeishaType>())
        {
            table.AddColumn(new TableColumn(GeishaStyleFunc(geishaType)).Centered());
        }

        var playerOnePointsPanels = new List<Panel>();
        var playerTwoPointsPanels = new List<Panel>();
        var convincedToPlayerPanels = new List<Panel>();


        playerOnePointsPanels.Add(new Panel(playerOneName));
        playerTwoPointsPanels.Add(new Panel(playerTwoName));
        convincedToPlayerPanels.Add(new Panel("Convinced to player"));


        foreach (var geishaType in Enum.GetValues<GeishaType>())
        {
            var playerOnePanel = new Panel($"{playerOneData.CountPointsForGeishaType(geishaType)}");
            var playerTwoPanel = new Panel($"{playerTwoData.CountPointsForGeishaType(geishaType)}");
            var convincedToPlayerPanel = new Panel(convincedToPlayerDict[geishaType]?.ToString() ?? "---");

            playerOnePointsPanels.Add(playerOnePanel);
            playerTwoPointsPanels.Add(playerTwoPanel);
            convincedToPlayerPanels.Add(convincedToPlayerPanel);
        }

        table.AddRow(playerOnePointsPanels);
        table.AddRow(playerTwoPointsPanels);
        table.AddRow(convincedToPlayerPanels);

        AnsiConsole.Write(table);
    }
}
