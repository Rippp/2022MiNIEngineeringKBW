using HanamikojiConsoleVersion.GameData;
using HanamikojiConsoleVersion.Moves;

namespace HanamikojiConsoleVersion;

public class Player
{
    string _name { get; set; }

    public Player(string name)
    {
        _name = name;
    }

    public IPlayerMove StartMove(List<GiftCard> cardsInHand)
    {
        Console.WriteLine($"CurrentPlayer: {_name}");
        ConsoleWrapper.ConsoleWriteCards(cardsInHand);
        ConsoleWrapper.ConsoleWritePossibleMoves();

        IPlayerMove? move = null;
        while (move == null)
        {
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    move = MakeGiveOneCardMove(cardsInHand);

                    break;
                case "2":
                    throw new NotImplementedException();
                    break;

                case "3":
                    throw new NotImplementedException();
                    break;

                case "4":
                    move = MakeCompromiseMove(cardsInHand);
                    break;

                default:
                    Console.WriteLine("!!! Incorrect move !!!");
                    break;
            }
        }

        return move;
    }

    private GiveOneCardMove MakeGiveOneCardMove(IReadOnlyList<GiftCard> cardsInHand)
    {
        Console.WriteLine("[GiveOneCardMove] Select card: ");
        ConsoleWrapper.ConsoleWriteCards(cardsInHand);

        var selectedCardIndex = ConsoleWrapper.ConsoleReadLineUntilProperIndexIsNotSelected(
                cardsInHand.Count() - 1,
                () => Console.WriteLine("Wrong card selected!"));

        return new GiveOneCardMove(cardsInHand[selectedCardIndex]);
    }

    private CompromiseMove MakeCompromiseMove(IReadOnlyList<GiftCard> cardsInHand)
    {
        Console.WriteLine("[CompromiseMove] Select 3 cards: ");
        ConsoleWrapper.ConsoleWriteCards(cardsInHand);

        var selectedCardIndices = ConsoleWrapper.ConsoleReadLineUnitProperCountOfIndiecesAreSelected(
            cardsInHand.Count() -  1, 3,
            () => Console.WriteLine("Wrong card selected!"));

        return new CompromiseMove(cardsInHand.Where((x, ind) => selectedCardIndices.Contains(ind)).ToList());
    }
    public GiftCard ChooseCompromiseCard(IReadOnlyList<GiftCard> possibleCards)
    {
        Console.WriteLine("Select compromise card: ");
        ConsoleWrapper.ConsoleWriteCards(possibleCards);
        var selectedIndex = ConsoleWrapper.ConsoleReadLineUntilProperIndexIsNotSelected(
            possibleCards.Count, () => Console.WriteLine("wrong input!"));
        return possibleCards[selectedIndex];
    }

    public override string ToString() => $"{_name}";

}