using System;

namespace Silk.NET.UI.Common.Dynamic
{
    public abstract class Observable<T>
    {
        protected Action<T> nextAction = null;
        protected Action<Exception> errorAction = null;
        protected Action completeAction = null;

        public virtual void Subscribe(Action<T> next, Action<Exception> error = null, Action complete = null)
        {
            nextAction = next;
            errorAction = error;
            completeAction = complete;
        }

        // TODO: Operators should unsubscribe and sometimes check for complete or has value

        public Observable<U> Map<U>(Func<T, U> mapper)
        {
            var newObservable = new Subject<U>();

            this.Subscribe(value => newObservable.Next(mapper(value)));

            return newObservable;
        }

        public Observable<Tuple<T, U>> Combine<U>(Observable<U> other)
        {
            var newObservable = new ValueSubject<Tuple<T, U>>(null);

            this.Subscribe(value => newObservable.Next(Tuple.Create(
                value,
                newObservable.Value == null ? default(U) : newObservable.Value.Item2
            )));
            other.Subscribe(value => newObservable.Next(Tuple.Create(
                newObservable.Value == null ? default(T) : newObservable.Value.Item1,
                value
            )));

            return newObservable;
        }

        public Observable<V> Merge<U, V>(Observable<U> other, Func<T, U, V> merger)
        {
            var newObservable = new Subject<V>();

            Combine(other).Subscribe(value=> newObservable.Next(merger(value.Item1, value.Item2)));

            return newObservable;
        }
    }
}