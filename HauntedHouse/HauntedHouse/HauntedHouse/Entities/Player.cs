using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HauntedHouse;
using Microsoft.Xna.Framework.Input;
using HauntedHouse.Entities;
using C3.XNA;

namespace HauntedHouse
{
    public class Player
    {
        //The players sprite
        Sprite playerSprite;
        Animation idleAnimation;

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
        KeyboardState keyboardState;
        // Gamepad states used to determine button presses
        GamePadState gamePadState;
        // To determin mouse clicks
        MouseState mouseState;

        //Player move speed
        int playerMoveSpeed = 5;

        Vector2 Spawn;

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

        //Level - the current game level
        Level level;

        //Collisions
        //Bounds
        Rectangle playerBounds;
        List<Platform> platforms;
        private float previousBottom;
        bool wasCollision; //Was there a collision this frame?

        /// <summary>
        /// Gets whether or not the player's feet are on the ground.
        /// </summary>
        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        bool isOnGround;

        /// <summary>
        /// Is the players torch on or off?
        /// </summary>
        public bool IsTorchOn
        {
            get { return isTorchOn; }
        }
        bool isTorchOn = true;

        //Physics Constants
        // Constants for controling horizontal movement
        private const float MoveAcceleration = 40000.0f;
        private const float MaxMoveSpeed = 10000.0f;
        private const float GroundDragFactor = 0.38f;
        private const float AirDragFactor = 0.48f;

        // Constants for controlling vertical movement
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -1800.0f;
        private const float GravityAcceleration = 3000.0f;
        private const float MaxFallSpeed = 1200.0f;
        private const float JumpControlPower = 0.50f;

        // Input configuration
        private const float MoveStickScale = 1.0f;
        private const Buttons JumpButton = Buttons.A;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

        // Jumping state
        private bool isJumping;
        private bool wasJumping;
        private float jumpTime;

        /// <summary>
        /// Current user movement input.
        /// </summary>
        private float movement;

        public Player(Vector2 position,Sprite sprite,Level level)
        {
            playerSprite = sprite;
            this.position = position;
            playerSprite.Position = this.position;
            Spawn = position;
            this.level = level;
            this.platforms = level.Platforms;
            random = new Random();
            Health = 100;
        }

        public void Update(GameTime gameTime)
        {
            //Find the bounds
            playerSprite.Position = this.position;
            playerBounds.X = (int)Math.Floor((double)Position.X + 24);
            playerBounds.Y = (int)Math.Floor((double)Position.Y);
            playerBounds.Width = playerSprite.Width - 48;
            playerBounds.Height = playerSprite.Height;

            playerSprite.Update(gameTime);

            // If the player is now colliding with the level, separate them.
            UpdateInput(gameTime);

            ApplyPhysics(gameTime); //Handle collisions called by apply physics
            
            // Clear input.
            movement = 0.0f;
            isJumping = false;
            //Update the sprite
        }

        public void UpdateInput(GameTime gameTime)
        {
            // Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
           // previousGamePadState = gamePadState;
           // previousKeyboardState = keyboardState;
          //  previousMouseState = mouseState;

            Vector2 rightStickDirection = gamePadState.ThumbSticks.Right;
            rightStickDirection.Normalize();
            torchAngle = -(float)Math.Atan2(rightStickDirection.Y, rightStickDirection.X);

            // Read the current state of the keyboard and gamepad and store it
            keyboardState = Keyboard.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);
            mouseState = Mouse.GetState();

            // Get analog horizontal movement.
            movement = gamePadState.ThumbSticks.Left.X * MoveStickScale;

            // Ignore small movements to prevent running in place.
            if (Math.Abs(movement) < 0.5f)
                movement = 0.0f;

            // If any digital horizontal movement input is found, override the analog movement.
            if (gamePadState.IsButtonDown(Buttons.DPadLeft) ||
                keyboardState.IsKeyDown(Keys.Left) ||
                keyboardState.IsKeyDown(Keys.A))
            {
                movement = -1.0f;
            }
            else if (gamePadState.IsButtonDown(Buttons.DPadRight) ||
                     keyboardState.IsKeyDown(Keys.Right) ||
                     keyboardState.IsKeyDown(Keys.D))
            {
                movement = 1.0f;
            }

