using System;

namespace Silk.NET.UI.Dynamic
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

        public override void Subscribe(Action<T> next, Action<Exception> error = null, Action complete = null)
        {
            base.Subscribe(next, error, complete);

            nextAction?.Invoke(currentValue);
        }
    }
}