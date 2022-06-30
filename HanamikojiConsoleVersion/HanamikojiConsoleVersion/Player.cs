using HanamikojiConsoleVersion.GameData;
using HanamikojiConsoleVersion.Moves;

namespace HanamikojiConsoleVersion;

public class Player
{
    private string _name { get; set; }
    private bool _currentlyPlaying;

    public Player(string name)
    {
        _currentlyPlaying = false;
        _name = name;
    }

    public IPlayerMove StartMove(List<GiftCard> cardsInHand)
    {
        _currentlyPlaying = true;

        Console.WriteLine($"CurrentPlayer: {_name}");
        Console.WriteLine("Cards:");
        foreach (var card in cardsInHand) card.Draw();

        PrintPossibleMoves();

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

    public GiftCard ChooseCompromiseCard(IReadOnlyList<GiftCard> possibleCards)
    {
        Console.WriteLine("Select compromise card");
        foreach (var card in possibleCards) card.Draw();
        var selectedIndex = ConsoleWrapper.ConsoleReadLineUntilProperIndexIsNotSelected(possibleCards.Count, () => Console.WriteLine("wrong input!"));
        return possibleCards[selectedIndex];
    }


    private void PrintPossibleMoves()
    {
        Console.WriteLine("1. [MakeGiveOneCardMove]");
        Console.WriteLine("2. [EliminateMove]");
        Console.WriteLine("3. [SecretMove]");
        Console.WriteLine("4. [CompromiseMove]");
    }

    private GiveOneCardMove MakeGiveOneCardMove(IReadOnlyList<GiftCard> cardsInHand)
    {
        GiftCard? selectedCard = null;

        int index = 0;
        Console.WriteLine("[GiveOneCardMove] Select card: ");
        foreach (var card in cardsInHand)
        {
            Console.WriteLine($"{index} : {card}");
            index++;
        }

        var selectedCardIndex = ConsoleWrapper.ConsoleReadLineUntilProperIndexIsNotSelected(
                cardsInHand.Count() - 1,
                () => Console.WriteLine("Wrong card selected!"));

        selectedCard = cardsInHand[selectedCardIndex];
        
        return new GiveOneCardMove(selectedCard);
    }

    private CompromiseMove MakeCompromiseMove(IReadOnlyList<GiftCard> cardsInHand)
    {
        Console.WriteLine("[CompromiseMove] Select 3 cards: ");
        var index = 0;
        foreach (var card in cardsInHand)
        {
            Console.WriteLine($"{index} : {card}");
            index++;
        }

        var selectedCardIndices = ConsoleWrapper.ConsoleReadLineUnitProperCountOfIndiecesAreSelected(
            cardsInHand.Count() -  1, 3,
            () => Console.WriteLine("Wrong card selected!"));

        var selectedCards = cardsInHand.Where((x, ind) => selectedCardIndices.Contains(ind)).ToList();

        return new CompromiseMove(selectedCards);
    }

}