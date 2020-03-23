using System;

namespace Silk.NET.UI
{
    internal class ConstantObservable<T> : Observable<T>, IObservableStatusProvider
    {
        private T value;

        bool IObservableStatusProvider.HasValue => true;
        bool IObservableStatusProvider.Errored => false;
        bool IObservableStatusProvider.Completed => true;

        internal ConstantObservable(T value)
        {
            this.value = value;
        }

        public override Subscription<T> Subscribe(Action<T> next, Action<Exception> error = null, Action complete = null)
        {
            next?.Invoke(value);
            complete?.Invoke();

            return new Subscription<T>(null, null);
        }
    }
}