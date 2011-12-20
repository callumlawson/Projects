﻿using System.Collections.Generic;
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


        public void Intialise(KryptonEngine krypton,ContentManager content,GraphicsDevice graphicsDevice, List<Sprite> sprites)
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
                            tile.Intialise(new Vector2(x, y));
                        }
                    }
                }
            }

            foreach (var layer in EntityLayers)
            {
                foreach (Entity entity in layer.Entities)
                {
                    entity.Intialise(platforms, sprites, player, krypton, content, graphicsDevice, this);
                }
            }

            // Create a light we can control
            torch = new Light2D()
            {
                Texture = LightTextureBuilder.CreatePointLight(graphicsDevice, 1024),
                X = 0,
                Y = 0,
                Range = 600,
                Intensity = 0.5f,
                Color = Color.White,
                ShadowType = ShadowType.Illuminated,
                Fov = MathHelper.PiOver2 * (float)(0.5)
            };
            krypton.Lights.Add(torch);
        }

        public void setPlayer(Player player)
        {
            this.player = player;
        }

        public void Update(GameTime gameTime)
        {
                //Update all the sprites
                foreach (Sprite sprite in sprites)
                {
                    sprite.Update(gameTime);
                }

                    player.Update(gameTime);
                    torch.Position = player.Position + new Vector2(57, 60);
                    torch.Angle = player.TorchAngle;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawTiles(spriteBatch);
            player.Draw(spriteBatch);

            foreach (var layer in EntityLayers)
            {
                foreach(Entity entity in layer.Entities)
                {
                    //For debugging
                    //spriteBatch.Draw(placeHolder,entity.EntityBounds,Color.WhiteSmoke);
                }
            }
        }


        private void DrawTiles(SpriteBatch spriteBatch)
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

        }

    }
}