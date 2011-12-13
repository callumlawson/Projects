using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using FarseerPhysics.Common;
//using FarseerPhysics.Common.PolygonManipulation;
//using FarseerPhysics.Common.Decomposition;
//using FarseerPhysics.SamplesFramework;
using Krypton;

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

        // The position of the Sprite relative to the top left corner of thescreen
        public Vector2 Position;

        //Angle of rotation
        public float Angle;

        //Alpha
        public float Opacity;

        //Scale
        public float Scale;

        // The state of the Sprite
        public bool Active;

        // Get the width of the Sprite
        public int Width;

        // Get the height of the Sprite
        public int Height;

        KryptonEngine krypton;

        //Shadowhull
        List<ShadowHull> hulls;

        //Convex hull
        //List<Vertices> list;
        //Vertices textureVertices;
        private Vector2 _origin;
        private float _scale;

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

        public Sprite(Texture2D texture, Vector2 position, KryptonEngine krypton)
        {
            Initialize();
            Active = true;
            Position = position;
            Texture = texture;
            Width = texture.Width;
            Height = texture.Height;
            this.krypton = krypton;
            //findShadowHull(Texture);
        }

        public void Initialize()
        {
            Scale = 1.0f;
            Opacity = 200;
            Angle = 0;
            hulls = new List<ShadowHull>();
        }

        /*
        public void findShadowHull(Texture2D texture)
        {
            //Create an array to hold the data from the texture
            uint[] data = new uint[texture.Width * texture.Height];

            //Transfer the texture data to the array
            texture.GetData(data);

            //Find the vertices that makes up the outline of the shape in the texture
            textureVertices = PolygonTools.CreatePolygon(data, texture.Width, false);

            //The tool return vertices as they were found in the texture.
            //We need to find the real center (centroid) of the vertices for 2 reasons:

            //1. To translate the vertices so the polygon is centered around the centroid.
            Vector2 centroid = -textureVertices.GetCentroid();
            textureVertices.Translate(ref centroid);

            //2. To draw the texture the correct place.
            _origin = -centroid;

            //We simplify the vertices found in the texture.
            textureVertices = SimplifyTools.CollinearSimplify(textureVertices, 0f);

            //Since it is a concave polygon, we need to partition it into several smaller convex polygons
            list = BayazitDecomposer.ConvexPartition(textureVertices);

            //Adjust the scale of the object for WP7's lower resolution
            //Adjust the scale of the object for WP7's lower resolution
            #if WINDOWS_PHONE
                _scale = 0.6f;
            #else
            _scale = 1f;
            #endif

            //scale the vertices from graphics space to sim space
            Vector2 vertScale = new Vector2(ConvertUnits.ToSimUnits(1)) * _scale;
            foreach (Vertices vertices in list)
            {
                vertices.Scale(ref vertScale);
                Vector2[] verticesArray = vertices.ToArray();
                var hull = ShadowHull.CreateConvex(ref verticesArray);
                hulls.Add(hull);
                krypton.Hulls.Add(hull);
            }
        }
         */

        virtual public void Update(GameTime gameTime)
        {
             //Update the position of the Animation
            Animation.Position = Position;
             //Update Animation
            Animation.Update(gameTime);

            //updateHulls();

            // If the enemy is past the screen or its health reaches 0 then deactivateit
            //if (Position.X < -Width || Position.Y > 520)
            //{
                // By setting the Active flag to false, the game will remove this object from the 
                // active game list
            //    Active = false;
            //}
        }

        public void updateHulls()
        {
            foreach (ShadowHull hull in hulls)
            {
                hull.Position = Position;
                //hull.Angle = this.Angle;
                //hull.Scale = new Vector2(this.Scale);
                //hull.Opacity = this.Opacity;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the animation
            if (Animated)
            {
                Animation.Draw(spriteBatch);
            }
            else spriteBatch.Draw(Texture, this.Position, null, Color.White, Angle, new Vector2(Position.X - Width / 2, Position.Y - Height / 2), Scale, SpriteEffects.None, 0f);
           
        }
    }
}
