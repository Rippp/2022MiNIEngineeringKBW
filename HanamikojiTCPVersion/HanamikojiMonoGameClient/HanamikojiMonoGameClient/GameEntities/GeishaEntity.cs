using CommonResources.Game;
using HanamikojiMonoGameClient.Managers;
using HanamikojiMonoGameClient.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HanamikojiMonoGameClient.GameEntities;

public class GeishaEntity : GameEntity
{
    public GeishaType GeishaType { get; set; }

    public GeishaEntity(GeishaType geishaType)
    {
        Sprite = SpritesProvider.GetGeishaSprite(geishaType);
        Position = EntitiesPositions.geishaPositions[geishaType];
        GeishaType = geishaType;
        DrawOrder = 0;
    }

    public override void Update(GameTime gameTime)
    {
    }
}