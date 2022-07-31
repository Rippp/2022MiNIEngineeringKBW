using HanamikojiMonoGameClient.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HanamikojiMonoGameClient.GameEntities;

public abstract class GameEntity
{
    public Sprite Sprite { get; protected set; }
    public int DrawOrder { get; protected set; }
    public Vector2 Position { get; protected set; }
    public float Rotation { get; protected set; }

    public int Width => Sprite.Width;
    public int Height => Sprite.Width;

    protected static Vector2 _hiddenPosition = new Vector2(GameSettings.WINDOW_WIDTH, GameSettings.WINDOW_HEIGHT / 2);
    public bool IsHidden => Position == _hiddenPosition;

    public abstract void Update(GameTime gameTime);

    public void MoveToPosition(Vector2 newPosition) => Position = newPosition;

    public void SetRotation(float rotation) => Rotation = rotation;

    public void Hide() => Position = _hiddenPosition;


    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        Sprite.Draw(spriteBatch, Position, Rotation);
    }
}