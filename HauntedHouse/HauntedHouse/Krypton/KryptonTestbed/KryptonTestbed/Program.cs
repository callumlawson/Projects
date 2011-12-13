using System;

namespace KryptonTestbed
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (KryptonDemoGame game = new KryptonDemoGame())
            {
                game.Run();
            }
        }
    }
#endif
}

