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
    class Player
    {
        //The players sprite
        Sprite playerSprite;
        Animation idleAnimation;
        SpriteManager spriteManger;

        // Physics state
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        // Amount of hit points that player has
        public int Health;

        //Random number gen
        Random random;
        double sinNumber = 0d;

        //Input
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // Gamepad states used to determine button presses
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;

        // To determin mouse clicks
        MouseState currentMouseState;
        MouseState previousMouseState;

        //Player move speed
        int playerMoveSpeed = 5;

        //Torch
        public float TorchAngle
        {
            get { return torchAngle; }
        }
        float torchAngle = 0.0f;   
    
        public Vector2 TorchPosition
        {
            get { return position; }
        }
        Vector2 torchPostion;

        /// <summary>
        /// Is the players torch on or off?
        /// </summary>
        public bool IsTorchOn
        {
            get { return isTorchOn; }
        }
        bool isTorchOn;

        public Player(Vector2 position,Sprite sprite)
        {
            playerSprite = sprite;
            this.position = position;

            random = new Random();

            Health = 100;
        }

        /*
        public void Initialize()
        {
        }
        */

        // TODO check inheritance stuff
        public void Update(GameTime gameTime)
        {
            playerSprite.Position = this.position;
            playerSprite.Update(gameTime);

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
            position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
            position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

            Vector2 rightStickDirection = currentGamePadState.ThumbSticks.Right;
            rightStickDirection.Normalize();
            torchAngle = -(float)Math.Atan2(rightStickDirection.Y, rightStickDirection.X);
            //TODO make much betterer
            Vector2 mouseDirection = new Vector2(currentMouseState.X - 640, currentMouseState.Y - 480);
            mouseDirection.Normalize();
            torchAngle = (float)Math.Atan2(mouseDirection.Y, mouseDirection.X);

            // Use the Keyboard / Dpad
            if (currentKeyboardState.IsKeyDown(Keys.Left) || currentKeyboardState.IsKeyDown(Keys.A) ||
            currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                position.X -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right) || currentKeyboardState.IsKeyDown(Keys.D) ||
            currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                position.X += playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W) ||
            currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                position.Y -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.S) ||
            currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                position.Y += playerMoveSpeed;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            playerSprite.Draw(spriteBatch);
        }

       // Get the width of the player ship
       //new public int Width
       // {
       //     get { return Animation.FrameWidth; }
       // }

       // Get the height of the player ship
       // new public int Height
       //  {
       //     get { return Animation.FrameHeight; }
       // }
    }
}
