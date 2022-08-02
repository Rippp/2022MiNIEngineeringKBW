using System;
using System.Collections.Generic;
using System.Linq;
using CommonResources.Game;
using HanamikojiMonoGameClient.GameEntities;
using HanamikojiMonoGameClient.Providers;
using Microsoft.Xna.Framework.Input;

namespace HanamikojiMonoGameClient.Managers;

public class PointedCardAnimator : IPointedCardAnimator
{
    private readonly PointedEntityProvider _pointedEntityProvider;

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

    private List<GiftCardEntity> _pointedGiftCardOfferEntitiesValue = null;
    private List<GiftCardEntity> _pointedGiftCardOfferEntities
    {
        get => _pointedGiftCardOfferEntitiesValue;
        set
        {
            if(_pointedGiftCardOfferEntitiesValue != null)
                foreach(var cardEntity in _pointedGiftCardOfferEntitiesValue) 
                    cardEntity.Enlarge(false);

            if (value != null)
                foreach (var cardEntity in value)
                    cardEntity.Enlarge(true);

            _pointedGiftCardOfferEntitiesValue = value;
        }
    }

    public PointedCardAnimator(PointedEntityProvider pointedEntityProvider)
    {
        _pointedEntityProvider = pointedEntityProvider;
    }

    public void Update(GameData gameData, MouseState mouseState)
    {
        _pointedCardEntity = _pointedEntityProvider.GetPointedCardOnHand(gameData, mouseState);

        _pointedGiftCardOfferEntities = _pointedEntityProvider.GetPointedDoubleGiftOfferCards(gameData, mouseState);
    }
}

public interface IPointedCardAnimator
{
    public void Update(GameData gameData, MouseState mouseState);
}