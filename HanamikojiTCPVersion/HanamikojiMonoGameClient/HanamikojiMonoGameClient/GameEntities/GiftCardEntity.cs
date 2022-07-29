using System;
using CommonResources.Game;
using HanamikojiMonoGameClient.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HanamikojiMonoGameClient.GameEntities;

public class GiftCardEntity : GameEntity
{
    public GeishaType GeishaType {  get; private set; }

    public bool right = true;

    public GiftCardEntity(GeishaType geishaType, Vector2? position = null) 
    {
        Sprite = SpritesProvider.GetGiftCardSprite(geishaType);
        Position = position ?? _hiddenPosition;
        GeishaType = geishaType;
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        Sprite.Draw(spriteBatch, Position);
    }

    public override void Update(GameTime gameTime)
    {
        var random = new Random();



        if (random.Next(0, 100) < 90)
        {
            if (Position.X > GameSettings.WINDOW_WIDTH || Position.X <= 0) right = !right;
            if (right)
            {
                Position += new Vector2(1, 0);
            }
            else
            {
                Position += new Vector2(-1, 0);
            }
        }
    }
}