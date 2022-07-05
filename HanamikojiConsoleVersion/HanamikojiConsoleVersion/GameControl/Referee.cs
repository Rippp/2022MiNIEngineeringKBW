using System.Data;
using HanamikojiConsoleVersion.Entities;
using HanamikojiConsoleVersion.Entities.Constants;
using HanamikojiConsoleVersion.Entities.Moves;
using HanamikojiConsoleVersion.InputUI;
using HanamikojiConsoleVersion.Output;
using Spectre.Console;

namespace HanamikojiConsoleVersion.GameControl;

public class Referee
{
    public Player CurrentPlayer { get; set; }
    public Player PlayerOne { get; set; }
    public Player PlayerTwo { get; set; }
    public Player OtherPlayer => CurrentPlayer == PlayerOne ? PlayerTwo : PlayerOne;

    public PlayerData CurrentPlayerData { get; set; }
    public PlayerData PlayerOneData { get; set; }
    public PlayerData PlayerTwoData { get; set; }
    public PlayerData OtherPlayerData => CurrentPlayerData == PlayerOneData ? PlayerTwoData : PlayerOneData;

    public List<GiftCard> CardDeck { get; set; }
    private Random _random;

    private Dictionary<GeishaType, Player?> convincedToPlayerInPreviousRound;

    public Referee(Player playerOne, Player playerTwo)
    {
        PlayerOne = playerOne;
        PlayerTwo = playerTwo;
        CurrentPlayer = playerOne;

        PlayerOneData = new PlayerData();
        PlayerTwoData = new PlayerData();

        convincedToPlayerInPreviousRound = new();
        foreach (var geishaType in Enum.GetValues<GeishaType>())
        {
            convincedToPlayerInPreviousRound.Add(geishaType, null);
        }

        CurrentPlayerData = PlayerOneData;

        _random = new Random();
        StartNewRound();
    }

    public bool NextRound()
    {
        PrintGameState();

        var nextCard = GetRandomCards(1).Single();
        CurrentPlayerData.CardsOnHand.Add(nextCard);

        var availableMoves = CurrentPlayerData.GetAvailableMoves();

        var move = CurrentPlayer.StartMove(CurrentPlayerData.CardsOnHand, availableMoves);

        move.Execute(this);
        CurrentPlayerData.MarkMoveAsNotAvailable(move.ToString());

        SwitchPlayer();

        if (!CurrentPlayerData.IsAnyMoveAvailable() && !OtherPlayerData.IsAnyMoveAvailable())
        {
            if (CheckIfEndOfTheGame(out var convincedToPlayerOne, out var ConvincedToPlayerTwo))
            {
                return true;
            }
            else
            {
                SaveConvincedGeisha(convincedToPlayerOne, ConvincedToPlayerTwo);
                StartNewRound();
            }
        }

        return false;
    }

    public void Execute(SecretMove move)
    {
        CurrentPlayerData.SecretCard = move.SecretCard;
        CurrentPlayerData.CardsOnHand.Remove(move.SecretCard);
    }

    public void Execute(EliminationMove move)
    {
        CurrentPlayerData.EliminatedCards = move.EliminatedCards;
        CurrentPlayerData.CardsOnHand.RemoveAll(x => move.EliminatedCards.Contains(x));
    }

    public void Execute(CompromiseMove move)
    {
        var selectedCard = OtherPlayer.ChooseCompromiseCard(move.CardToChooseFrom);
        OtherPlayerData.GiftsFromPlayer.Add(selectedCard);
        CurrentPlayerData.GiftsFromPlayer.AddRange(move.CardToChooseFrom.Where(x => x != selectedCard));
        CurrentPlayerData.CardsOnHand.RemoveAll(x => move.CardToChooseFrom.Contains(x));
    }

    public void Execute(DoubleGiftMove move)
    {
        var selectedCards = OtherPlayer.ChooseDoubleGift(move.PairOne, move.PairTwo);
        var collectionForOtherPlayer = selectedCards;
        var collectionForCurrentPlayer = selectedCards == move.PairOne ? move.PairTwo : move.PairOne;
        OtherPlayerData.GiftsFromPlayer.AddRange(collectionForOtherPlayer);
        CurrentPlayerData.GiftsFromPlayer.AddRange(collectionForCurrentPlayer);
        CurrentPlayerData.CardsOnHand.RemoveAll(x => move.PairOne.Contains(x) || move.PairTwo.Contains(x));
    }

    private void SaveConvincedGeisha(IReadOnlyList<GeishaType> convincedToPlayerOne, IReadOnlyList<GeishaType> convincedToPlayerTwo)
    {
        foreach (var geishaConvincedToPlayerOne in convincedToPlayerOne)
            convincedToPlayerInPreviousRound[geishaConvincedToPlayerOne] = PlayerOne;
    
        foreach(var geishaConvincedToPlayerTwo in convincedToPlayerTwo)
            convincedToPlayerInPreviousRound[geishaConvincedToPlayerTwo] = PlayerTwo;
    }

    private void StartNewRound()
    {
        CardDeck = new List<GiftCard>(GiftCardConstants.AllCards);
        CurrentPlayerData.ClearData();
        OtherPlayerData.ClearData();
        PlayerOneData.CardsOnHand.AddRange(GetRandomCards(6));
        PlayerTwoData.CardsOnHand.AddRange(GetRandomCards(6));
    }

