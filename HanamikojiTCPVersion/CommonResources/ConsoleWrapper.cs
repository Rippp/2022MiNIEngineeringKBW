using CommonResources.Game;
using CommonResources.Game.Constants;
using Spectre.Console;

namespace CommonResources
{
    public static class ConsoleWrapper
    {
        public static Func<GiftCard, string> GiftCardStyleFunc =
            (card) => $"[{GeishaConstants.GeishaConsoleColors[card.Type]}]{card}[/]";

        public static void WriteError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Error] {errorMessage}");
            Console.ResetColor();
        }

        public static void WriteInfo(string infoMessage)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"[Info] {infoMessage}");
            Console.ResetColor();
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
        public static void ConsoleWriteCards(IReadOnlyList<GiftCard> cards, string title)
            => AnsiConsole.Write(GetCardsTable(cards, title));
    }
}
