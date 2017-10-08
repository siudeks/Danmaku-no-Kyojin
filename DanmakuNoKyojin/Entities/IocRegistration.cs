using Microsoft.Xna.Framework;
using Ninject.Modules;

namespace DanmakuNoKyojin.Entities
{
    public sealed class IocRegistration : NinjectModule
    {
        public override void Load()
        {
            Bind<IContentBasedPart, IUpdatablePart, IDrawablePart>().To<Ships>().InSingletonScope();
        }
    }
}
