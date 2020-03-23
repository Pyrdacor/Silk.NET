using System;

namespace Silk.NET.UI
{
    internal class Subscriber<T> : IObserver<T>
    {
        protected Action<T> nextAction = null;
        protected Action<Exception> errorAction = null;
        protected Action completeAction = null;

        public Subscriber(Action<T> next, Action<Exception> error = null, Action complete = null)
        {
            nextAction = next;
            errorAction = error;
            completeAction = complete;
        }

        public void Next(T value) => nextAction?.Invoke(value);
        public void Error(Exception exception) => errorAction?.Invoke(exception);
        public void Complete() => completeAction?.Invoke();
    }
}