using System;

namespace Project_Origin
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Shooter game = new Shooter())
            {
                game.Run();
            }
        }
    }
#endif
}

