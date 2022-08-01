using System;
using System.Collections.Generic;
using CommonResources.Game;
using HanamikojiMonoGameClient.GameEntities;
using Microsoft.Xna.Framework.Input;

namespace HanamikojiMonoGameClient.Managers.Moves;

public abstract class MoveHandler 
{
    protected readonly IDictionary<Guid, GiftCardEntity> _giftCardEntityDictionary;

    protected MoveHandler(IDictionary<Guid, GiftCardEntity> giftCardEntityDictionary)
    {
        _giftCardEntityDictionary = giftCardEntityDictionary;
    }

    public abstract void Update(GameData gameData, MouseState mouseState);
    public virtual bool Validate() => true;
    public abstract MoveData GetMoveData(GameData gameData);
    public abstract void Clear();
}