    private bool CheckIfEndOfTheGame(out IReadOnlyList<GeishaType> convincedToPlayerOne, out IReadOnlyList<GeishaType> convincedToPlayerTwo)
    {
        var geishaScore = new Dictionary<GeishaType, (int playerOneScore, int playerTwoScore)>();

        foreach (var geishaType in Enum.GetValues<GeishaType>())
        {
            var playerOneScore = PlayerOneData.CountPointsForGeishaType(geishaType);
            var playerTwoScore = PlayerTwoData.CountPointsForGeishaType(geishaType);

            playerOneScore += PlayerOneData.SecretCard?.Type == geishaType ? 1 : 0;
            playerTwoScore += PlayerTwoData.SecretCard?.Type == geishaType ? 1 : 0;

            geishaScore.Add(geishaType, (playerOneScore, playerTwoScore));
        }

        convincedToPlayerOne = geishaScore.Where(x => x.Value.playerOneScore > x.Value.playerTwoScore).Select(x => x.Key).ToList();
        convincedToPlayerTwo = geishaScore.Where(x => x.Value.playerOneScore < x.Value.playerTwoScore).Select(x => x.Key).ToList();

        var playerOnePoints = convincedToPlayerOne.Select(x => GeishaConstants.GeishaPoints[x]).Sum();
        var playerTwoPoints = convincedToPlayerTwo.Select(x => GeishaConstants.GeishaPoints[x]).Sum();

        const int pointsToWin = 11;
        const int convincedGeishaToWin = 4;

        return (playerOnePoints > pointsToWin ||
                playerTwoPoints > pointsToWin ||
                convincedToPlayerOne.Count() >= convincedGeishaToWin ||
                convincedToPlayerTwo.Count() >= convincedGeishaToWin);
    }

    private List<GiftCard> GetRandomCards(int numberOfCards)
    {
        var cardsForPlayer = new List<GiftCard>();
        for (int i = 0; i < numberOfCards; i++)
        {
            var randomCardIndex = _random.Next(CardDeck.Count());
            cardsForPlayer.Add(CardDeck[randomCardIndex]);
            CardDeck.RemoveAt(randomCardIndex);
        }

        return cardsForPlayer;
    }

 
    private void PrintGameState()
    {
        ConsoleWrapper.PrintGeishaStates(PlayerOneData, PlayerTwoData, convincedToPlayerInPreviousRound, PlayerOne.ToString(), PlayerTwo.ToString());
        const string HandLabel = "Hand";
        const string SecretLabel = "Secret";
        const string EliminationCardsLabel = "Elimination Cards";
        
        // hand
        var cardsInHandPlayerOneTable = ConsoleWrapper.GetCardsTable(PlayerOneData.CardsOnHand, HandLabel);
        var cardsInHandPlayerTwoTable = ConsoleWrapper.GetCardsTable(PlayerTwoData.CardsOnHand, HandLabel);
        
        // deck
        var deckCardsTable = ConsoleWrapper.GetCardsTable(CardDeck, "Deck");
        
        // secret
        var secretsPlayerOne = PlayerOneData.SecretCard is not null ? new List<GiftCard>{ PlayerOneData.SecretCard } : new List<GiftCard>();
        var secretsPlayerTwo = PlayerTwoData.SecretCard is not null ? new List<GiftCard> { PlayerTwoData.SecretCard } : new List<GiftCard>();
        var playerOneSecretTable = ConsoleWrapper.GetCardsTable(secretsPlayerOne, SecretLabel);
        var playerTwoSecretTable = ConsoleWrapper.GetCardsTable(secretsPlayerTwo, SecretLabel);


        // elimination
        var eliminationPlayerOne = PlayerOneData.EliminatedCards is not null
            ? new List<GiftCard> (PlayerOneData.EliminatedCards)
            : new List<GiftCard>();
        var eliminationPlayerTwo = PlayerTwoData.EliminatedCards is not null
            ? new List<GiftCard> (PlayerTwoData.EliminatedCards)
            : new List<GiftCard>();
        var eliminationPlayerOneTable = ConsoleWrapper.GetCardsTable(eliminationPlayerOne, EliminationCardsLabel);
        var eliminationPlayerTwoTable = ConsoleWrapper.GetCardsTable(eliminationPlayerTwo, EliminationCardsLabel);

        // First column
        var tablePlayerOne = new Table();
        tablePlayerOne.AddColumn(new TableColumn(PlayerOne.ToString()));
        tablePlayerOne.AddRow(cardsInHandPlayerOneTable);
        tablePlayerOne.AddRow(playerOneSecretTable);
        tablePlayerOne.AddRow(eliminationPlayerOneTable);

        // Second column
        var tablePlayerTwo = new Table();
        tablePlayerTwo.AddColumn(new TableColumn(PlayerTwo.ToString()));
        tablePlayerTwo.AddRow(cardsInHandPlayerTwoTable);
        tablePlayerTwo.AddRow(playerTwoSecretTable);
        tablePlayerTwo.AddRow(eliminationPlayerTwoTable);

        var table = new Table();
        table.AddColumn(new TableColumn(tablePlayerOne));
        table.AddColumn(new TableColumn(tablePlayerTwo));
        table.AddColumn(new TableColumn(deckCardsTable));
        AnsiConsole.Write(table);
    }

    private void SwitchPlayer()
    {
        var isPlayerOneCurrent = PlayerOne == CurrentPlayer;
        CurrentPlayer = isPlayerOneCurrent ? PlayerTwo : PlayerOne;
        CurrentPlayerData = isPlayerOneCurrent ? PlayerTwoData : PlayerOneData;
    }
}
