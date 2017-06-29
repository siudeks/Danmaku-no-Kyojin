using System;

namespace System.Reactive.Disposables
{
    public static class DisposableExtensions
    {
        public static T DisposeWith<T>(this T disposable, CompositeDisposable disposer)
            where T : IDisposable
        {
            disposer.Add(disposable);
            return disposable;
        }
    }
}
