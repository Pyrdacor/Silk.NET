using System;

namespace Silk.NET.UI.Common.Properties
{
    using Dynamic;

    internal interface IControlProperty
    {
        string Name { get; }
        bool ChangeEventsEnabled { get; set; }
    }

    internal class DisableChangeEventContext : IDisposable
    {
        private IControlProperty[] properties;

        internal DisableChangeEventContext(params IControlProperty[] properties)
        {
            this.properties = properties;

            foreach (var property in properties)
                property.ChangeEventsEnabled = false;
        }

        public void Dispose()
        {
            foreach (var property in properties)
                property.ChangeEventsEnabled = true;
        }
    }

    public abstract class ControlProperty<T> : IControlProperty
    {
        public string Name { get; }

        public abstract T Value { get; set; }

        internal Observable<T> BoundVariable { get; private set; }
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
            BoundVariable = variable;
            BoundVariable?.Subscribe(value => {
                if (!Value.Equals(value))
                {
                    Value = value;
                    if (ChangeEventsEnabled)
                    {
                        OnValueChanged();
                        DynamicValueChanged?.Invoke();
                    }
                }
            }, error => throw error);
        }

        internal void OnValueChanged()
        {
            if (ChangeEventsEnabled)
                ValueChanged?.Invoke();
            InternalValueChanged?.Invoke();
        }
    }
}
