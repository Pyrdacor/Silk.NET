using System;

namespace Silk.NET.UI
{
    internal interface IControlProperty
    {
        string Name { get; }
        bool ChangeEventsEnabled { get; set; }
        bool HasValue { get; }
        U ConvertTo<U>();
    }

    internal class DisableChangeEventContext : IDisposable
    {
        private IControlProperty[] _properties;

        internal DisableChangeEventContext(params IControlProperty[] properties)
        {
            _properties = properties;

            foreach (var property in _properties)
                property.ChangeEventsEnabled = false;
        }

        public void Dispose()
        {
            foreach (var property in _properties)
                property.ChangeEventsEnabled = true;
        }
    }

    public abstract class ControlProperty<T> : IControlProperty
    {
        public string Name { get; }
        public abstract T Value { get; set; }
        public bool HasValue { get; protected set; } = false;
        public bool ChangeEventsEnabled { get; set; } = true;

        internal event Action InternalValueChanged;
        public event Action ValueChanged;
        /// <summary>
        /// Is not triggered by manual value changes.
        /// Only if the bound variable changes.
        /// </summary>
        public event Action DynamicValueChanged;

        internal ControlProperty(string name)
        {
            Name = name;
        }

        internal void Bind(Observable<T> variable)
        {
            variable?.Subscribe(value =>
            {
                if (!Value.Equals(value))
                {
                    Value = value;
                    if (ChangeEventsEnabled)
                    {
                        OnValueChanged();
                        DynamicValueChanged?.Invoke();
                    }
                }
            }, error => throw error); // TODO: how to handle errors here
        }

        internal void OnValueChanged()
        {
            if (ChangeEventsEnabled)
                ValueChanged?.Invoke();
            InternalValueChanged?.Invoke();
        }

        U IControlProperty.ConvertTo<U>()
        {
            return ConvertTo<U>();
        }

        internal abstract U ConvertTo<U>();
    }
}
