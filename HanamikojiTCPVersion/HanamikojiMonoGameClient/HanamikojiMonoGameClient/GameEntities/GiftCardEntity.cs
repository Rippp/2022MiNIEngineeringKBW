using System;
using CommonResources.Game;
using HanamikojiMonoGameClient.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HanamikojiMonoGameClient.GameEntities;

public class GiftCardEntity : GameEntity
{
    public Guid CardId { get; init; }
    public GeishaType GeishaType {  get; private set; }

    public GiftCardEntity(GeishaType geishaType, Guid cardId, Vector2? position = null) 
    {
        CardId = cardId;
        Sprite = SpritesProvider.GetGiftCardSprite(geishaType);
        Position = position ?? new Vector2(_hiddenPosition.X, _hiddenPosition.Y);
        GeishaType = geishaType;
    }

    public void RevealCard(GeishaType revealedGeishaType) 
    {
        if (GeishaType != GeishaType.AnonymizedGeisha)
        {
            throw new Exception("Can not reveal not anonymized card");
        }

        GeishaType = revealedGeishaType;
        Sprite = SpritesProvider.GetGiftCardSprite(revealedGeishaType);
    }

    public bool IsPointInsideLeftHalfOfSprite(Vector2 point) => (point.X >= Position.X && point.X <= (Position.X + Width / 2.0)) &&
                                                      (point.Y >= Position.Y && point.Y <= (Position.Y + Height));

    public override void Update(GameTime gameTime)
    {
    }
}