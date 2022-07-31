using CommonResources.Game;
using HanamikojiMonoGameClient.Managers;
using HanamikojiMonoGameClient.Sprites;
using Microsoft.Xna.Framework;

namespace HanamikojiMonoGameClient.GameEntities;

public class Button : GameEntity
{
    public Button()
    {
        Sprite = SpritesProvider.GetGeishaSprite(GeishaType.Geisha2_A);
        Position = new Vector2(GameSettings.WINDOW_WIDTH - SpritesProvider.GeishaSize, 0);
    }

    public override void Update(GameTime gameTime)
    {
    }
}