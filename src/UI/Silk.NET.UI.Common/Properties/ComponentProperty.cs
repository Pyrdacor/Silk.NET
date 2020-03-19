using System;

namespace Silk.NET.UI.Common.Properties
{
    using Dynamic;

    public abstract class ComponentProperty<T>
    {
        public string Name { get; }

        public abstract T Value { get; set; }

        internal Observable<BindableVariable> BoundVariable { get; private set; }

        public event Action ValueChanged;
        /// <summary>
        /// Is not triggered by manual value changes.
        /// Only if the bound variable changes.
        /// </summary>
        public event Action DynamicValueChanged;

        internal ComponentProperty(string name)
        {
            Name = name;
        }

        internal void Bind(Observable<BindableVariable> variable)
        {
            BoundVariable = variable;
            BoundVariable?.Subscribe(value => {
                var newValue = (T)Convert.ChangeType(value.Value, typeof(T));
                if (!Value.Equals(newValue))
                {
                    Value = newValue;
                    OnValueChanged();
                    DynamicValueChanged?.Invoke();
                }
            }, error => throw error);
        }

        internal void OnValueChanged()
        {
            ValueChanged?.Invoke();
        }
    }
}
