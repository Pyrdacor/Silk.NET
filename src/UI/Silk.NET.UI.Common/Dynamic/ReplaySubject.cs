using System;

namespace Silk.NET.UI
{
    /// <summary>
    /// Subject which has an optional initial value and
    /// will emit its value on every subscription immediately.
    /// </summary>
    public class ReplaySubject<T> : Subject<T>
    {
        public ReplaySubject()
        {

        }

        public ReplaySubject(T value)
        {
            currentValue = value;
            hasValue = true;
        }

        public override Subscription<T> Subscribe(Action<T> next, Action<Exception> error = null, Action complete = null)
        {
            if (completed)
                return Subscription<T>.Empty;

            var subscription = base.Subscribe(next, error, complete);

            CallNextActions(currentValue);

            return subscription;
        }
    }
}