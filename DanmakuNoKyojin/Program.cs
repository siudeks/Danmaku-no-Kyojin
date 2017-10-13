using Akka.Actor;
using Ninject;
using System;

namespace DanmakuNoKyojin
{
#if WINDOWS || XBOX
    static class Program
    {
        public static ActorSystem system;

        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Load(AppDomain.CurrentDomain.GetAssemblies());

            system = ActorSystem.Create("default");
            var game = kernel.Get<GameRunner>();

            game.GameProcessor = kernel.Get<GameProcessor>();
            
            game.Run();

            kernel.Dispose();
        }
    }
#endif
}

