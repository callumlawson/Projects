using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Krypton;


namespace HauntedHouse
{
    // This is not the most efficient way to store the data or perform layer rendering. It is, however,
    // close to the simplest way to interact with TiledLib in order to get data from the Tiled editor
    // to the game using custom game types.

    public class Map
    {
        public int TileGridWidth;
        public int TileGridHeight;
        public List<ObjectLayer> ObjectLayers = new List<ObjectLayer>();
        public List<TileLayer> TileLayers = new List<TileLayer>();

        private List<Sprite> sprites;
        KryptonEngine krypton;

        public void Intialise(KryptonEngine krypton, List<Sprite> sprites)
        {
            this.krypton = krypton;
            this.sprites = sprites;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var layer in TileLayers)
            {
                for (int y = 0; y < layer.Height; y++)
                {
                    for (int x = 0; x < layer.Width; x++)
                    {
                        Tile tile = layer.Tiles[y * layer.Width + x];

                        if (tile.Exists)
                        {
                            tile.Intialise(new Vector2(x, y));
                            tile.Draw(spriteBatch);
                        }
                    }
                }
            }
        }
    }
}
