using DanmakuNoKyojin.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ninject.Modules;

namespace DanmakuNoKyojin
{
    public sealed class IocDefinitions : NinjectModule
    {
        public override void Load()
        {
            Bind<GameProcessor>().ToSelf();
            Bind<GameRunner, IContentLoader, IViewportProvider>().To<GameRunner>().InSingletonScope();
        }
    }
}
