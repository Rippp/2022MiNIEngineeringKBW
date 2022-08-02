using CommonResources.Game;
using HanamikojiMonoGameClient.GameEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace HanamikojiMonoGameClient.Providers;

public class PointedEntityProvider
{
    private readonly IEntitiesRepository _entitiesRepository;
    private GiftCardEntity _pointedGiftCardEntityOnHand;
    private List<GiftCardEntity> _pointedDoubleGiftOfferCardEntities;

    public PointedEntityProvider(IEntitiesRepository entitiesRepository)
    {
        _entitiesRepository = entitiesRepository;
    }

    public MoveCardEntity GetPointedMoveCardEntity()
    {
        throw new System.NotImplementedException();
    }

    public GiftCardEntity GetPointedCardOnHand(GameData gameData, MouseState mouseState) => _pointedGiftCardEntityOnHand;
    public List<GiftCardEntity> GetPointedDoubleGiftOfferCards(GameData gameData, MouseState mouse) => _pointedDoubleGiftOfferCardEntities;

    public void Update(GameData gameData, MouseState mouseState)
    {
        if(gameData.IsMovePossible(PlayerMoveTypeEnum.DoubleGiftOffer))
            UpdatePointedDoubleGiftCardOffer(gameData, mouseState);
        else
            UpdatePointedCardOnHand(gameData, mouseState);
    }

    private void UpdatePointedDoubleGiftCardOffer(GameData gameData, MouseState mouseState)
    {
        var doubleGiftOfferCards = gameData.DoubleGiftCards;
        var mousePosition = new Vector2(mouseState.X, mouseState.Y);

        var pointedCardsOnHand = new List<GiftCardEntity>();

        foreach (var card in doubleGiftOfferCards)
        {
            var giftCardEntity = _entitiesRepository.GetByCardId(card.CardId);

            if(giftCardEntity.IsPointInsideSprite(mousePosition))
            {
                pointedCardsOnHand.Add(giftCardEntity);
            }
        }

        var topCardPointed = pointedCardsOnHand.MaxBy(x => x.DrawOrder);

        if (topCardPointed != null)
        {
            var index = doubleGiftOfferCards.Select(x => x.CardId).ToList().IndexOf(topCardPointed.CardId);

            if (index < 2 && index >= 0)
            {
                _pointedDoubleGiftOfferCardEntities = new List<GiftCardEntity>()
            {
                _entitiesRepository.GetByCardId(doubleGiftOfferCards[0].CardId),
                _entitiesRepository.GetByCardId(doubleGiftOfferCards[1].CardId),
            };
            }
            else if (index >= 2)
            {
                _pointedDoubleGiftOfferCardEntities = new List<GiftCardEntity>()
            {
                _entitiesRepository.GetByCardId(doubleGiftOfferCards[2].CardId),
                _entitiesRepository.GetByCardId(doubleGiftOfferCards[3].CardId),
            };
            }
        }
        else
        {
            _pointedDoubleGiftOfferCardEntities = null;
        }
    }

    private void UpdatePointedCardOnHand(GameData gameData, MouseState mouseState)
    {
        var cardsOnHand = gameData.CurrentPlayerData.CardsOnHand;
        var mousePosition = new Vector2(mouseState.X, mouseState.Y);

        var pointedCardsOnHand = new List<GiftCardEntity>();

        foreach (var card in cardsOnHand)
        {
            var giftCardEntity = _entitiesRepository.GetByCardId(card.CardId);

            if (giftCardEntity.IsPointInsideSprite(mousePosition))
            {
                pointedCardsOnHand.Add(giftCardEntity);
            }
        }

        var topCardPointed = pointedCardsOnHand.MaxBy(x => x.DrawOrder);

        _pointedGiftCardEntityOnHand = topCardPointed;
    }
}
