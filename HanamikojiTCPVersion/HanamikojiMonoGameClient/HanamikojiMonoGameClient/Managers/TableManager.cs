using System;
using System.Collections.Generic;
using System.Linq;
using CommonResources.Game;
using HanamikojiMonoGameClient.Animations;
using HanamikojiMonoGameClient.GameEntities;
using HanamikojiMonoGameClient.Sprites;
using Microsoft.Xna.Framework;

namespace HanamikojiMonoGameClient.Managers;

public class TableManager
{
    private readonly AnimationManager _animationManager = new();
    
    private List<MoveCardEntity> _playerMoves;
    private List<MoveCardEntity> _opponentMoves;

    private GameData _lastReceivedGameData = null;

    private readonly IEntitiesRepository _entitiesRepository;

    public TableManager(IEntitiesRepository entitiesRepository)
    {
        _entitiesRepository = entitiesRepository;
    }

    public void Update(GameData gameData, GameTime gameTime)
    {
        _animationManager.Update(gameTime);
        if (!gameData.Equals(_lastReceivedGameData))
        {
            if (_lastReceivedGameData == null)
            {
                InitTableEntities();
            }

            CreateMissingGiftCards(gameData);
            ChangeVisibilityOfPlayerMoves(gameData);
            ChangeVisibilityOfOpponentMoves(gameData);
            RevealCards(gameData);

            InitMoveCardsToTheirDestination(gameData);
            
            _lastReceivedGameData = gameData;
        }
    }

    private void InitTableEntities()
    {
        var playerMoves = new List<MoveCardEntity>
        {
            new MoveCardEntity(PlayerMoveTypeEnum.Secret, true),
            new MoveCardEntity(PlayerMoveTypeEnum.Compromise, true),
            new MoveCardEntity(PlayerMoveTypeEnum.DoubleGift, true),
            new MoveCardEntity(PlayerMoveTypeEnum.Elimination, true),
        };

        var opponentMoves = new List<MoveCardEntity>
        {
            new MoveCardEntity(PlayerMoveTypeEnum.Secret, false),
            new MoveCardEntity(PlayerMoveTypeEnum.Compromise, false),
            new MoveCardEntity(PlayerMoveTypeEnum.DoubleGift, false),
            new MoveCardEntity(PlayerMoveTypeEnum.Elimination, false),
        };

        var geishaIcons = new List<GeishaEntity>
        {
            new GeishaEntity(GeishaType.Geisha2_A),
            new GeishaEntity(GeishaType.Geisha2_B),
            new GeishaEntity(GeishaType.Geisha2_C),
            new GeishaEntity(GeishaType.Geisha3_A),
            new GeishaEntity(GeishaType.Geisha3_B),
            new GeishaEntity(GeishaType.Geisha4_A),
            new GeishaEntity(GeishaType.Geisha5_A),
        };

        _playerMoves = playerMoves;
        _opponentMoves = opponentMoves;

        var topDeckCard = new GiftCardEntity(GeishaType.AnonymizedGeisha, Guid.NewGuid());
        topDeckCard.MoveToPosition(EntitiesPositions.TopDeckCardPosition);

        _entitiesRepository.Add(topDeckCard);

        playerMoves.ForEach(_entitiesRepository.Add);
        opponentMoves.ForEach(_entitiesRepository.Add);
        geishaIcons.ForEach(_entitiesRepository.Add);
    }

    private void InitMoveCardsToTheirDestination(GameData gameData)
    {
        PlayerHandInitMove(gameData);
        OpponentHandInitMove(gameData);

        PlayerSecretInitMove(gameData);
        OpponentSecretCardInitMove(gameData);

        PlayerEliminationCardsInitMove(gameData);
        OpponentEliminationCardsInitMove(gameData);

        PlayerGiftsInitMove(gameData);
        OpponentGiftsInitMove(gameData);

        // Table manager or MoveHandler?
        if (gameData.DoubleGiftCards != null)
        {
            var cardPosition = EntitiesPositions.FirstPlayerTradeOfferCardPosition;
            for (int i = 0; i < gameData.DoubleGiftCards.Count; i++)
            {
                var cardEntity = _entitiesRepository.GetByCardId(gameData.DoubleGiftCards[i].CardId);

                cardPosition.X += cardEntity.Width / 2.0f;

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
                var cardEntity = _entitiesRepository.GetByCardId(gameData.CompromiseCards[i].CardId);
                cardPosition.X += cardEntity.Width / 2.0f;
                _animationManager.AddMoveAnimationToDestination(cardEntity, cardPosition);
                _animationManager.AddRotationAnimation(cardEntity, 0);
            }
        }
    }

