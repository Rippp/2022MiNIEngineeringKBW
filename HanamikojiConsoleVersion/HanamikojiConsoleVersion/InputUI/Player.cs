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

    public IPlayerMove StartMove(List<GiftCard> cardsInHand)
    {
        ConsoleWrapper.ConsoleWriteCards(cardsInHand, $"{_name} cards");

        var possibleMoves = new List<string> {nameof(CompromiseMove), nameof(GiveOneCardMove), nameof(SecretMove)};
        var selectedMoveType = ConsoleWrapper.PromptSingleSelection(possibleMoves, customTitle: "Select move:");

        IPlayerMove? move = null;
        switch (selectedMoveType)
        {
            case nameof(CompromiseMove): return MakeCompromiseMove(cardsInHand);
            case nameof(GiveOneCardMove):return MakeGiveOneCardMove(cardsInHand);
            case nameof(SecretMove): throw new NotImplementedException();
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private GiveOneCardMove MakeGiveOneCardMove(IReadOnlyList<GiftCard> cardsInHand)
    {
        var selectedCard = ConsoleWrapper.PromptSingleSelection(
            cardsInHand, 
            "GiveOneCardMove - select card: ",
            optionStyleFunction: ConsoleWrapper.GiftCardStyleFunc);
        return new GiveOneCardMove(selectedCard);
    }

    private CompromiseMove MakeCompromiseMove(IReadOnlyList<GiftCard> cardsInHand)
    {
        var selectedCards = ConsoleWrapper.PromptMultipleCardsSelection(
            cardsInHand, 
            3, 
            "CompromiseMove - Select 3 cards:");
        return new CompromiseMove(selectedCards);
    }
    public GiftCard ChooseCompromiseCard(IReadOnlyList<GiftCard> possibleCards)
        => ConsoleWrapper.PromptSingleSelection(
            possibleCards, 
            customTitle: "Choose compromise card: ", 
            optionStyleFunction: ConsoleWrapper.GiftCardStyleFunc);

    public override string ToString() => $"{_name}";

}