            // Check if the player wants to jump.
            isJumping =
                gamePadState.IsButtonDown(JumpButton) ||
                keyboardState.IsKeyDown(Keys.Space) ||
                keyboardState.IsKeyDown(Keys.Up) ||
                keyboardState.IsKeyDown(Keys.W);

            //TODO make much betterer
            Vector2 mouseDirection = new Vector2(mouseState.X - 640, mouseState.Y - 480);
            mouseDirection.Normalize();
            torchAngle = (float)Math.Atan2(mouseDirection.Y, mouseDirection.X);  
        }

        /// <summary>
        /// Calculates the Y velocity accounting for jumping and
        /// animates accordingly.
        /// </summary>
        /// <remarks>
        /// During the accent of a jump, the Y velocity is completely
        /// overridden by a power curve. During the decent, gravity takes
        /// over. The jump velocity is controlled by the jumpTime field
        /// which measures time into the accent of the current jump.
        /// </remarks>
        /// <param name="velocityY">
        /// The player's current velocity along the Y axis.
        /// </param>
        /// <returns>
        /// A new Y velocity if beginning or continuing a jump.
        /// Otherwise, the existing Y velocity.
        /// </returns>
        private float DoJump(float velocityY, GameTime gameTime)
        {
            // If the player wants to jump
            if (isJumping)
            {
                // Begin or continue a jump
                if ((!wasJumping && IsOnGround) || jumpTime > 0.0f)
                {
                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //sprite.PlayAnimation(jumpAnimation);
                }

                // If we are in the ascent of the jump
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump
                    jumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                jumpTime = 0.0f;
            }
            wasJumping = isJumping;

            return velocityY;
        }

        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            velocity.X += movement * MoveAcceleration * elapsed;
            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

            velocity.Y = DoJump(velocity.Y, gameTime);

            // Apply pseudo-drag horizontally.
            if (IsOnGround)
                velocity.X *= GroundDragFactor;
            else
                velocity.X *= AirDragFactor;

            // Prevent the player from running faster than his top speed.            
            velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            // Apply velocity.
            Position += velocity * elapsed;
         

            // If the player is now colliding with the level, separate them.
            HandleCollisions();

            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            // If the collision stopped us from moving, reset the velocity to zero.
           // if (Position.X == previousPosition.X)
            //    velocity.X = 0;

           // if (Position.Y == previousPosition.Y)
            //    velocity.Y = 0;
        }

        private void HandleCollisions()
        {
            Rectangle bounds = playerBounds;
            // For each potentially colliding platform

            wasCollision = false;
            isOnGround = false;

                for (int platformIndex = 0; platformIndex < platforms.Count; platformIndex++)
                {
                    
                    Platform platform = platforms[platformIndex];
                    Rectangle platformBounds = platform.Bounds;

                    // If this tile is collidable,
                    TileCollision collision = platform.Collision;
                    if (collision != TileCollision.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = platformBounds;
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if ((absDepthY < absDepthX))
                            {
                                isOnGround = true;

                                // If we crossed the top of a tile, we are on the ground.
                                if (previousBottom <= tileBounds.Top)
                                    isOnGround = true;


                                //Resolve the collision along the Y axis.
                                Position = new Vector2(Position.X, Position.Y + depth.Y);
                                velocity.Y = 0;
                                wasCollision = true;

                                // Perform further collisions with the new bounds.
                                //bounds = playerBounds;
                            }
                            else if ((absDepthY >= absDepthX))
                            {
                                isOnGround = true;
                                //Resolve the collision along the Y axis.
                                Position = new Vector2(Position.X + depth.X, Position.Y);
                                velocity.X = 0;
 
                                wasCollision = true;
                            }
         
                        }
                    }
                }
                // Save the new bounds bottom.
               // previousBottom = bounds.Bottom;
           }
      

        public void Draw(SpriteBatch spriteBatch)
        {
            if (velocity.X < 0)
            {
                playerSprite.Flip = SpriteEffects.FlipHorizontally;
            }
            else { playerSprite.Flip = SpriteEffects.None; }

            #if DEBUG
            spriteBatch.DrawRectangle(playerBounds,Color.Red);
            #endif

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
