using System;

namespace Silk.NET.UI
{
    public class PropertyValueProvider<T>
    {
        private Observable<T> _provider = null;
        internal T Value { get; private set; }
        internal event Action ValueChanged;
        internal bool HasValue { get; private set; } = false;

        private PropertyValueProvider(Observable<T> value)
        {
            _provider = value;
            _provider.SubscribeUntilCompleted(value => {
                Value = value;
                HasValue = true;
                ValueChanged?.Invoke();
            });
        }

        private PropertyValueProvider(T value)
        {
            Value = value;
            HasValue = true;
        }

        public static implicit operator PropertyValueProvider<T>(Observable<T> value)
        {
            return new PropertyValueProvider<T>(value);
        }

        public static implicit operator PropertyValueProvider<T>(T value)
        {
            return new PropertyValueProvider<T>(value);
        }
    }
}