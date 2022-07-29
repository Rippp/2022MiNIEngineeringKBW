using System;
using System.Collections.Generic;
using System.Linq;
using CommonResources.Game;
using HanamikojiMonoGameClient.GameEntities;
using Microsoft.Xna.Framework;

namespace HanamikojiMonoGameClient;

public class TableManager
{
    private MoveAnimationManager moveAnimationManager = new();
    
    private List<MoveCardEntity> _playerMoves;
    private List<MoveCardEntity> _opponentMoves;

    private IDictionary<Guid, GiftCardEntity> _giftCardEntityDictionary;
    

    private GameData _lastReceivedGameData = null;

    private List<GameEntity> _recentlyAddedEntities = new List<GameEntity>();

    public TableManager(List<MoveCardEntity> playerMoves, List<MoveCardEntity> opponentMoves)
    {
        _playerMoves = playerMoves;
        _opponentMoves = opponentMoves;
        _giftCardEntityDictionary = new Dictionary<Guid, GiftCardEntity>();
    }

    public void Update(GameData gameData, GameTime gameTime)
    {
        moveAnimationManager.Update(gameTime);

        if (!gameData.Equals(_lastReceivedGameData))
        {
            CreateMissingGiftCards(gameData);
            ChangeVisibilityOfPlayerMoves(gameData);
            ChangeVisibilityOfOpponentMoves(gameData);

            InitializeMoveCardsToTheirDestination(gameData);
            
            _lastReceivedGameData = gameData;
        }
    }

    private void InitializeMoveCardsToTheirDestination(GameData gameData)
    {
        var cardsOnHand = gameData.CurrentPlayerData.CardsOnHand;

        var nextCardDestination = EntitiesPositions.FirstPlayerCardPosition;
        foreach (var card in cardsOnHand)
        {
            var giftCardEntity = _giftCardEntityDictionary[card.CardId];

            moveAnimationManager.AddMoveAnimationToDestination(giftCardEntity, nextCardDestination);

            nextCardDestination = new Vector2(nextCardDestination.X + giftCardEntity.Width / 2, nextCardDestination.Y);
        }
    }

    private void CreateMissingGiftCards(GameData gameData)
    {
        var missingGiftCards = GetMissingGiftCards(_giftCardEntityDictionary.Keys, gameData);
        foreach (var card in missingGiftCards)
        {
            var giftCardEntity = new GiftCardEntity(card.Type, card.CardId);

            _giftCardEntityDictionary[card.CardId] = giftCardEntity;
            _recentlyAddedEntities.Add(giftCardEntity);
        }
    }

    public static List<GiftCard> GetMissingGiftCards(ICollection<Guid> existingCards, GameData gameData)
        //    => gameData.GetAllCards().Where(x => !existingCards.Contains(x.CardId)).ToList();
        => gameData.CurrentPlayerData.CardsOnHand.Where(x => !existingCards.Contains(x.CardId)).ToList();

    public List<GameEntity> GiveRecentlyAddedEntities()
    {
        var gameEntitiesToGive = _recentlyAddedEntities.ToList();
        _recentlyAddedEntities = new List<GameEntity>();

        return gameEntitiesToGive;
    }

    private void ChangeVisibilityOfOpponentMoves(GameData gameData)
    {
        foreach (var move in gameData.OtherPlayerData.movesAvailability)
        {
            var moveType = move.Key;
            var isMoveAvailable = move.Value;

            var relatedMoveCard = _opponentMoves.Single(x => x.MoveType == moveType);

            if (isMoveAvailable)
            {
                relatedMoveCard.MoveTo(EntitiesPositions.OpponentMoveCardDefaultPositionDictionary[moveType]);
            }
            else
            {
                relatedMoveCard.Hide();
            }
        }
    }

    private void ChangeVisibilityOfPlayerMoves(GameData gameData)
    {
        foreach (var move in gameData.CurrentPlayerData.movesAvailability)
        {
            var moveType = move.Key;
            var isMoveAvailable = move.Value;

            var relatedMoveCard = _playerMoves.Single(x => x.MoveType == moveType);

            if (isMoveAvailable)
            {
                relatedMoveCard.MoveTo(EntitiesPositions.PlayerMoveCardDefaultPositionDictionary[moveType]);
            }
            else
            {
                relatedMoveCard.Hide();
            }
        }
    }
}


public static class EntitiesPositions
{
    public static IDictionary<PlayerMoveTypeEnum, Vector2> PlayerMoveCardDefaultPositionDictionary =
        new Dictionary<PlayerMoveTypeEnum, Vector2>
        {
            {PlayerMoveTypeEnum.Compromise, new Vector2(1000,750)},
            {PlayerMoveTypeEnum.DoubleGift, new Vector2(1120,750)},
            {PlayerMoveTypeEnum.Elimination, new Vector2(1240,750)},
            {PlayerMoveTypeEnum.Secret, new Vector2(1360,750)},
        };

    public static IDictionary<PlayerMoveTypeEnum, Vector2> OpponentMoveCardDefaultPositionDictionary =
        new Dictionary<PlayerMoveTypeEnum, Vector2>
        {
            {PlayerMoveTypeEnum.Compromise, new Vector2(100,100)},
            {PlayerMoveTypeEnum.DoubleGift, new Vector2(220,100)},
            {PlayerMoveTypeEnum.Elimination, new Vector2(340,100)},
            {PlayerMoveTypeEnum.Secret, new Vector2(460,100)},
        };

    public static Vector2 FirstPlayerCardPosition = new Vector2(200, 700);

    public static Vector2 FirstOpponentCardPosition;
}