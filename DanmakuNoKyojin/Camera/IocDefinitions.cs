using Ninject.Modules;

namespace DanmakuNoKyojin.Camera
{
    public sealed class IocDefinitions : NinjectModule
    {
        public override void Load()
        {
            Bind<Camera2D>().ToSelf().InSingletonScope();
        }
    }
}
