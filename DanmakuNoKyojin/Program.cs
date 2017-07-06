using Ninject;
using System;

namespace DanmakuNoKyojin
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Load(AppDomain.CurrentDomain.GetAssemblies());

            var game = kernel.Get<GameRunner>();
            
            game.Run();

            kernel.Dispose();
        }
    }
#endif
}

