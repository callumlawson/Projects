using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Krypton;

namespace HauntedHouse
{
        public class Tile
        {
            public String TileSource;
            Texture2D Texture;
            public Rectangle SourceRectangle;
            public SpriteEffects SpriteEffects;
            public bool IsShadowCaster;
            public bool Exists;
            public Vector2 GridPosition;
            public Vector2 GridSize;

            private List<Sprite> sprites;
            Sprite tileSprite;

            public void Intialise(Vector2 gridPosition, ContentManager content)
            {
                GridPosition = gridPosition;
                TileSource = TileSource.Remove(TileSource.Length - 4); //Remove the .png extension
                Texture = content.Load<Texture2D>("TileSets/" + TileSource);

                //this.sprites = sprites;
                //tileSprite = new Sprite(Texture, Position, false, krypton);
               // tileSprite.sourceRectangle = SourceRectangle;
               // sprites.Add(tileSprite);
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(
                             Texture,
                             new Rectangle((int)(GridSize.X * GridPosition.X), (int)(GridSize.Y * GridPosition.Y), (int)SourceRectangle.Width, (int)SourceRectangle.Height),
                             SourceRectangle,
                             Color.White,
                             0,
                             Vector2.Zero,
                             SpriteEffects,
                             0);
            }
        }
}
