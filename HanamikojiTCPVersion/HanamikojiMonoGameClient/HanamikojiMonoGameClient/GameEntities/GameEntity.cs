using HanamikojiMonoGameClient.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HanamikojiMonoGameClient.GameEntities;

public abstract class GameEntity
{
    public Sprite Sprite { get; protected set; }
    public int DrawOrder { get; protected set; }
    public Vector2 Position { get; protected set; }

    public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    public abstract void Update(GameTime gameTime);
}