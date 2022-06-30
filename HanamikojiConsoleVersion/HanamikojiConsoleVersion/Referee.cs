﻿using HanamikojiConsoleVersion.GameData;
using HanamikojiConsoleVersion.Moves;

namespace HanamikojiConsoleVersion;

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

    public Referee(Player playerOne, Player playerTwo)
    {
        PlayerOne = playerOne;
        PlayerTwo = playerTwo;
        CurrentPlayer = playerOne;

        PlayerOneData = new PlayerData();
        PlayerTwoData = new PlayerData();
        CurrentPlayerData = PlayerOneData;

        CardDeck = new List<GiftCard>(GiftCardData.AllCards);
        _random = new Random();
    }

    public bool NextRound()
    {
        ConsoleWrapper.ConsoleWriteGameState(PlayerOneData, PlayerTwoData, PlayerOne.ToString(), PlayerTwo.ToString());

        var nextCard = GetRandomCards(1).Single();
        CurrentPlayerData.CardsOnHand.Add(nextCard);

        var move = CurrentPlayer.StartMove(CurrentPlayerData.CardsOnHand);
        move.Execute(this);

        SwitchPlayer();

        return CheckIfEndOfTheGame();
    }

    public bool CheckIfEndOfTheGame() => CardDeck.Count() == 0;

    public void BeginRound()
    {
        PlayerOneData.CardsOnHand.AddRange(GetRandomCards(3));
        PlayerTwoData.CardsOnHand.AddRange(GetRandomCards(3));
    }

    public List<GiftCard> GetRandomCards(int numberOfCards)
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

    public void Execute(GiveOneCardMove move) => CurrentPlayerData.GiftsFromPlayer.Add(move.CardToGive);

    public void Execute(SecretMove move) => CurrentPlayerData.SecretCard = move.SecretCard;    

    public void Execute(CompromiseMove move)
    {
        var selectedCard = OtherPlayer.ChooseCompromiseCard(move.CardToChooseFrom);
        OtherPlayerData.GiftsFromPlayer.Add(selectedCard);
        CurrentPlayerData.GiftsFromPlayer.AddRange(move.CardToChooseFrom.Where(x => x != selectedCard));
    }

    private void SwitchPlayer()
    {
        var isPlayerOneCurrent = PlayerOne == CurrentPlayer;
        CurrentPlayer = isPlayerOneCurrent ? PlayerTwo : PlayerOne;
        CurrentPlayerData = isPlayerOneCurrent ? PlayerTwoData : PlayerOneData;
    }
}

