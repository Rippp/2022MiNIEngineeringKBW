using System;
using CommonResources.Game;
using HanamikojiMonoGameClient.Providers;
using HanamikojiMonoGameClient.Sprites;
using Microsoft.Xna.Framework;

namespace HanamikojiMonoGameClient.GameEntities;

public class GiftCardEntity : GameEntity
{
    public const int DefaultDrawOrder = 50;
    public Guid CardId { get; init; }
    public GeishaType GeishaType {  get; private set; }

    public GiftCardEntity(GeishaType geishaType, Guid cardId, Vector2? position = null, int drawOrder = DefaultDrawOrder) 
    {
        CardId = cardId;
        Sprite = SpritesProvider.GetGiftCardSprite(geishaType);
        Position = position ?? new Vector2(_hiddenPosition.X, _hiddenPosition.Y);
        GeishaType = geishaType;
        DrawOrder = drawOrder;
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

    public override void Update(GameTime gameTime)
    {
    }
}