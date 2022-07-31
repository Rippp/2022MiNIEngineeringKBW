using System;
using System.Collections.Generic;
using System.Linq;
using CommonResources.Game;
using HanamikojiMonoGameClient.GameEntities;
using HanamikojiMonoGameClient.Sprites;
using Microsoft.Xna.Framework;

namespace HanamikojiMonoGameClient.Managers;

public class TableManager
{
    private AnimationManager _animationManager = new();
    
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
        _animationManager.Update(gameTime);

        if (!gameData.Equals(_lastReceivedGameData))
        {
            CreateMissingGiftCards(gameData);
            ChangeVisibilityOfPlayerMoves(gameData);
            ChangeVisibilityOfOpponentMoves(gameData);
            RevealCards(gameData);

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
            _animationManager.AddMoveAnimationToDestination(giftCardEntity, nextPlayerCardDestination);
            nextPlayerCardDestination = new Vector2(nextPlayerCardDestination.X + giftCardEntity.Width / 2, nextPlayerCardDestination.Y);
        }

        var opponentPlayerCardDestination = EntitiesPositions.FirstOpponentCardPosition;
        foreach (var card in gameData.OtherPlayerData.CardsOnHand)
        {
            var giftCardEntity = _giftCardEntityDictionary[card.CardId];
            _animationManager.AddMoveAnimationToDestination(giftCardEntity, opponentPlayerCardDestination);
            if (giftCardEntity.IsHidden) _animationManager.AddRotationAnimation(giftCardEntity, (float)Math.PI);
            opponentPlayerCardDestination = new Vector2(opponentPlayerCardDestination.X + giftCardEntity.Width / 2, opponentPlayerCardDestination.Y);
        }

        var playerSecretCard = gameData.CurrentPlayerData.SecretCard;
        if (playerSecretCard != null)
        {
            var secretCardEntity = _giftCardEntityDictionary[playerSecretCard.CardId];
            _animationManager.AddMoveAnimationToDestination(secretCardEntity, EntitiesPositions.PlayerSecretPosition);
        }

        var opponentSecretCard = gameData.OtherPlayerData.SecretCard;
        if (opponentSecretCard != null)
        {
            var secretCardEntity = _giftCardEntityDictionary[opponentSecretCard.CardId];
            _animationManager.AddMoveAnimationToDestination(secretCardEntity, EntitiesPositions.OpponentSecretPosition);
        }

        if (gameData.CurrentPlayerData.EliminationCards != null)
        {
            var eliminatedPlayerCardPosition = EntitiesPositions.PlayerEliminatedPosition;
            foreach (var eliminatedCards in gameData.CurrentPlayerData.EliminationCards)
            {
                var eliminatedCardEntity = _giftCardEntityDictionary[eliminatedCards.CardId];
                _animationManager.AddMoveAnimationToDestination(eliminatedCardEntity, eliminatedPlayerCardPosition);
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
                _animationManager.AddMoveAnimationToDestination(eliminatedCardEntity, eliminatedOpponentCardPosition);
                eliminatedOpponentCardPosition =
                    new Vector2(eliminatedOpponentCardPosition.X + (3*eliminatedCardEntity.Width) / 4, eliminatedOpponentCardPosition.Y);
            }
        }

        if (gameData.CurrentPlayerData.GiftsFromPlayer != null)
        {
            var currentGiftsCount = new Dictionary<GeishaType, int>();
            foreach (var giftCard in gameData.CurrentPlayerData.GiftsFromPlayer)
            {
                var cardPosition = EntitiesPositions.GetPlayerGiftForGeishaFirstPosition(giftCard.Type);

                if (currentGiftsCount.ContainsKey(giftCard.Type))
                {
                    cardPosition.Y += 20 * currentGiftsCount[giftCard.Type];
                }
                else
                {
                    currentGiftsCount[giftCard.Type] = 1;
                }

                var giftCardEntity = _giftCardEntityDictionary[giftCard.CardId];

                _animationManager.AddMoveAnimationToDestination(giftCardEntity, cardPosition);
                _animationManager.AddRotationAnimation(giftCardEntity, 0);
            }
        }

        if (gameData.OtherPlayerData.GiftsFromPlayer != null)
        {
            var currentGiftsCount = new Dictionary<GeishaType, int>();
            foreach (var giftCard in gameData.OtherPlayerData.GiftsFromPlayer)
            {
                var cardPosition = EntitiesPositions.GetOpponentGiftForGeishaFirstPosition(giftCard.Type);

                if (currentGiftsCount.ContainsKey(giftCard.Type))
                {
                    cardPosition.Y -= 20 * currentGiftsCount[giftCard.Type];
                }
                else
                {
                    currentGiftsCount[giftCard.Type] = 1;
                }

                var giftCardEntity = _giftCardEntityDictionary[giftCard.CardId];

                _animationManager.AddMoveAnimationToDestination(giftCardEntity, cardPosition);
                _animationManager.AddRotationAnimation(giftCardEntity, (float)Math.PI);
            }
        }

