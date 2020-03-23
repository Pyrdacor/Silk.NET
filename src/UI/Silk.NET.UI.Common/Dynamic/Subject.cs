using System;

namespace Silk.NET.UI
{
    /// <summary>
    /// Subject which provides a value by using its
    /// <see cref="Next"> method.
    /// </summary>
    public class Subject<T> : Observable<T>, IObserver<T>, IObservableStatusProvider
    {
        protected T currentValue = default(T);
        private Exception errorException = null;
        protected bool hasValue = false;
        protected bool errored = false;
        protected bool completed = false;

        internal T CurrentValue => currentValue;
        internal Exception ErrorException => errorException;
        bool IObservableStatusProvider.HasValue => hasValue;
        bool IObservableStatusProvider.Errored => errored;
        bool IObservableStatusProvider.Completed => completed;

        public virtual void Next(T value)
        {
            if (completed)
                return;

            currentValue = value;
            hasValue = true;

            CallNextActions(currentValue);
        }

        public virtual void Error(Exception error)
        {
            if (completed)
                return;

            hasValue = false;
            errorException = error;
            errored = true;
            completed = true;
            CallErrorActions(error);
        }

        public virtual void Complete()
        {
            if (completed)
                return;

            completed = true;
            CallCompleteActions();
        }

        public void CompleteWith(T value)
        {
            Next(value);
            Complete();
        }

        internal override Subject<T> AsSubject()
        {
            return this;
        }
    }
}