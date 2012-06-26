using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Krypton;
using HauntedHouse.Entities;
using Krypton.Lights;

namespace HauntedHouse
{
    // This is not the most efficient way to store the data or perform layer rendering. It is, however,
    // close to the simplest way to interact with TiledLib in order to get data from the Tiled editor
    // to the game using custom game types.

    public class Level
    {
        //Loaded on deserialisation
        public int TileGridWidth;
        public int TileGridHeight;
        public List<EntityLayer> EntityLayers = new List<EntityLayer>();
        public List<TileLayer> TileLayers = new List<TileLayer>();

        //Handles (?)
        KryptonEngine krypton;
        ContentManager content;
        Texture2D placeHolder;

        //Entities
        List<Sprite> sprites;

        public List<Platform> Platforms
        {
            get { return platforms; }
        }
        List<Platform> platforms;

        public Player Player
        {
            get { return player; }
        }
        Player player;

        //Torch
        Light2D torch;
        Light2D torchGlow;

        public void Intialise(KryptonEngine krypton, ContentManager content, GraphicsDevice graphicsDevice, List<Sprite> sprites)
        {
            this.content = content;
            this.krypton = krypton;
            this.sprites = sprites;
            placeHolder = content.Load<Texture2D>("PlaceHolder");
            platforms = new List<Platform>();

            //Texture2D playerImage = content.Load<Texture2D>("playerDraft");
            //Sprite testSprite = new Sprite(playerImage, new Vector2(0,0), false, krypton);
            //player = new Player(new Vector2(100, 100), testSprite);

            foreach (var layer in TileLayers)
            {
                for (int y = 0; y < layer.Height; y++)
                {
                    for (int x = 0; x < layer.Width; x++)
                    {
                        Tile tile = layer.Tiles[y * layer.Width + x];
                        if (tile.Exists)
                        {
                            tile.Intialise(new Vector2(x, y), content);
                        }
                    }
                }
            }

            foreach (var layer in EntityLayers)
            {
                foreach (Entity entity in layer.Entities)
                {
                    entity.Intialise(platforms, sprites, krypton, content, graphicsDevice, this);
                }
            }

            // Create a light we can control
            torch = new Light2D()
            {
                Texture = LightTextureBuilder.CreatePointLight(graphicsDevice, 1024),
                X = 0,
                Y = 0,
                Range = 800,
                Intensity = 0.6f,
                Color = Color.White,
                ShadowType = ShadowType.Illuminated,
                Fov = MathHelper.PiOver2 * (float)(0.3)
            };
            krypton.Lights.Add(torch);

            torchGlow = new Light2D()
            {
                Texture = LightTextureBuilder.CreatePointLight(graphicsDevice, 1024),
                X = 0,
                Y = 0,
                Range = 700,
                Intensity = 0.25f,
                Color = Color.White,
                ShadowType = ShadowType.Solid,
                //Fov = MathHelper.PiOver2 * (float)(0.5)
            };
            krypton.Lights.Add(torchGlow);
        }

        public void setPlayer(Player player)
        {
            this.player = player;
        }

        public void Update(GameTime gameTime)
        {
            torch.Position = player.Position + new Vector2(57, 60);
            torchGlow.Position = player.Position + new Vector2(57, 50);
            torch.Angle = player.TorchAngle;
            player.Update(gameTime);

            //Update all the sprites
            foreach (Sprite sprite in sprites)
            {
                sprite.Update(gameTime);
            }
        }

        public void PreLightDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Draw the layers in order
            foreach (var layer in TileLayers)
            {
                if (layer.LayerName == "Background")
                {
                    drawTileLayer(layer, spriteBatch);
                }
            }

            foreach (var layer in TileLayers)
            {
                if (layer.LayerName == "Midground")
                {
                    drawTileLayer(layer, spriteBatch);
                }
            }

            player.Draw(spriteBatch);

            foreach (var layer in EntityLayers)
            {
                foreach (Entity entity in layer.Entities)
                {

                    //For debugging
                    #if DEBUG
                      spriteBatch.Draw(placeHolder,entity.EntityBounds,Color.WhiteSmoke);
                    #endif
                }
            }


        }

        public void PostLightDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var layer in TileLayers)
            {
                if (layer.LayerName == "Forground")
                {
                    drawTileLayer(layer, spriteBatch);
                }
            }
        }

        public void drawTileLayer(TileLayer layer, SpriteBatch spriteBatch)
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
    }
}