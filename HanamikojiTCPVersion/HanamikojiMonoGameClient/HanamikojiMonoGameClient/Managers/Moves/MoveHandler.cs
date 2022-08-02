using System;
using System.Collections.Generic;
using CommonResources.Game;
using HanamikojiMonoGameClient.GameEntities;
using Microsoft.Xna.Framework.Input;

namespace HanamikojiMonoGameClient.Managers.Moves;

public abstract class MoveHandler
{
    protected IEntitiesRepository EntitiesRepository;

    protected MoveHandler(IEntitiesRepository entitiesRepository)
    {
        EntitiesRepository = entitiesRepository;
    }

    public abstract void Update(GameData gameData, MouseState mouseState);
    public virtual bool Validate() => true;
    public abstract MoveData GetMoveData(GameData gameData);
    public abstract void Clear();
}

