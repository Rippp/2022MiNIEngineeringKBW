using System;
using CommonResources.Game;
using HanamikojiMonoGameClient.Providers;
using HanamikojiMonoGameClient.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HanamikojiMonoGameClient.GameEntities;

public class MoveCardEntity : GameEntity
{
    public PlayerMoveTypeEnum MoveType { get; set; }
    public bool IsPlayerMove { get; set; }


    public MoveCardEntity(PlayerMoveTypeEnum moveType, bool isPlayerMove, Vector2? position = null) 
    {
        Sprite = SpritesProvider.GetMoveCardSprite(moveType);
        MoveType = moveType;
        Position = position ?? _hiddenPosition;
        IsPlayerMove = isPlayerMove;
        DrawOrder = 100;
    }

    public void MoveTo(Vector2 position) => Position = position;


    public override void Update(GameTime gameTime)
    {
    }
}