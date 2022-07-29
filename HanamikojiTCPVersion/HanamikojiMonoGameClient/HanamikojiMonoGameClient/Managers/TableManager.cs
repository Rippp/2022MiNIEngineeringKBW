using System;
using System.Collections.Generic;
using System.Linq;
using CommonResources.Game;
using HanamikojiMonoGameClient.GameEntities;
using HanamikojiMonoGameClient.Managers;
using HanamikojiMonoGameClient.Sprites;
using Microsoft.Xna.Framework;

namespace HanamikojiMonoGameClient;

public class TableManager
{
    private MoveAnimationManager _moveAnimationManager = new();
    
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
        _moveAnimationManager.Update(gameTime);

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
        var nextPlayerCardDestination = EntitiesPositions.FirstPlayerCardPosition;
        foreach (var card in gameData.CurrentPlayerData.CardsOnHand)
        {
            var giftCardEntity = _giftCardEntityDictionary[card.CardId];
            _moveAnimationManager.AddMoveAnimationToDestination(giftCardEntity, nextPlayerCardDestination);
            nextPlayerCardDestination = new Vector2(nextPlayerCardDestination.X + giftCardEntity.Width / 2, nextPlayerCardDestination.Y);
        }

        var opponentPlayerCardDestination = EntitiesPositions.FirstOpponentCardPosition;
        foreach (var card in gameData.OtherPlayerData.CardsOnHand)
        {
            var giftCardEntity = _giftCardEntityDictionary[card.CardId];
            _moveAnimationManager.AddMoveAnimationToDestination(giftCardEntity, opponentPlayerCardDestination);
            opponentPlayerCardDestination = new Vector2(opponentPlayerCardDestination.X + giftCardEntity.Width / 2, opponentPlayerCardDestination.Y);
        }

        var playerSecretCard = gameData.CurrentPlayerData.SecretCard;
        if (playerSecretCard != null)
        {
            var secretCardEntity = _giftCardEntityDictionary[playerSecretCard.CardId];
            _moveAnimationManager.AddMoveAnimationToDestination(secretCardEntity, EntitiesPositions.PlayerSecretPosition);
        }

        var opponentSecretCard = gameData.OtherPlayerData.SecretCard;
        if (opponentSecretCard != null)
        {
            var secretCardEntity = _giftCardEntityDictionary[opponentSecretCard.CardId];
            _moveAnimationManager.AddMoveAnimationToDestination(secretCardEntity, EntitiesPositions.OpponentSecretPosition);
        }

        if (gameData.CurrentPlayerData.EliminationCards != null)
        {
            var eliminatedPlayerCardPosition = EntitiesPositions.PlayerEliminatedPosition;
            foreach (var eliminatedCards in gameData.CurrentPlayerData.EliminationCards)
            {
                var eliminatedCardEntity = _giftCardEntityDictionary[eliminatedCards.CardId];
                _moveAnimationManager.AddMoveAnimationToDestination(eliminatedCardEntity, eliminatedPlayerCardPosition);
                eliminatedPlayerCardPosition = new Vector2(eliminatedPlayerCardPosition.X + eliminatedCardEntity.Width / 2,
                    eliminatedPlayerCardPosition.Y);
            }
        }

        if (gameData.OtherPlayerData.EliminationCards != null)
        {
            var eliminatedOpponentCardPosition = EntitiesPositions.OpponentEliminatedPosition;
            foreach (var eliminatedCards in gameData.OtherPlayerData.EliminationCards)
            {
                var eliminatedCardEntity = _giftCardEntityDictionary[eliminatedCards.CardId];
                _moveAnimationManager.AddMoveAnimationToDestination(eliminatedCardEntity, eliminatedOpponentCardPosition);
                eliminatedOpponentCardPosition =
                    new Vector2(eliminatedOpponentCardPosition.X + eliminatedCardEntity.Width / 2, eliminatedOpponentCardPosition.Y);
            }
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
        => gameData.GetAllCards().Where(x => !existingCards.Contains(x.CardId)).ToList();
        //=> gameData.CurrentPlayerData.CardsOnHand.Where(x => !existingCards.Contains(x.CardId)).ToList();

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
    public static readonly Vector2 FirstPlayerCardPosition = new Vector2(200, 700);

    public static readonly Vector2 FirstOpponentCardPosition = new Vector2(500, 200);

    public static readonly Vector2 PlayerSecretPosition = new Vector2(1300, 650);
    public static readonly Vector2 OpponentSecretPosition = new Vector2(50,150);

    public static readonly Vector2 PlayerEliminatedPosition = new Vector2(PlayerSecretPosition.X + (int)(1.5 * SpritesProvider.CardWidth), PlayerSecretPosition.Y);
    public static readonly Vector2 OpponentEliminatedPosition = new Vector2(OpponentSecretPosition.X + (int)(1.5 * SpritesProvider.CardWidth), OpponentSecretPosition.Y);

    private static readonly Vector2 _firstPlayerMovePosition = new Vector2(1300, 800);
    private static readonly Vector2 _firstOpponentMovePosition = new Vector2(50, 50);

    public static IDictionary<PlayerMoveTypeEnum, Vector2> PlayerMoveCardDefaultPositionDictionary =
        new Dictionary<PlayerMoveTypeEnum, Vector2>
        {
            { PlayerMoveTypeEnum.Compromise, _firstPlayerMovePosition},
            { PlayerMoveTypeEnum.DoubleGift, _firstPlayerMovePosition + new Vector2(75,0) },
            { PlayerMoveTypeEnum.Elimination, _firstPlayerMovePosition + new Vector2(150,0) },
            { PlayerMoveTypeEnum.Secret, _firstPlayerMovePosition + new Vector2(225,0) },
        };

    public static IDictionary<PlayerMoveTypeEnum, Vector2> OpponentMoveCardDefaultPositionDictionary =
        new Dictionary<PlayerMoveTypeEnum, Vector2>
        {
            { PlayerMoveTypeEnum.Compromise, _firstOpponentMovePosition },
            { PlayerMoveTypeEnum.DoubleGift, _firstOpponentMovePosition + new Vector2(75,0) },
            { PlayerMoveTypeEnum.Elimination, _firstOpponentMovePosition + new Vector2(150,0)},
            { PlayerMoveTypeEnum.Secret, _firstOpponentMovePosition + new Vector2(225,0)},
        };
}