﻿// Animation.cs
//Using declarations
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HauntedHouse
{
    public class Animation
    {
        // The image representing the collection of images used for animation
        Texture2D spriteStrip;
        Texture2D firstFrame;

        // Render the first frame of the animation so it can be used to create a shadow hull for the sprite
        RenderTarget2D renderTarget;
        Texture2D firstFrameRender;

        //Graphics device
        GraphicsDevice device;

        // The scale used to display the sprite strip
        float scale;

        // Angle
        public float Rotation;

        // Fading and Alpha
        public Boolean LoopFade;
        public Boolean Fade;
        public int AlphaValue;
        public int FadeIncrement;
        public float FadeDelay;
        private float currentFadeDelay;

        // The time since we last updated the frame
        int elapsedTime;

        // The time we display a frame until the next one
        int frameTime;

        // The number of frames that the animation contains
        int frameCount;

        // The index of the current frame we are displaying
        public int currentFrame;

        // The color of the frame we will be displaying
        public Color Color;

        // The area of the image strip we want to display
        Rectangle sourceRect = new Rectangle();

        // The area where we want to display the image strip in the game
        public Rectangle destinationRect = new Rectangle();

        // Width of a given frame
        public int FrameWidth;

        // Height of a given frame
        public int FrameHeight;

        // The state of the Animation
        public bool Active;

        // Determines if the animation will keep playing or deactivate after one run
        public bool Looping;

        // Width of a given frame
        public Vector2 Position;

        public SpriteEffects spriteEffects = SpriteEffects.None;

        public Animation(Texture2D texture, Vector2 position,
                                int frameWidth, int frameHeight, int frameCount,
                                int frametime, float scale, bool looping, GraphicsDevice device)
        {
            // Keep a local copy of the values passed in
            //this.Color = color;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.frameCount = frameCount;
            this.frameTime = frametime;
            this.scale = scale;

            Looping = looping;
            Position = position;
            spriteStrip = texture;

            // Fading and Alpha init
            //LoopFade = true;
            //Fade = false;
            //AlphaValue = 255;
            //FadeIncrement = 1;
           // FadeDelay = 0.5f;
           // currentFadeDelay = FadeDelay;

            // Set the time to zero
            elapsedTime = 0;
            currentFrame = 0;

            // Set the Animation to active by default
            Active = true;

            //Render the first frame as a texture
        }

        public void renderFirstFrame()
        {
            //Offscreen Render
            PresentationParameters pp = new PresentationParameters();
            renderTarget = new RenderTarget2D(device, FrameWidth, FrameWidth, true, device.DisplayMode.Format, DepthFormat.Depth24);
        }

        public void Update(GameTime gameTime, Vector2 position)
        {
            // Do not update the game if we are not active
            if (Active == false)
                return;

            this.Position = position;

            // Update the elapsed time
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            // If the elapsed time is larger than the frame time
            // we need to switch frames
            if (elapsedTime > frameTime)
            {
                // Move to the next frame
                currentFrame++;

                // If the currentFrame is equal to frameCount reset currentFrame to zero
                if (currentFrame == frameCount)
                {
                    currentFrame = 0;
                    // If we are not looping deactivate the animation
                    if (Looping == false)
                        Active = false;
                }

                // Reset the elapsed time to zero
                elapsedTime = 0;
            }


            //Fading
            /*
            if (Fade)
            {
                currentFadeDelay -= elapsedTime;
                if (currentFadeDelay < 0)
                {
                    currentFadeDelay = FadeDelay;
                    AlphaValue += FadeIncrement;

                    if ((AlphaValue >= 255 || AlphaValue <= 0) && LoopFade)
                    {
                        FadeIncrement *= -1;
                    }
                }
                AlphaValue = (byte)MathHelper.Clamp(AlphaValue, 0, 255);
                Color.A = (byte)AlphaValue;
            }
             */

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
            sourceRect = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);

            destinationRect = new Rectangle((int)Position.X ,(int)Position.Y ,(int)(FrameWidth * scale),(int)(FrameHeight * scale));

            //TODO decide on centering
            destinationRect = new Rectangle((int)Position.X - (int)(FrameWidth * scale) / 2,(int)Position.Y - (int)(FrameHeight * scale) / 2,
            (int)(FrameWidth * scale),
            (int)(FrameHeight * scale));
        }

        // Draw the Animation Strip
        public void Draw(SpriteBatch spriteBatch)
        {
            
            // Only draw the animation when we are active
            if (Active)
            {
               // Vector2 origin = new Vector2(destinationRect.Center.X,destinationRect.Center.Y)
                //Vector2 origin = new Vector2(0,0);
                spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, Color.White, Rotation, new Vector2(-destinationRect.Width/2,-destinationRect.Height/2) , spriteEffects, 0f);
            }
        }

    }
}