        if (gameData.DoubleGiftCards != null)
        {
            var cardPosition = EntitiesPositions.FirstPlayerTradeOfferCardPosition;
            for (int i = 0; i < gameData.DoubleGiftCards.Count; i++)
            {
                var cardEntity = _giftCardEntityDictionary[gameData.DoubleGiftCards[i].CardId];

                cardPosition.X += cardEntity.Width / 2;

                if (i == 2) cardPosition.X += cardEntity.Width;

                _animationManager.AddMoveAnimationToDestination(cardEntity, cardPosition);
                _animationManager.AddRotationAnimation(cardEntity, 0);
            }
        }

        if(gameData.CompromiseCards != null)
        {
            var cardPosition = EntitiesPositions.FirstPlayerTradeOfferCardPosition;
            for (int i = 0; i < gameData.CompromiseCards.Count; i++)
            {
                var cardEntity = _giftCardEntityDictionary[gameData.CompromiseCards[i].CardId];
                cardPosition.X += cardEntity.Width / 2;
                _animationManager.AddMoveAnimationToDestination(cardEntity, cardPosition);
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

    public static List<GiftCard> GetMissingGiftCards(ICollection<Guid> existingCardIds, GameData gameData)
        => gameData.GetAllCards().Where(x => !existingCardIds.Contains(x.CardId)).ToList();

    public List<GameEntity> GiveRecentlyAddedEntities()
    {
        var gameEntitiesToGive = _recentlyAddedEntities.ToList();
        _recentlyAddedEntities = new List<GameEntity>();

        return gameEntitiesToGive;
    }

    private void RevealCards(GameData gameData)
    {
        var allCards = gameData.GetAllCards();

        foreach (var card in allCards)
        {
            var cardEntity = _giftCardEntityDictionary[card.CardId];

            if (cardEntity.GeishaType == GeishaType.AnonymizedGeisha && card.Type != GeishaType.AnonymizedGeisha)
            {
                cardEntity.RevealCard(card.Type);
            }
        }
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
    public static readonly Vector2 FirstPlayerCardPosition = new Vector2(700, 800);

    public static readonly Vector2 FirstOpponentCardPosition = new Vector2(700, 0);

    public static readonly Vector2 PlayerSecretPosition = new Vector2(1300, 650);
    public static readonly Vector2 OpponentSecretPosition = new Vector2(50,150);

    public static readonly Vector2 PlayerEliminatedPosition = new Vector2(PlayerSecretPosition.X + (int)(1.5 * SpritesProvider.CardWidth), PlayerSecretPosition.Y);
    public static readonly Vector2 OpponentEliminatedPosition = new Vector2(OpponentSecretPosition.X + (int)(1.5 * SpritesProvider.CardWidth), OpponentSecretPosition.Y);

    public static readonly Vector2 FirstPlayerTradeOfferCardPosition = new Vector2(FirstOpponentCardPosition.X, (int)(FirstOpponentCardPosition.Y + 1.1 * SpritesProvider.CardHeight));

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

    private static readonly Vector2 _firstGeishaPosition = new Vector2(100, GameSettings.WINDOW_HEIGHT / 2 - SpritesProvider.GeishaSize / 2);
    private const int _gapBetweenGeisha = 25;


    public static IDictionary<GeishaType, Vector2> geishaPositions =
        new Dictionary<GeishaType, Vector2>
        {
            {GeishaType.Geisha2_A, _firstGeishaPosition },
            {GeishaType.Geisha2_B, _firstGeishaPosition + new Vector2(SpritesProvider.GeishaSize + _gapBetweenGeisha,0) },
            {GeishaType.Geisha2_C, _firstGeishaPosition + new Vector2(2*(SpritesProvider.GeishaSize + _gapBetweenGeisha) ,0)},
            {GeishaType.Geisha3_A, _firstGeishaPosition + new Vector2(3*(SpritesProvider.GeishaSize + _gapBetweenGeisha) ,0)},
            {GeishaType.Geisha3_B, _firstGeishaPosition + new Vector2(4*(SpritesProvider.GeishaSize + _gapBetweenGeisha) ,0)},
            {GeishaType.Geisha4_A, _firstGeishaPosition + new Vector2(5*(SpritesProvider.GeishaSize + _gapBetweenGeisha) ,0)},
            {GeishaType.Geisha5_A, _firstGeishaPosition + new Vector2(6*(SpritesProvider.GeishaSize + _gapBetweenGeisha),0)},
        };

    public static Vector2 GetPlayerGiftForGeishaFirstPosition(GeishaType geishaType)
    {
        var geishaIconPosition = geishaPositions[geishaType];

        return new Vector2(geishaIconPosition.X, geishaIconPosition.Y + SpritesProvider.GeishaSize + 10);
    }

    public static Vector2 GetOpponentGiftForGeishaFirstPosition(GeishaType geishaType)
    {
        var geishaIconPosition = geishaPositions[geishaType];

        return new Vector2(geishaIconPosition.X, geishaIconPosition.Y - SpritesProvider.CardHeight - 10);
    }
}