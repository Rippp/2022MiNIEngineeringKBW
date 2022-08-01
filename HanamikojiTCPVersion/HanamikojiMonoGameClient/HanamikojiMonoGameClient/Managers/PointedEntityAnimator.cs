using System;
using System.Collections.Generic;
using System.Linq;
using CommonResources.Game;
using HanamikojiMonoGameClient.GameEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HanamikojiMonoGameClient.Managers;

public class PointedCardAnimator
{
    private readonly IDictionary<Guid, GiftCardEntity> _giftCardEntityDictionary;

    private GiftCardEntity? _pointedCardEntityValue = null;
    private GiftCardEntity? _pointedCardEntity
    {
        get => _pointedCardEntityValue;
        set
        {
            _pointedCardEntityValue?.Enlarge(false);
            value?.Enlarge(true);
            _pointedCardEntityValue = value;
        }
    }

    public PointedCardAnimator(IDictionary<Guid, GiftCardEntity> giftCardEntityDictionary)
    {
        _giftCardEntityDictionary = giftCardEntityDictionary;
    }

    public void Update(GameData gameData, MouseState mouseState)
    {
        var cardsOnHand = gameData.CurrentPlayerData.CardsOnHand;
        var mousePosition = new Vector2(mouseState.X, mouseState.Y);

        var pointedCardsOnHand = new List<GiftCardEntity>();

        foreach (var card in cardsOnHand)
        {
            var giftCardEntity = _giftCardEntityDictionary[card.CardId];

            if (giftCardEntity.IsPointInsideSprite(mousePosition))
            {
                pointedCardsOnHand.Add(giftCardEntity);
            }
        }

        _pointedCardEntity = pointedCardsOnHand.MaxBy(x => x.DrawOrder);
    }
}