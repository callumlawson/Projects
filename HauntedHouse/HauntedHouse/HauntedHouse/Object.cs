using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HauntedHouse
{
    //Generic Object Loaded from Tiled. Type must be found and the relevent game object created!
    public class Object
    {
        public String ObjectType;
        public String ObjectName;
        public Rectangle ObjectBounds;

        public Object()
        {
            Console.WriteLine("ObjectType");
        }
    }
}
