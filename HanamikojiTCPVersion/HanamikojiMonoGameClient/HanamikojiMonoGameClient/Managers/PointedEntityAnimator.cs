using System;
using System.Collections.Generic;
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


        var isAnyCardPointed = false;
        for (int i = 0; i < cardsOnHand.Count; i++)
        {
            var giftCardEntity = _giftCardEntityDictionary[cardsOnHand[i].CardId];

            if (i == cardsOnHand.Count - 1)
            {
                if (giftCardEntity.IsPointInsideSprite(mousePosition))
                {
                    isAnyCardPointed = true;
                    _pointedCardEntity = giftCardEntity;
                }
            }
            else
            {
                if (giftCardEntity.IsPointInsideLeftHalfOfSprite(mousePosition))
                {
                    isAnyCardPointed = true;
                    _pointedCardEntity = giftCardEntity;
                }
            }
        }

        if (!isAnyCardPointed) _pointedCardEntity = null;
    }
}