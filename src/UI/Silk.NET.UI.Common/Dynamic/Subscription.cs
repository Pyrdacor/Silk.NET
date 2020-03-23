namespace Silk.NET.UI
{
    public class Subscription<T>
    {
        internal static readonly Subscription<T> Empty = new Subscription<T>(null, null);
        private Observable<T> observable;
        private Subscriber<T> subscriber;

        internal Subscription(Observable<T> observable, Subscriber<T> subscriber)
        {
            this.observable = observable;
            this.subscriber = subscriber;
        }

        public void Unsubscribe()
        {
            observable?.Unsubscribe(subscriber);
        }
    }
}