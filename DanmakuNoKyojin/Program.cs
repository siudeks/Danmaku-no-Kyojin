using Akka.Actor;
using Microsoft.Xna.Framework.Graphics;
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

            var p = game.Pixel;
            game.GameProcessor = kernel.Get<GameProcessor>(new Ninject.Parameters.ConstructorArgument("pixel", p));

            game.Run();

            kernel.Dispose();
        }
    }
#endif
}

