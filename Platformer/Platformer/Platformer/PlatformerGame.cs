#region File Description
//-----------------------------------------------------------------------------
// PlatformerGame.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input.Touch;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using System.Collections.Generic;
using FarseerPhysics.SamplesFramework;
//Krypton
using Krypton;
using Krypton.Lights;



namespace Platformer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PlatformerGame : Microsoft.Xna.Framework.Game
    {
        // Resources for drawing.
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // Global content.
        private SpriteFont hudFont;

        private Texture2D winOverlay;
        private Texture2D loseOverlay;
        private Texture2D diedOverlay;
        private Texture2D exitSign;

        // Meta-level game state.
        private int levelIndex = -1;
        private Level level;
        private bool wasContinuePressed;

        // When the time remaining is less than the warning time, it blinks on the hud
        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);

        // We store our input states so that we only poll once per frame, 
        // then we use the same input state wherever needed
        private GamePadState gamePadState;
        private KeyboardState keyboardState;
        private TouchCollection touchState;
        private AccelerometerState accelerometerState;
        
        // The number of levels in the Levels directory of our content. We assume that
        // levels in our content are 0-based and that all numbers under this constant
        // have a level file present. This allows us to not need to check for the file
        // or handle exceptions, both of which can add unnecessary time to level loading.
        private const int numberOfLevels = 3;

        //Farseer
        protected World World;
        private Texture2D _polygonTexture;

        //Krypton
        private Texture2D mLightTexture;
        KryptonEngine krypton;

        private float mVerticalUnits = 30;

        Camera2D camera2D;

        Random random = new Random();

        List<Vertices> list;
        Vertices textureVertices;

        private Light2D mLight2D;

        List<Sprite> sprites;

        public PlatformerGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Create Krypton
            this.krypton = new KryptonEngine(this, "KryptonEffect");
            krypton.AmbientColor = new Color(130, 130, 150);

            Accelerometer.Initialize();
        }

        protected override void Initialize()
        {
            // Make sure to initialize krpyton, unless it has been added to the Game's list of Components
            this.krypton.Initialize();
            sprites = new List<Sprite>();
            camera2D = new Camera2D(GraphicsDevice);

            //Vector2 amount = new Vector2(-GraphicsDevice.Viewport.Width/2,-GraphicsDevice.Viewport.Height/2);
            Vector2 amount = new Vector2(-1, -3);

            camera2D.Position = Vector2.Zero;
            
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

            // Load fonts
            hudFont = Content.Load<SpriteFont>("Fonts/Hud");

            // Load overlay textures
            winOverlay = Content.Load<Texture2D>("Overlays/you_win");
            loseOverlay = Content.Load<Texture2D>("Overlays/you_lose");
            diedOverlay = Content.Load<Texture2D>("Overlays/you_died");

            //Known issue that you get exceptions if you use Media PLayer while connected to your PC
            //See http://social.msdn.microsoft.com/Forums/en/windowsphone7series/thread/c8a243d2-d360-46b1-96bd-62b1ef268c66
            //Which means its impossible to test this from VS.
            //So we have to catch the exception and throw it away
            try
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(Content.Load<Song>("Sounds/Music"));
            }
            catch { }

            if (World == null)
            {
                World = new World(Vector2.Zero);
            }
            else
            {
                World.Clear();
            }


            loadConvexHulls();

            // Create a new simple point light texture to use for the lights
            this.mLightTexture = LightTextureBuilder.CreatePointLight(this.GraphicsDevice, 200);
            // Create some lights and hulls
            CreateLights(mLightTexture);
            CreateShadowHulls();

            exitSign = Content.Load<Texture2D>("Tiles/exit");

            Sprite sprite = new Sprite(winOverlay, new Vector2(20,20), krypton);
            Sprite sprite2 = new Sprite(exitSign, new Vector2(0, 0), krypton);
            sprites.Add(sprite);
            sprites.Add(sprite2);

            LoadNextLevel();
        }

        public void CreateLights(Texture2D texture)
        {
            // Make some random lights!
            for (int i = 0; i < 1; i++)
            {
                Light2D light = new Light2D()
                {
                    Texture = texture,
                    Range = (float)(50),
                    Color = new Color(255, 255, 255),
                    //Intensity = (float)(this.mRandom.NextDouble() * 0.25 + 0.75),
                    Intensity = 0.8f,
                    Angle = MathHelper.TwoPi * (float)0,
                    X = (float)(10),
                    Y = (float)(-10),
                };

                // Here we set the light's field of view
                if (i % 2 == 0)
                {
                    light.Fov = MathHelper.PiOver2 * (float)(1);
                }

                light.ShadowType = ShadowType.Illuminated;

                this.krypton.Lights.Add(light);
            }
        }

        public void CreateShadowHulls()
        {
            var hull2 = ShadowHull.CreateRectangle(new Vector2(7,7));
            hull2.Position.X = 30;
            hull2.Position.Y = -20;
            hull2.Scale.X = (float)(1);
            hull2.Scale.Y = (float)(1);
            krypton.Hulls.Add(hull2);
        }

        public void loadConvexHulls()
        {
            //World.Gravity = Vector2.Zero;
            //load texture that will represent the physics body
            _polygonTexture = Content.Load<Texture2D>("Tiles/Exit");

            //Create an array to hold the data from the texture
            uint[] data = new uint[_polygonTexture.Width * _polygonTexture.Height];

            //Transfer the texture data to the array
            _polygonTexture.GetData(data);

            //Find the vertices that makes up the outline of the shape in the texture
            textureVertices = PolygonTools.CreatePolygon(data, _polygonTexture.Width, false);

            //The tool return vertices as they were found in the texture.
            //We need to find the real center (centroid) of the vertices for 2 reasons:

            //1. To translate the vertices so the polygon is centered around the centroid.
            //Vector2 centroid = -textureVertices.GetCentroid();
            //textureVertices.Translate(ref centroid);

            //2. To draw the texture the correct place.
            //_origin = -centroid;
           
            //We simplify the vertices found in the texture.
            textureVertices = SimplifyTools.CollinearSimplify(textureVertices, 1f);

            //Since it is a concave polygon, we need to partition it into several smaller convex polygons
            list = BayazitDecomposer.ConvexPartition(textureVertices);

            //scale the vertices from graphics space to sim space
           // Vector2 vertScale = new Vector2(ConvertUnits.ToSimUnits(1)) * _scale;
            //foreach (Vertices vertices in list)
            //{
           //     vertices.Scale(ref vertScale);

           // }

            //Create a single body with multiple fixtures
          // _compound = BodyFactory.CreateCompoundPolygon(World, list, 1f, BodyType.Dynamic);
           // _compound.BodyType = BodyType.Dynamic;
   
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Handle polling for our input and handling high-level input
            HandleInput();
            // update our level, passing down the GameTime along with all of our input states
            level.Update(gameTime, keyboardState, gamePadState, touchState, 
                         accelerometerState, Window.CurrentOrientation);

            //Update all the sprites in the game
            foreach (Sprite sprite in sprites)
            {
                sprite.Update(gameTime);
            }

            base.Update(gameTime);
        }

        private void HandleInput()
        {
            // get all of our input states
            keyboardState = Keyboard.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);
            touchState = TouchPanel.GetState();
            accelerometerState = Accelerometer.GetState();

            // Exit the game when back is pressed.
            if (gamePadState.Buttons.Back == ButtonState.Pressed)
                Exit();

            bool continuePressed =
                keyboardState.IsKeyDown(Keys.Space) ||
                gamePadState.IsButtonDown(Buttons.A) ||
                touchState.AnyTouch();

            // Perform the appropriate action to advance the game and
            // to get the player back to playing.
            if (!wasContinuePressed && continuePressed)
            {
                if (!level.Player.IsAlive)
                {
                    level.StartNewLife();
                }
                else if (level.TimeRemaining == TimeSpan.Zero)
                {
                    if (level.ReachedExit)
                        LoadNextLevel();
                    else
                        ReloadCurrentLevel();
                }
            }

            wasContinuePressed = continuePressed;
        }

        private void LoadNextLevel()
        {
            // move to the next level
            levelIndex = (levelIndex + 1) % numberOfLevels;

            // Unloads the content for the current level before loading the next one.
            if (level != null)
                level.Dispose();

            // Load the level.
            string levelPath = string.Format("Content/Levels/{0}.txt", levelIndex);
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                level = new Level(Services, fileStream, levelIndex);
        }

        private void ReloadCurrentLevel()
        {
            --levelIndex;
            LoadNextLevel();
        }

        /// <summary>
        /// Draws the game from background to foreground.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Create a world view projection matrix to use with krypton
            Matrix world = Matrix.Identity;
            Matrix view = Matrix.CreateTranslation(new Vector3(0, 0, 0) * -1f);
            Matrix projection = Matrix.CreateOrthographic(this.mVerticalUnits * this.GraphicsDevice.Viewport.AspectRatio, this.mVerticalUnits, 0, 1);
            Matrix wvp = world * view * projection;
             //this.krypton.CullMode = CullMode.None;
            // Assign the matrix and pre-render the lightmap.
            // Make sure not to change the position of any lights or shadow hulls after this call, as it won't take effect till the next frame!
            this.krypton.LightMapPrepare();

            // Make sure we clear the backbuffer *after* Krypton is done pre-rendering
            this.GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            level.Draw(gameTime, spriteBatch);

            DrawHud();

            foreach (Sprite sprite in sprites)
            {
                sprite.Draw(spriteBatch);
            }

            //spriteBatch.Draw(_polygonTexture, ConvertUnits.ToDisplayUnits(_compound.Position),
            //                          null, Color.Tomato, _compound.Rotation, _origin, _scale, SpriteEffects.None,
            //                            0f);

            spriteBatch.End();

            // Draw hulls
            //this.DebugDrawHulls(true);

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

        private void DrawHud()
        {
            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);

            // Draw time remaining. Uses modulo division to cause blinking when the
            // player is running out of time.
            string timeString = "TIME: " + level.TimeRemaining.Minutes.ToString("00") + ":" + level.TimeRemaining.Seconds.ToString("00");
            Color timeColor;
            if (level.TimeRemaining > WarningTime ||
                level.ReachedExit ||
                (int)level.TimeRemaining.TotalSeconds % 2 == 0)
            {
                timeColor = Color.Yellow;
            }
            else
            {
                timeColor = Color.Red;
            }
            DrawShadowedString(hudFont, timeString, hudLocation, timeColor);

            // Draw score
            float timeHeight = hudFont.MeasureString(timeString).Y;
            DrawShadowedString(hudFont, "SCORE: " + level.Score.ToString(), hudLocation + new Vector2(0.0f, timeHeight * 1.2f), Color.Yellow);
           
            // Determine the status overlay message to show.
            Texture2D status = null;
            if (level.TimeRemaining == TimeSpan.Zero)
            {
                if (level.ReachedExit)
                {
                    status = winOverlay;
                }
                else
                {
                    status = loseOverlay;
                }
            }
            else if (!level.Player.IsAlive)
            {
                status = diedOverlay;
            }

            if (status != null)
            {
                // Draw status message.
                Vector2 statusSize = new Vector2(status.Width, status.Height);
                spriteBatch.Draw(status, center - statusSize / 2, Color.White);
            }
        }

        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(font, value, position, color);
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
