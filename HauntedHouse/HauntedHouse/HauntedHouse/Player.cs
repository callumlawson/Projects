using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HauntedHouse;
using Microsoft.Xna.Framework.Input;

namespace HauntedHouse
{
    class Player : Sprite
    {
        //The players sprite
        Sprite playerSprite;
        Animation idleAnimation;
        SpriteManager spriteManger;

        // Amount of hit points that player has
        public int Health;

        //Random number gen
        Random random;
        double sinNumber = 0d;

        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // Gamepad states used to determine button presses
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;

        // To determin mouse clicks
        MouseState currentMouseState;
        MouseState previousMouseState;

        //Player move speed
        int playerMoveSpeed = 2;

        //Torch
        public float TorchAngle = 0.0f;

        public Player()
        {

        }

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
        override public void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            sinNumber += 0.05;
            Animation.Rotation = (float)Math.Sin(sinNumber) * 0.02f;

            updateInput(gameTime);
        }

        public void updateInput(GameTime gameTime)
        {
            // Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
            previousGamePadState = currentGamePadState;
            previousKeyboardState = currentKeyboardState;
            previousMouseState = currentMouseState;

            // Read the current state of the keyboard and gamepad and store it
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            currentMouseState = Mouse.GetState();
            
            // Get Thumbstick Controls
            this.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
            this.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

            Vector2 rightStickDirection = currentGamePadState.ThumbSticks.Right;
            rightStickDirection.Normalize();
            TorchAngle = -(float)Math.Atan2(rightStickDirection.Y, rightStickDirection.X);
            //TODO make much betterer
            Vector2 mouseDirection = new Vector2(currentMouseState.X - 640, currentMouseState.Y - 480);
            mouseDirection.Normalize();
            TorchAngle = (float)Math.Atan2(mouseDirection.Y, mouseDirection.X);

            // Use the Keyboard / Dpad
            if (currentKeyboardState.IsKeyDown(Keys.Left) || currentKeyboardState.IsKeyDown(Keys.A) ||
            currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                this.Position.X -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right) || currentKeyboardState.IsKeyDown(Keys.D) ||
            currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                this.Position.X += playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W) ||
            currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                this.Position.Y -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.S) ||
            currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                this.Position.Y += playerMoveSpeed;
            }
        }

        override public void Draw(SpriteBatch spriteBatch)
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
