using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HanamikojiMonoGameClient.Sprites
{ 
    public class Sprite
    {
        public Texture2D Texture { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Color TintColor { get; set; } = Color.White;

        public Sprite(Texture2D texture2D, int x, int y, int width, int height)
        {
            Texture = texture2D;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation, float scale)
        {
            var positionWithCenterRotationAlignment = new Vector2(position.X + Width/2, position.Y + Height/2);
            spriteBatch.Draw(
                Texture, 
                positionWithCenterRotationAlignment, 
                new Rectangle(X, Y, Width, Height), 
                TintColor, 
                rotation, 
                new Vector2(Width/2,Height/2), 
                scale, 
                SpriteEffects.None, 
                0f);
        }
    }
}
