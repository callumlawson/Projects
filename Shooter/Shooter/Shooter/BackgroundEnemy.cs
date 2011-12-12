using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    class BackgroundEnemy
    {
        // Animation representing the enemy
        public Animation EnemyAnimation;

        // The position of the enemy ship relative to the top left corner of thescreen
        public Vector2 Position;

        //Angle of rotation
        public float Rotation;

        // The state of the Enemy Ship
        public bool Active;

        // Cool gravity thing
        public float DownwardsVelocity = 0;
        public float Gravity = 0.01f;
        public Boolean Exploded = false;

        // Get the width of the enemy ship
        public int Width
        {
            get { return EnemyAnimation.FrameWidth; }
        }

        // Get the height of the enemy ship
        public int Height
        {
            get { return EnemyAnimation.FrameHeight; }
        }

        // The speed at which the enemy moves
        float enemyMoveSpeed;

        //Random
        Random random;
        Boolean crashing;

        public void Initialize(Animation animation, Vector2 position)
        {
            // Load the enemy ship texture
            EnemyAnimation = animation;

            // Set the position of the enemy
            Position = position;

            // We initialize the enemy to be active so it will be update in the game
            Active = true;

            // Set how fast the enemy moves
            enemyMoveSpeed = 1.3f;

            random = new Random();
        }

        public void Update(GameTime gameTime)
        {
            // The enemy always moves to the left so decrement it's xposition
            Position.X -= enemyMoveSpeed;

            // Update the position of the Animation
            EnemyAnimation.Position = Position;

            // Update Animation
            EnemyAnimation.Update(gameTime);

            // If the enemy is past the screen or its health reaches 0 then deactivateit
            if (Position.X < -Width || Position.Y > 480)
            {
                // By setting the Active flag to false, the game will remove this object from the 
                // active game list
                Active = false;
            }

            /*
            if (random.NextDouble() > 0.9995) { crashing = true; }
            
            if(crashing)
            {
                DownwardsVelocity += Gravity;
                Position.Y += DownwardsVelocity;
                Rotation -= 0.03f;
                EnemyAnimation.Rotation = Rotation;
            }
            */
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the animation
            EnemyAnimation.Draw(spriteBatch);
        }

    }
}