    private void OpponentGiftsInitMove(GameData gameData)
    {
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

                var giftCardEntity = _entitiesRepository.GetByCardId(giftCard.CardId);

                _animationManager.AddMoveAnimationToDestination(giftCardEntity, cardPosition);
                _animationManager.AddRotationAnimation(giftCardEntity, (float) Math.PI);
            }
        }
    }

    private void PlayerGiftsInitMove(GameData gameData)
    {
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

                var giftCardEntity = _entitiesRepository.GetByCardId(giftCard.CardId);

                _animationManager.AddMoveAnimationToDestination(giftCardEntity, cardPosition);
                _animationManager.AddRotationAnimation(giftCardEntity, 0);
            }
        }
    }

    private void OpponentEliminationCardsInitMove(GameData gameData)
    {
        if (gameData.OtherPlayerData.EliminationCards != null)
        {
            var eliminatedOpponentCardPosition = EntitiesPositions.OpponentEliminatedPosition;
            foreach (var eliminatedCards in gameData.OtherPlayerData.EliminationCards)
            {
                var eliminatedCardEntity = _entitiesRepository.GetByCardId(eliminatedCards.CardId);
                _animationManager.AddMoveAnimationToDestination(eliminatedCardEntity, eliminatedOpponentCardPosition);
                eliminatedOpponentCardPosition =
                    new Vector2(eliminatedOpponentCardPosition.X + (3 * eliminatedCardEntity.Width) / 4,
                        eliminatedOpponentCardPosition.Y);
            }
        }
    }

    private void PlayerEliminationCardsInitMove(GameData gameData)
    {
        if (gameData.CurrentPlayerData.EliminationCards != null)
        {
            var eliminatedPlayerCardPosition = EntitiesPositions.PlayerEliminatedPosition;
            foreach (var eliminatedCards in gameData.CurrentPlayerData.EliminationCards)
            {
                var eliminatedCardEntity = _entitiesRepository.GetByCardId(eliminatedCards.CardId);
                _animationManager.AddMoveAnimationToDestination(eliminatedCardEntity, eliminatedPlayerCardPosition);
                eliminatedPlayerCardPosition = new Vector2(
                    eliminatedPlayerCardPosition.X + eliminatedCardEntity.Width / 2.0f,
                    eliminatedPlayerCardPosition.Y);
            }
        }
    }

    private void OpponentSecretCardInitMove(GameData gameData)
    {
        var opponentSecretCard = gameData.OtherPlayerData.SecretCard;
        if (opponentSecretCard != null)
        {
            var secretCardEntity = _entitiesRepository.GetByCardId(opponentSecretCard.CardId);
            _animationManager.AddMoveAnimationToDestination(secretCardEntity, EntitiesPositions.OpponentSecretPosition);
        }
    }

    private void PlayerSecretInitMove(GameData gameData)
    {
        var playerSecretCard = gameData.CurrentPlayerData.SecretCard;
        if (playerSecretCard != null)
        {
            var secretCardEntity = _entitiesRepository.GetByCardId(playerSecretCard.CardId);
            _animationManager.AddMoveAnimationToDestination(secretCardEntity, EntitiesPositions.PlayerSecretPosition);
        }
    }

    private void OpponentHandInitMove(GameData gameData)
    {
        var opponentPlayerCardDestination = EntitiesPositions.FirstOpponentCardPosition;
        var nextOpponentCardDrawOrder = GiftCardEntity.DefaultDrawOrder;
        foreach (var card in gameData.OtherPlayerData.CardsOnHand)
        {
            var giftCardEntity = _entitiesRepository.GetByCardId(card.CardId);
            giftCardEntity.SetDrawOrder(nextOpponentCardDrawOrder);
            nextOpponentCardDrawOrder++;
            _animationManager.AddMoveAnimationToDestination(giftCardEntity, opponentPlayerCardDestination);
            if (giftCardEntity.Position == EntitiesPositions.TopDeckCardPosition)
                _animationManager.AddRotationAnimation(giftCardEntity, (float) Math.PI);
            opponentPlayerCardDestination = new Vector2(opponentPlayerCardDestination.X + giftCardEntity.Width / 2.0f,
                opponentPlayerCardDestination.Y);
        }
    }

    private void PlayerHandInitMove(GameData gameData)
    {
        var nextPlayerCardDestination = EntitiesPositions.FirstPlayerCardPosition;
        var nextPlayerCardDrawOrder = GiftCardEntity.DefaultDrawOrder;
        foreach (var card in gameData.CurrentPlayerData.CardsOnHand)
        {
            var giftCardEntity = _entitiesRepository.GetByCardId(card.CardId);
            giftCardEntity.SetDrawOrder(nextPlayerCardDrawOrder);
            nextPlayerCardDrawOrder++;
            _animationManager.AddMoveAnimationToDestination(giftCardEntity, nextPlayerCardDestination);
            _animationManager.AddRotationAnimation(giftCardEntity, 0);
            nextPlayerCardDestination = new Vector2(nextPlayerCardDestination.X + giftCardEntity.Width / 2.0f,
                nextPlayerCardDestination.Y);
        }
    }

    private void CreateMissingGiftCards(GameData gameData)
    {
        var missingGiftCards = GetMissingGiftCards(gameData);
        foreach (var card in missingGiftCards)
        {
            var giftCardEntity = new GiftCardEntity(card.Type, card.CardId);
            giftCardEntity.MoveToPosition(EntitiesPositions.TopDeckCardPosition);

            _entitiesRepository.Add(giftCardEntity);
        }
    }

    public List<GiftCard> GetMissingGiftCards(GameData gameData)
    {
        var existingCardIds = _entitiesRepository.GetAllGiftCards().Select(x => x.CardId).ToList();

        return gameData.GetAllCards().Where(x => !existingCardIds.Contains(x.CardId)).ToList();
    }

    private void RevealCards(GameData gameData)
    {
        var allCards = gameData.GetAllCards();

        foreach (var card in allCards)
        {
            var cardEntity = _entitiesRepository.GetByCardId(card.CardId);

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