using System;

namespace Silk.NET.UI.Dynamic
{
    /// <summary>
    /// Subject which provides a value by using its
    /// <see cref="Next"> method.
    /// </summary>
    public class Subject<T> : Observable<T>, IObserver<T>
    {
        protected T currentValue = default(T);
        protected bool hasValue = false;
        protected bool completed = false;

        public virtual void Next(T value)
        {
            if (completed)
                return;

            currentValue = value;
            hasValue = true;

            nextAction?.Invoke(currentValue);
        }

        public virtual void Error(Exception error)
        {
            if (completed)
                return;

            hasValue = false;
            completed = true;
            errorAction?.Invoke(error);
        }

        public virtual void Complete()
        {
            if (completed)
                return;

            completed = true;
            completeAction?.Invoke();
        }

        public void CompleteWith(T value)
        {
            Next(value);
            Complete();
        }
    }
}