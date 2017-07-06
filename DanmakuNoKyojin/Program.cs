using System;

namespace DanmakuNoKyojin
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (var game = new GameRunner())
            {
                game.Run();
            }
        }
    }
#endif
}

