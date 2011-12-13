using System;

namespace HauntedHouse
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (HauntedHouse game = new HauntedHouse())
            {
                game.Run();
            }
        }
    }
#endif
}

