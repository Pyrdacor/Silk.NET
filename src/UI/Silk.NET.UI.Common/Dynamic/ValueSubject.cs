using System;

namespace Silk.NET.UI.Common.Dynamic
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

        public override void Subscribe(Action<T> next, Action<Exception> error = null, Action complete = null)
        {
            base.Subscribe(next, error, complete);

            if (firstSubscription)
            {
                firstSubscription = false;
                nextAction?.Invoke(currentValue);
            }
        }
    }
}