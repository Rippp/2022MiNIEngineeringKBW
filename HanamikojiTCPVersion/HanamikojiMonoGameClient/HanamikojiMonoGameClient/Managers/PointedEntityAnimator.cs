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
    private readonly IPointedEntityProvider _pointedEntityProvider;

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

    public PointedCardAnimator(IPointedEntityProvider pointedEntityProvider)
    {
        _pointedEntityProvider = pointedEntityProvider;
    }

    public void Update(GameData gameData, MouseState mouseState)
    {
        _pointedCardEntity = _pointedEntityProvider.GetPointedCardOnHand(gameData, mouseState);
    }
}

public interface IPointedCardAnimator
{
    public void Update(GameData gameData, MouseState mouseState);
}