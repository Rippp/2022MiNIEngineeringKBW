using CommonResources.Game;
using HanamikojiMonoGameClient.GameEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace HanamikojiMonoGameClient.Providers;

public interface IPointedEntityProvider
{
    public MoveCardEntity GetPointedMoveCardEntity();

    public GiftCardEntity GetPointedCardOnHand(GameData gameData, MouseState mouseState);

    public void Update(GameData gameData, MouseState mouseState);
}

public class PointedEntityProvider : IPointedEntityProvider
{
    private readonly IEntitiesRepository _entitiesRepository;
    private GiftCardEntity _pointedGiftCardEntityOnHand;

    public PointedEntityProvider(IEntitiesRepository entitiesRepository)
    {
        _entitiesRepository = entitiesRepository;
    }

    public MoveCardEntity GetPointedMoveCardEntity()
    {
        throw new System.NotImplementedException();
    }

    public GiftCardEntity GetPointedCardOnHand(GameData gameData, MouseState mouseState)
    {
        return _pointedGiftCardEntityOnHand;
    }

    public void Update(GameData gameData, MouseState mouseState)
    {
        UpdatePointedCardOnHand(gameData, mouseState);
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
