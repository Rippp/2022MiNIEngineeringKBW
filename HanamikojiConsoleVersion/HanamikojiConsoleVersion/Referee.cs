using HanamikojiConsoleVersion.GameData;
using HanamikojiConsoleVersion.Moves;

namespace HanamikojiConsoleVersion;

public class Referee
{
    public Player CurrentPlayer { get; set; }
    public Player PlayerOne { get; set; }
    public Player PlayerTwo { get; set; }
    public List<GiftCard> LeftCards { get; set; }
    public List<GiftCard> GiftsFromPlayerOne { get; set; }
    public List<GiftCard> GiftsFromPlayerTwo { get; set; }
    public GiftCard? PlayerOneSecretCard { get; set; }
    public GiftCard? PlayerTwoSecretCard { get; set; }

    private Random _random;

    public Referee(Player playerOne, Player playerTwo)
    {
        PlayerOne = playerOne;
        CurrentPlayer = playerOne;
        PlayerTwo = playerTwo;
        LeftCards = new List<GiftCard>(GiftCardData.AllCards);
        GiftsFromPlayerOne = new List<GiftCard>();
        GiftsFromPlayerTwo = new List<GiftCard>();
        _random = new Random();
    }

    public bool NextRound()
    {
        GiveRandomCardsToPlayer(CurrentPlayer, 1);
        var move = CurrentPlayer.StartMove();
        move.Execute(CurrentPlayer, this);
        CurrentPlayer = PlayerOne == CurrentPlayer ? PlayerTwo : PlayerOne;
        return CheckIfEndOfTheGame();
    }

    public bool CheckIfEndOfTheGame() => LeftCards.Count() == 0;

    public void BeginRound()
    {
        GiveRandomCardsToPlayer(PlayerOne, 3);
        GiveRandomCardsToPlayer(PlayerTwo, 3);
    }

    public void GiveRandomCardsToPlayer(Player player, int numberOfCards)
    {
        var cardsForPlayer = new List<GiftCard>();
        for (int i = 0; i < numberOfCards; i++)
        {
            var randomCardIndex = _random.Next(LeftCards.Count());
            cardsForPlayer.Add(LeftCards[randomCardIndex]);
            LeftCards.RemoveAt(randomCardIndex);
        }

        player.AddCardsToHand(cardsForPlayer);
    }

    public void Execute(Player player, GiveOneCardMove move)
    {
        var isPlayerOne = player == PlayerOne;
        var collectionToAdd = isPlayerOne ? GiftsFromPlayerOne : GiftsFromPlayerTwo;
        collectionToAdd.Add(move.CardToGive);
    }

    public void Execute(Player player, SecretMove move)
    {
        if (player == PlayerOne)
        {
            PlayerOneSecretCard = move.SecretCard;
        }
        else
        {
            PlayerTwoSecretCard = move.SecretCard;
        }
    }
}

