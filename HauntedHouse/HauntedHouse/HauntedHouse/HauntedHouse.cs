using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Krypton;
using Krypton.Lights;
using System.Diagnostics;
using C3.XNA;

namespace HauntedHouse
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class HauntedHouse : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KryptonEngine krypton;
        KryptonEngine ambiantLight;

        //Temporary Map Test
        Level level;

        //Render offscreen test
        RenderTarget2D renderTarget;
        Texture2D shadowMap;

        private Texture2D lightTexture;

        Random random = new Random();

        Camera2D camera;

        int verticalUnits;

        Texture2D playerImage;
        Texture2D gemImage;
        Texture2D stairsImage;

        //List of all the active sprites
        List<Sprite> sprites;

        //List of all the levels in the game
        //List<Level> levels;

        //FPS
        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        //Font
        SpriteFont stdFont;

        public HauntedHouse()
        {
            // Setup the graphics device manager with some default settings
            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.PreferredBackBufferHeight = 720;
            //this.graphics.ToggleFullScreen();

            // Allow the window to be resized (to demonstrate render target recreation)
            this.Window.AllowUserResizing = true;

            // Setup the content manager with some default settings
            this.Content.RootDirectory = "Content";

            // Create Krypton
            this.krypton = new KryptonEngine(this, "KryptonEffect");
            this.ambiantLight = new KryptonEngine(this, "KryptonEffect");
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Make sure to initialize krpyton, unless it has been added to the Game's list of Components
            krypton.Initialize();
            krypton.SpriteBatchCompatablityEnabled = true;
            krypton.CullMode = CullMode.None;
            krypton.Bluriness = 3;
            krypton.AmbientColor = new Color(15, 15, 15);

            ambiantLight.Initialize();
            //ambiantLight.SpriteBatchCompatablityEnabled = true;
            //ambiantLight.CullMode = CullMode.None;
            ambiantLight.Bluriness = 0;
            ambiantLight.AmbientColor = new Color(40, 40, 40);
            //ambiantLight.AmbientColor = new Color(255, 255, 255);

            //Sprites list
            sprites = new List<Sprite>();

            //Create camera
            camera = new Camera2D(GraphicsDevice);

            //Set vertical scale
            verticalUnits = GraphicsDevice.Viewport.Height;

            // TODO: use this.Content to load your game content here
            level = Content.Load<Level>("Levels/newTest");
            level.Intialise(krypton, this.Content, this.GraphicsDevice, sprites);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            playerImage = Content.Load<Texture2D>("player");

            // Create a new simple point light texture to use for the lights
            this.lightTexture = LightTextureBuilder.CreatePointLight(this.GraphicsDevice, 1024);

            //Offscreen render
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, true, GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);

            stdFont = Content.Load<SpriteFont>("Font/stdFont");

            //for (int x = 0; x < 200; x++)
            //{
                // Sprite spam = new Sprite(playerImage, new Vector2((float)random.NextDouble() * 500 + 700, (float)random.NextDouble() * 500 + 400), true, krypton);
                // spam.Velocity = new Vector2((float)random.NextDouble() * 1.5f -1 , (float)random.NextDouble() * 1.5f -1);
                // sprites.Add(spam);
           // }
            
            /*
            // Make some random lights!
            for (int i = 0; i < 9; i++)
            {
                byte r = (byte)(this.random.Next(255 - 32) + 32);
                byte g = (byte)(this.random.Next(255 - 32) + 32);
                byte b = (byte)(this.random.Next(255 - 32) + 32);

                Light2D light = new Light2D()
                {
                    Texture = lightTexture,
                    Range = (float)(this.random.NextDouble() * 200 + 100),
                    Color = new Color(r, g, b),
                    //Intensity = (float)(this.mRandom.NextDouble() * 0.25 + 0.75),
                    Intensity = 0.8f,
                    Angle = MathHelper.TwoPi * (float)this.random.NextDouble(),
                    X = (float)(this.random.NextDouble() * 500),
                    Y = (float)(this.random.NextDouble() * 500),
                };

                // Here we set the light's field of view
                if (i % 2 == 0)
                {
                    light.Fov = MathHelper.PiOver2 * (float)(this.random.NextDouble() * 0.75 + 0.25);
                }

                this.krypton.Lights.Add(light);
            }
             */

            /*
            int x = 10;
            int y = 10;
            float w = 1000;
            float h = 1000;

            // Make lines of lines of hulls!
            for (int j = 0; j < y; j++)
            {
                // Make lines of hulls!
                for (int i = 0; i < x; i++)
                {
                    var posX = (((i + 0.5f) * w) / x) - w / 2 + (j % 2 == 0 ? w / x / 2 : 0);
                    var posY = (((j + 0.5f) * h) / y) - h / 2; // +(i % 2 == 0 ? h / y / 4 : 0);

                    var hull = ShadowHull.CreateRectangle(Vector2.One * 10f);
                    hull.Position.X = posX;
                    hull.Position.Y = posY;
                    hull.Scale.X = (float)(this.random.NextDouble() * 2.75f + 0.25f);
                    hull.Scale.Y = (float)(this.random.NextDouble() * 2.75f + 0.25f);

                    krypton.Hulls.Add(hull);
                }
            }
            */
            //Test lights //Test code - light spammer
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //For FPS
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //Follow the player
            camera.setTarget(level.Player.Position);

            //Update the camera
            camera.Update(gameTime);

            // update the matrix, per camera
            krypton.Matrix = camera.View;

            //Update the level
            level.Update(gameTime);

            //Now draw a new layer of graphics
            {
                ambiantLight.LightMapPrepare();

                GraphicsDevice.SetRenderTarget(renderTarget);

                GraphicsDevice.Clear(new Color(0, 0, 0, 0));

                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.View);

                level.PostLightDraw(spriteBatch, gameTime);

                spriteBatch.End();

                ambiantLight.Draw(gameTime);

                GraphicsDevice.SetRenderTarget(null);

                shadowMap = (Texture2D)renderTarget;
            }

            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //FPS
            frameCounter++;

            string fps = string.Format("fps: {0}", frameRate);

            // Create a world view projection matrix to use with krypton
            //Matrix world = Matrix.Identity;
            //Matrix view = camera.SimView;
            //Matrix projection = Matrix.CreateOrthographic(this.verticalUnits * this.GraphicsDevice.Viewport.AspectRatio, this.verticalUnits, 0, 1);
            // Matrix wvp = world * view * projection;

            // Assign the matrix and pre-render the lightmap.
            // Make sure not to change the position of any lights or shadow hulls after this call, as it won't take effect till the next frame!
            //this.krypton.Matrix = wvp;
            krypton.LightMapPrepare();
            ambiantLight.LightMapPrepare();

            // Make sure we clear the backbuffer *after* Krypton is done pre-rendering
            this.GraphicsDevice.Clear(new Color(0,0,0,0));

            camera.Zoom = 0.5f;

            // ----- DRAW STUFF HERE ----- //
            // By drawing here, you ensure that your scene is properly lit by krypton.
            // Drawing after KryptonEngine.Draw will cause you objects to be drawn on top of the lightmap (can be useful, fyi)
            // ----- DRAW STUFF HERE ----- //

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.View);

            level.PreLightDraw(spriteBatch, gameTime);

            spriteBatch.End();

            // Draw hulls
            //this.DebugDrawHulls(true);

            // Draw krypton (This can be omited if krypton is in the Component list. It will simply draw krypton when base.Draw is called
            this.krypton.Draw(gameTime);

            //Draw the new texture
            spriteBatch.Begin();

            spriteBatch.Draw(shadowMap, Vector2.Zero, Color.White);

            spriteBatch.DrawString(stdFont,fps, new Vector2(33, 33), Color.Black);
            spriteBatch.DrawString(stdFont, fps, new Vector2(32, 32), Color.White);

            spriteBatch.End();

          #if DEBUG
            if (Keyboard.GetState().IsKeyDown(Keys.H))
            {
                // Draw hulls
                this.DebugDrawHulls(false);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
                // Draw hulls
                this.DebugDrawLights();
            }
          #endif 

            base.Draw(gameTime);
        }

        private void DebugDrawHulls(bool drawSolid)
        {
            this.krypton.RenderHelper.Effect.CurrentTechnique = this.krypton.RenderHelper.Effect.Techniques["DebugDraw"];

            this.GraphicsDevice.RasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None,
                FillMode = drawSolid ? FillMode.Solid : FillMode.WireFrame,
            };

            // Clear the helpers vertices
            this.krypton.RenderHelper.ShadowHullVertices.Clear();
            this.krypton.RenderHelper.ShadowHullIndicies.Clear();

            foreach (var hull in krypton.Hulls)
            {
                this.krypton.RenderHelper.BufferAddShadowHull(hull);
            }

            foreach (var effectPass in krypton.RenderHelper.Effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                this.krypton.RenderHelper.BufferDraw();
            }

        }

        private void DebugDrawLights()
        {
            this.krypton.RenderHelper.Effect.CurrentTechnique = this.krypton.RenderHelper.Effect.Techniques["DebugDraw"];

            this.GraphicsDevice.RasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None,
                FillMode = FillMode.WireFrame,
            };

            // Clear the helpers vertices
            this.krypton.RenderHelper.ShadowHullVertices.Clear();
            this.krypton.RenderHelper.ShadowHullIndicies.Clear();

            foreach (Light2D light in krypton.Lights)
            {
                this.krypton.RenderHelper.BufferAddBoundOutline(light.Bounds);
            }

            foreach (var effectPass in krypton.RenderHelper.Effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                this.krypton.RenderHelper.BufferDraw();
            }

        }
    }
}