using System;
using CommonResources.Game;
using HanamikojiMonoGameClient.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HanamikojiMonoGameClient.GameEntities;

public class GiftCardEntity : GameEntity
{
    public Guid CardId { get; init; }
    public GeishaType GeishaType {  get; init; }

    public GiftCardEntity(GeishaType geishaType, Guid cardId, Vector2? position = null) 
    {
        CardId = cardId;
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
    }
}