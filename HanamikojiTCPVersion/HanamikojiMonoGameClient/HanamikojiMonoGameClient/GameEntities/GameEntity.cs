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
    public bool IsEnlarged { get; private set; } 

    public int Width => Sprite.Width;
    public int Height => Sprite.Height;

    protected static Vector2 _hiddenPosition = new Vector2(GameSettings.WINDOW_WIDTH, GameSettings.WINDOW_HEIGHT / 2.0f);
    public bool IsHidden => Position == _hiddenPosition;

    public abstract void Update(GameTime gameTime);

    public void MoveToPosition(Vector2 newPosition) => Position = newPosition;

    public void MoveInX(int offset) => Position = new Vector2(Position.X + offset, Position.Y);

    public void MoveInY(int offset) => Position = new Vector2(Position.X, Position.Y + offset);

    public void SetRotation(float rotation) => Rotation = rotation;

    public void Hide() => Position = _hiddenPosition;

    public void Enlarge(bool isEnlarged = true) => IsEnlarged = isEnlarged;

    // todo: unit tests?
    public bool IsPointInsideSprite(Vector2 point) => (point.X >= Position.X && point.X <= (Position.X + Width)) &&
                                                      (point.Y >= Position.Y && point.Y <= (Position.Y + Height));

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        Sprite.Draw(spriteBatch, Position, Rotation,  IsEnlarged ? GameSettings.ENLARGED_MOVE_SCALE : 1);
    }
}