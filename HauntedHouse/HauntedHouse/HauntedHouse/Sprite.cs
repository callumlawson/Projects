using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Krypton;
using FarseerPhysics.Common;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Common.Decomposition;

namespace HauntedHouse
{
    class Sprite
    {
        //Is the sprite animated?
        public bool Animated = false;

        // Animation representing the Sprite
        public Animation Animation;

        // Texture2D representing the Sprite
        public Texture2D Texture;

        // Physics state
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position = Vector2.Zero;

        // Physics state
        public bool IsShadowCaster
        {
            get { return isShadowCaster; }
            set { isShadowCaster = value; }
        }
        bool isShadowCaster = true;

        // The velocity of the Sprite which is added to the Position each step
        public Vector2 Velocity = Vector2.Zero;

        //Angle of rotation
        public float Angle = 0.0f;

        //Alpha
        public float Opacity = 255;

        //Scale
        public float Scale = 1f;

        // The state of the Sprite
        public bool Active = true;

        // Should the sprite be drawn? This would be set to false when it moves offscreen
        public bool Drawn = true;

        // Get the width of the Sprite
        public int Width;

        // Get the height of the Sprite
        public int Height;

        //Krypton
        KryptonEngine krypton;

        //List of the shadowhulls which represent this object
        List<ShadowHull> hulls;

        //Convex hull
        //List<Vertices> list;
        //Vertices textureVertices;
        private Vector2 _origin;
        private float _scale;
        Vertices textureVertices;
        List<Vertices> vertices;

        /*
        public Sprite(SpriteAnimation animation, Vector2 position)
        {
            Initialize();
            // Load the Sprite texture
            Animation = animation;
            Active = true;
            // Set the position of the enemy
            Position = position;
        }
        */

        public Sprite()
        {
        }

        public Sprite(Texture2D texture, Vector2 position,bool isShadowCaster, KryptonEngine krypton)
        {
            Active = true;
            this.position = position;

            Texture = texture;
            Width = texture.Width;
            Height = texture.Height;

            this.krypton = krypton;

            this.isShadowCaster = isShadowCaster;
            if (isShadowCaster)
            {
                hulls = new List<ShadowHull>();
                findShadowHull(Texture);
            }
        }
        
        public void findShadowHull(Texture2D texture)
        {
            //Create an array to hold the data from the texture
            uint[] data = new uint[texture.Width * texture.Height];

            //Transfer the texture data to the array
            texture.GetData(data);

            //Find the vertices that makes up the outline of the shape in the texture
            textureVertices = PolygonTools.CreatePolygon(data, texture.Width, false);

            //We simplify the vertices found in the texture.
            textureVertices = SimplifyTools.DouglasPeuckerSimplify(textureVertices, 0.5f);

            //Since it is a concave polygon, we need to partition it into several smaller convex polygons
            vertices = BayazitDecomposer.ConvexPartition(textureVertices);
           
            _scale = 1f;

            //scale the vertices from graphics space to sim space
            foreach (Vertices vertex in vertices)
            {
                Vector2[] verticesArray = vertex.ToArray();
                var hull = ShadowHull.CreateConvex(ref verticesArray);
                hulls.Add(hull);
                krypton.Hulls.Add(hull);
            }
        }
        

        virtual public void Update(GameTime gameTime)
        {
            Position += Velocity;

            if (Animated)
            {
                //Update the position of the Animation
                Animation.Position = Position;
                //Update Animation
                Animation.Update(gameTime);
            }
            else
            {
                if (isShadowCaster)
                {
                    updateHulls(gameTime);
                }
            }

            // If the enemy is past the screen or its health reaches 0 then deactivateit
            //if (Position.X < -Width || Position.Y > 520)
            //{
                // By setting the Active flag to false, the game will remove this object from the 
                // active game list
            //    Active = false;
            //}
        }

        public void updateHulls(GameTime gameTime)
        {
            foreach (ShadowHull hull in hulls)
            {
               hull.Position = this.Position;
               hull.Angle = this.Angle;
               hull.Scale = new Vector2(this.Scale);
               hull.Opacity = this.Opacity;
               krypton.Hulls.Add(hull);
            }
        }

        virtual public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the animation
            if (Animated)
            {
                Animation.Draw(spriteBatch);
            }
            else spriteBatch.Draw(Texture, Position, null, Color.White, Angle, Vector2.Zero, Scale, SpriteEffects.None, 0f);
        }
    }
}
