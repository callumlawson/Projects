using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    class Player
    {
        // Animation representing the player
        public Animation PlayerAnimation;

        // Position of the Player relative to the upper left side of the screen
        public Vector2 Position;

        // State of the player
        public bool Active;

        // Amount of hit points that player has
        public int Health;

        //Random number gen
        Random random;
        double sinNumber = 0d;

        public void Initialize(Animation animation, Vector2 position)
        {
            PlayerAnimation = animation;

            //Stop the player animation rotating to much
            //PlayerAnimation.Rotation = MathHelper.Clamp(PlayerAnimation.Rotation, -0.4f, 0.4f);
           
            // Set the starting position of the player around the middle of the screen and to the back
            Position = position;

            random = new Random();

            // Set the player to be active
            Active = true;

            // Set the player health
            Health = 100;
        }

        public void Update(GameTime gameTime)
        {
            PlayerAnimation.Position = Position;

            sinNumber += 0.05;
            PlayerAnimation.Rotation = (float) Math.Sin(sinNumber)*0.02f;

            PlayerAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerAnimation.Draw(spriteBatch);
        }

        // Get the width of the player ship
        public int Width
        {
            get { return PlayerAnimation.FrameWidth; }
        }

        // Get the height of the player ship
        public int Height
        {
            get { return PlayerAnimation.FrameHeight; }
        }
    }
}
