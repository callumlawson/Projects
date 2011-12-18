using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Krypton;
using HauntedHouse.Entities;

namespace HauntedHouse
{
    // This is not the most efficient way to store the data or perform layer rendering. It is, however,
    // close to the simplest way to interact with TiledLib in order to get data from the Tiled editor
    // to the game using custom game types.

    public class Map
    {
        //Loaded on deserialisation
        public int TileGridWidth;
        public int TileGridHeight;
        public List<EntityLayer> EntityLayers = new List<EntityLayer>();
        public List<TileLayer> TileLayers = new List<TileLayer>();

        //Private variables
        private List<Sprite> sprites;
        private List<Platform> platforms;
        KryptonEngine krypton;
        ContentManager content;
        Texture2D placeHolder;

        public void Intialise(KryptonEngine krypton, List<Sprite> sprites, ContentManager content,GraphicsDevice graphicsDevice)
        {
            this.content = content;
            this.krypton = krypton;
            this.sprites = sprites;
            placeHolder = content.Load<Texture2D>("PlaceHolder");
            platforms = new List<Platform>();

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
                        }
                    }
                }
            }

            foreach (var layer in EntityLayers)
            {
                foreach (Entity entity in layer.Entities)
                {
                    entity.Intialise(platforms, krypton, content,graphicsDevice);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var layer in TileLayers)
            {
                for (int y = 0; y < layer.Height; y++)
                {
                    for (int x = 0; x < layer.Width; x++)
                    {
                        if (layer.Tiles[y * layer.Width + x] != null)
                        {
                            Tile tile = layer.Tiles[y * layer.Width + x];
                            if (tile.Exists)
                            {
                                tile.Draw(spriteBatch);
                            }
                        }
                    }
                }
            }

            foreach (var layer in EntityLayers)
            {
                foreach(Entity entity in layer.Entities)
                {
                    //For debugging
                    //spriteBatch.Draw(placeHolder,entity.EntityBounds,Color.WhiteSmoke);
                }
            }
        }
    }
}
