using System;

namespace Silk.NET.UI
{
    /// <summary>
    /// Subject which has an initial value.
    /// On first subscription the value is immediately provided.
    /// </summary>
    public class ValueSubject<T> : Subject<T>
    {
        private bool firstSubscription = true;

        public T Value => Value;

        public ValueSubject(T value)
        {
            currentValue = value;
            hasValue = true;
        }

        public override Subscription<T> Subscribe(Action<T> next, Action<Exception> error = null, Action complete = null)
        {
            if (completed)
                return Subscription<T>.Empty;

            var subscription = base.Subscribe(next, error, complete);

            if (firstSubscription)
            {
                firstSubscription = false;
                CallNextActions(currentValue);
            }

            return subscription;
        }
    }
}