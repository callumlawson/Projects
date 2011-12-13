using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HauntedHouse;

namespace HauntedHouse
{
    class Player : Sprite
    {
        // Amount of hit points that player has
        public int Health;

        //Random number gen
        Random random;
        double sinNumber = 0d;

        public void Initialize(Animation animation, Vector2 position)
        {
            Animation = animation;
            Animated = true;

            base.Width = animation.FrameWidth;
            base.Height = animation.FrameHeight;
            //Stop the player animation rotating to much
            //PlayerAnimation.Rotation = MathHelper.Clamp(PlayerAnimation.Rotation, -0.4f, 0.4f);
           
            // Set the starting position of the player around the middle of the screen and to the back
            base.Position = position;

            random = new Random();

            // Set the player to be active
            Active = true;

            // Set the player health
            Health = 100;
        }

        // TODO check inheritance stuff
        new public void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Animation.Position = Position;

            sinNumber += 0.05;
            Animation.Rotation = (float) Math.Sin(sinNumber)*0.02f;

            Animation.Update(gameTime);
        }

        new public void Draw(SpriteBatch spriteBatch)
        {
            Animation.Draw(spriteBatch);
        }

        // Get the width of the player ship
        new public int Width
        {
            get { return Animation.FrameWidth; }
        }

        // Get the height of the player ship
        new public int Height
        {
            get { return Animation.FrameHeight; }
        }
    }
}
