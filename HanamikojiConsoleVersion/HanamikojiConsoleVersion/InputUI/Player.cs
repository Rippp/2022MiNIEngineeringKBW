using HanamikojiConsoleVersion.Entities;
using HanamikojiConsoleVersion.Entities.Moves;
using HanamikojiConsoleVersion.Output;

namespace HanamikojiConsoleVersion.InputUI;

public class Player
{
    string _name { get; set; }

    public Player(string name)
    {
        _name = name;
    }

    public IPlayerMove StartMove(List<GiftCard> cardsInHand, List<string> possibleMoves)
    {
        ConsoleWrapper.ConsoleWriteCards(cardsInHand, $"{_name} cards");

        var selectedMoveType = ConsoleWrapper.PromptSingleSelection(possibleMoves, customTitle: "Select move:");

        switch (selectedMoveType)
        {
            case nameof(CompromiseMove): return MakeCompromiseMove(cardsInHand);
            case nameof(SecretMove): return MakeSecretMove(cardsInHand);
            case nameof(EliminationMove): return MakeEliminationMove(cardsInHand);
            case nameof(DoubleGiftMove): return MakeDoubleGiftMove(cardsInHand);
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private CompromiseMove MakeCompromiseMove(IReadOnlyList<GiftCard> cardsInHand)
    {
        var selectedCards = ConsoleWrapper.PromptMultipleCardsSelection(
            cardsInHand, 
            3, 
            "CompromiseMove - Select 3 cards:");
        return new CompromiseMove(selectedCards);
    }

    private SecretMove MakeSecretMove(IReadOnlyList<GiftCard> cardsInHand)
    {
        var selectedCard = ConsoleWrapper.PromptSingleSelection(
            cardsInHand,
            "Select secret card:",
            optionStyleFunction: ConsoleWrapper.GiftCardStyleFunc);
        return new SecretMove(selectedCard);
    }

    private DoubleGiftMove MakeDoubleGiftMove(IReadOnlyList<GiftCard> cardsInHand)
    {
        var selectedCards = ConsoleWrapper.PromptMultipleCardsSelection(cardsInHand, 4,"DoubleGiftMove - Select 4 cards:");

        var pair1 = ConsoleWrapper.PromptMultipleCardsSelection(selectedCards, 2, "Select first pair: ");
        var pair2 = selectedCards.Where(x => !pair1.Contains(x));

        return new DoubleGiftMove(pair1.ToArray(), pair2.ToArray());
    }

    private EliminationMove MakeEliminationMove(IReadOnlyList<GiftCard> cardsInHand)
    {
        var selectedCards = ConsoleWrapper.PromptMultipleCardsSelection(
            cardsInHand,
            2,
            "EliminationMove - Select 2 cards:");
        return new EliminationMove(selectedCards);
    }

    public GiftCard ChooseCompromiseCard(IReadOnlyList<GiftCard> cardsInHand)
        => ConsoleWrapper.PromptSingleSelection(
            cardsInHand,
            customTitle: "Choose compromise card: ",
            optionStyleFunction: ConsoleWrapper.GiftCardStyleFunc);

    public IReadOnlyCollection<GiftCard> ChooseDoubleGift(IReadOnlyList<GiftCard> pair1, IReadOnlyList<GiftCard> pair2)
    {
        var possibleOptions = new List<IReadOnlyCollection<GiftCard>> {pair1, pair2};

        var selectedPair = 
            ConsoleWrapper.PromptSingleSelection(
                possibleOptions, 
                "Choose your pair", 
                optionStyleFunction: cardList => string.Concat(cardList.Select(x => ConsoleWrapper.GiftCardStyleFunc(x))));

        return selectedPair;
    }

    public override string ToString() => $"{_name}";
}