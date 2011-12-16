using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace HauntedHouse
{
    // This is not the most efficient way to store the data or perform layer rendering. It is, however,
    // close to the simplest way to interact with TiledLib in order to get data from the Tiled editor
    // to the game using custom game types.

    public class Tile
    {
        public Texture2D Texture;
        public Rectangle SourceRectangle;
        public SpriteEffects SpriteEffects;
        public bool IsShadowCaster;
        public bool Drawable;
    }

    public class Layer
    {
        public int Width;
        public int Height;
        public Tile[] Tiles;
    }

    public class Map
    {
        public int TileWidth;
        public int TileHeight;
        public List<Layer> Layers = new List<Layer>();

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var layer in Layers)
            {
           
                for (int y = 0; y < layer.Height; y++)
                {
                    for (int x = 0; x < layer.Width; x++)
                    {
                        Tile t = layer.Tiles[y * layer.Width + x];
                        if (t.Drawable)
                        {
                            spriteBatch.Draw(t.Texture, new Rectangle(x * TileWidth, y * TileHeight, TileWidth, TileHeight), t.SourceRectangle, Color.White, 0, Vector2.Zero, t.SpriteEffects, 0);
                        }
                    }
                }
            }
        }
    }


}
