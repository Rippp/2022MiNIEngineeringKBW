using HanamikojiMonoGameClient.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HanamikojiMonoGameClient.GameEntities;

public abstract class GameEntity
{
    public Sprite Sprite { get; protected set; }
    public int DrawOrder { get; protected set; }
    public Vector2 Position { get; protected set; }

    public int Width => Sprite.Width;
    public int Height => Sprite.Width;

    protected static Vector2 _hiddenPosition = new Vector2(10000, 10000);

    public void MoveToPosition(Vector2 newPosition) => Position = newPosition;
    public void Hide() => Position = _hiddenPosition;
    public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    public abstract void Update(GameTime gameTime);
}