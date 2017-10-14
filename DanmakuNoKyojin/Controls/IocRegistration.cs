using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ninject.Modules;
using System;
using System.Reactive.Subjects;

namespace DanmakuNoKyojin.Controls
{
    public sealed class IocRegistration : NinjectModule
    {
        public override void Load()
        {
            var mouseStateSubject = new Subject<MouseState>();
            Bind<IObservable<MouseState>>().ToConstant(mouseStateSubject);
            Bind<IObserver<MouseState>>().ToConstant(mouseStateSubject);

            Bind<InputHandler, IUpdatablePart>().To<InputHandler>().InSingletonScope();
        }
    }
}
