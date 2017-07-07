using Ninject.Modules;
using System;
using System.Reactive.Subjects;

namespace DanmakuNoKyojin.Framework
{
    public sealed class IocRegistration : NinjectModule
    {
        public override void Load()
        {
            var eventFrameworkInitialized = new Subject<FrameworkInitialized>();

            Bind<IObservable<FrameworkInitialized>>().ToConstant(eventFrameworkInitialized);
            Bind<IObserver<FrameworkInitialized>>().ToConstant(eventFrameworkInitialized);
        }
    }
}
