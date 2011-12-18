using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using HauntedHouse.Entities;
using Krypton;
using Krypton.Lights;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HauntedHouse
{
    //Generic Object Loaded from Tiled. Type must be found and the relevent game object created from here and added to its approprate list.
    public class Entity
    {
        public String EntityType;
        public String EntityName;
        public Rectangle EntityBounds;
        public Dictionary<String, String> Properties;

        public void Intialise(List<Platform> platforms,KryptonEngine krypton,ContentManager content,GraphicsDevice graphicsDevice)
        {
            if (EntityType == "Platform")
            {
                TileCollision tileCollision = TileCollision.Platform;
                if (Properties["CollisionType"] == "Platform") {tileCollision = TileCollision.Platform;}
                if (Properties["CollisionType"] == "Passable") { tileCollision = TileCollision.Passable; }
                if (Properties["CollisionType"] == "Impassable") { tileCollision = TileCollision.Impassable; }
                Platform platform = new Platform(EntityBounds, tileCollision, Convert.ToBoolean(Properties["IsShadowCaster"]),krypton);
                platforms.Add(platform);
            }

            if (EntityType == "Light")
            {
                Color color = new Color();
                color.R = (byte)Convert.ToInt32(Properties["R"]);
                color.G = (byte)Convert.ToInt32(Properties["G"]);
                color.B = (byte)Convert.ToInt32(Properties["B"]);
                color.A = 255;

                /*
                    X = EntityBounds.X,
                    Y = EntityBounds.Y,
                    Range = (float)Convert.ToInt32(Properties["Range"]),
                    Intensity = (float)Convert.ToDouble(Properties["Intensity"]),
                    Color = color,
                    ShadowType = ShadowType.Illuminated,
                    Fov = MathHelper.PiOver2 * (float) (0.5)
                 * */

                Light2D light = new Light2D
                {
                    Texture = LightTextureBuilder.CreatePointLight(graphicsDevice, 1024),
                    X = EntityBounds.X,
                    Y = EntityBounds.Y,
                    Range = (float)Convert.ToInt32(Properties["Range"]),
                    Intensity = (float)Convert.ToDouble(Properties["Intensity"]),
                    Color = color,
                    ShadowType = ShadowType.Illuminated,
                    Fov = MathHelper.PiOver2 * (float)Convert.ToDouble(Properties["Fov"])
                };
                krypton.Lights.Add(light);
            }
        }
    }
}

