using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Krypton;

namespace HauntedHouse.Entities
{
    /// <summary>
    /// Controls the collision detection and response behavior of a tile.
    /// </summary>
    public enum TileCollision
    {
        /// <summary>
        /// A passable tile is one which does not hinder player motion at all.
        /// </summary>
        Passable = 0,

        /// <summary>
        /// An impassable tile is one which does not allow the player to move through
        /// it at all. It is completely solid.
        /// </summary>
        Impassable = 1,

        /// <summary>
        /// A platform tile is one which behaves like a passable tile except when the
        /// player is above it. A player can jump up through a platform as well as move
        /// past it to the left and right, but can not fall down through the top of it.
        /// </summary>
        Platform = 2,
    }

    public class Platform
    {
        public TileCollision Collision;
        public Boolean IsShadowCaster;
        public Rectangle PlatformBounds;

        KryptonEngine krypton;

        public Platform(Rectangle platformBounds,TileCollision collision, Boolean isShadowCaster, KryptonEngine krypton)
        {
            this.krypton = krypton;
            Collision = collision;
            IsShadowCaster = isShadowCaster;
            PlatformBounds = platformBounds;
            if (isShadowCaster)
            {
                ShadowHull shadowHull = ShadowHull.CreateRectangle(new Vector2(platformBounds.Width, platformBounds.Height));
                shadowHull.Position.X = platformBounds.X + platformBounds.Width/2;
                shadowHull.Position.Y = platformBounds.Y + platformBounds.Height/2;
                krypton.Hulls.Add(shadowHull);
            }
        }
    }
}
