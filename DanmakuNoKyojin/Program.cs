using System;

namespace Danmaku_no_Kyojin
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (var game = new GameProcessor())
            {
                game.Run();
            }
        }
    }
#endif
}

