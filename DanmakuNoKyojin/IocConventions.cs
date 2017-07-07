using DanmakuNoKyojin.Controls;
using Microsoft.Xna.Framework;
using Ninject.Modules;

namespace DanmakuNoKyojin
{
    public class IocConventions : NinjectModule
    {
        public override void Load()
        {
            // TODO Register all IUpdatableParts
            Bind<IUpdatablePart>().To<InputHandler>().InSingletonScope();
        }
    }
}
