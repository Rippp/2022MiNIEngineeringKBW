using HanamikojiConsoleVersion.GameData;
using HanamikojiConsoleVersion.Moves;

namespace HanamikojiConsoleVersion;

public class Player
{
    private string _name { get; set; }
    List<GiftCard> CardsInHand { get; set; }
    private bool _currentlyPlaying;

    public Player(string name)
    {
        CardsInHand = new List<GiftCard>();
        _currentlyPlaying = false;
        _name = name;
    }

    public IPlayerMove StartMove()
    {
        _currentlyPlaying = true;

        Console.WriteLine($"CurrentPlayer: {_name}");
        Console.WriteLine("Cards:");
        foreach (var card in CardsInHand) card.Draw();

        PrintPossibleMoves();

        IPlayerMove? move = null;


        while (move == null)
        {
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    move = MakeGiveOneCardMove();
                    break;

                case "2":
                    throw new NotImplementedException();

                    break;

                case "3":
                    throw new NotImplementedException();

                    break;

                default:
                    Console.WriteLine("!!! Incorrect move !!!");
                    break;
            }
        }
        
        return new GiveOneCardMove(GiftCardData.AllCards.First());
    }

    private void PrintPossibleMoves()
    {
        Console.WriteLine("1. [MakeGiveOneCardMove]");
        Console.WriteLine("2. [EliminateMove]");
        Console.WriteLine("3. [SecretMove]");
    }

    private GiveOneCardMove MakeGiveOneCardMove()
    {
        GiftCard? selectedCard = null;

        int index = 0;

        Console.WriteLine("[GiveOneCardMove] Select card: ");
        foreach (var card in CardsInHand)
        {
            Console.WriteLine($"{index} : {card}");
            index++;
        }

        while(selectedCard == null)
        {
            var selectedCardInput = Console.ReadLine();
            var converted = int.TryParse(selectedCardInput, out var selectedCardIndex);
            if (!converted || selectedCardIndex >= CardsInHand.Count || selectedCardIndex < 0)
            {
                Console.WriteLine("!!! Incorrect input !!!");
                continue;
            }

            selectedCard = CardsInHand[selectedCardIndex];
            CardsInHand.RemoveAt(selectedCardIndex);
        }

        return new GiveOneCardMove(selectedCard);
    }

    public void AddCardsToHand(List<GiftCard> givenCards) => CardsInHand.AddRange(givenCards);
}