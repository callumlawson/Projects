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

        private Texture2D lightTexture;
        private Light2D light2D;

        Random random = new Random();

        Camera2D camera;

        int verticalUnits;

        Texture2D playerImage;

        //List of all the active sprites
        List<Sprite> sprites;

        //Player
        Player player;

        public HauntedHouse()
        {
            // Setup the graphics device manager with some default settings
            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.PreferredBackBufferHeight = 720;

            // Allow the window to be resized (to demonstrate render target recreation)
            this.Window.AllowUserResizing = true;

            // Setup the content manager with some default settings
            this.Content.RootDirectory = "Content";

            // Create Krypton
            this.krypton = new KryptonEngine(this, "KryptonEffect");
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
            this.krypton.Initialize();
            krypton.SpriteBatchCompatablityEnabled = true;
            krypton.CullMode = CullMode.None;
            krypton.AmbientColor = new Color(150, 150, 150);

            //Sprites list
            sprites = new List<Sprite>();

            //Create player
            player = new Player();

            //Create camera
            camera = new Camera2D(GraphicsDevice);
            
            //Set vertical scale
            verticalUnits = GraphicsDevice.Viewport.Height;

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

            // Create a new simple point light texture to use for the lights
            this.lightTexture = LightTextureBuilder.CreatePointLight(this.GraphicsDevice, 512);

            // Load sprites
            playerImage = Content.Load<Texture2D>("player");

            // Load the player resources
            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("shipAnimation");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 45, Color.White, 1f, true);
            player.Initialize(playerAnimation, Vector2.Zero);
            sprites.Add(player);

            // Create a light we can control
            this.light2D = new Light2D()
            {
                Texture = this.lightTexture,
                X = 0,
                Y = 0,
                Range = 500,
                Intensity = 0.8f,
                Color = Color.Multiply(Color.Blue, 2.0f),
                ShadowType = ShadowType.Occluded
            };

            this.krypton.Lights.Add(this.light2D);
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //Update all the sprites
            foreach (Sprite sprite in sprites)
            {
                sprite.Update(gameTime);
            }

            //Update the camera
            camera.Update(gameTime);
            camera.MoveCamera(new Vector2(1, 0));

            // update the matrix, per camera
            krypton.Matrix = camera.View;

            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Create a world view projection matrix to use with krypton
            //Matrix world = Matrix.Identity;
            //Matrix view = camera.SimView;
            //Matrix projection = Matrix.CreateOrthographic(this.verticalUnits * this.GraphicsDevice.Viewport.AspectRatio, this.verticalUnits, 0, 1);
           // Matrix wvp = world * view * projection;

            // Assign the matrix and pre-render the lightmap.
            // Make sure not to change the position of any lights or shadow hulls after this call, as it won't take effect till the next frame!
            //this.krypton.Matrix = wvp;
            this.krypton.LightMapPrepare();

            // Make sure we clear the backbuffer *after* Krypton is done pre-rendering
            this.GraphicsDevice.Clear(Color.White);

            // ----- DRAW STUFF HERE ----- //
            // By drawing here, you ensure that your scene is properly lit by krypton.
            // Drawing after KryptonEngine.Draw will cause you objects to be drawn on top of the lightmap (can be useful, fyi)
            // ----- DRAW STUFF HERE ----- //

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.View);

            foreach (Sprite sprite in sprites)
            {
                sprite.Draw(spriteBatch);
            }

            spriteBatch.Draw(playerImage, Vector2.Zero, Color.White);

            spriteBatch.End();

            // Draw hulls
            this.DebugDrawHulls(true);

            // Draw krypton (This can be omited if krypton is in the Component list. It will simply draw krypton when base.Draw is called
            this.krypton.Draw(gameTime);

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
