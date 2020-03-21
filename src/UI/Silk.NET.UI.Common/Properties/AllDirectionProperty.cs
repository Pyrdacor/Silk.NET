using System;

namespace Silk.NET.UI.Common.Properties
{
    public class AllDirectionProperty<T> : ControlProperty<AllDirectionStyleValue<T>?> where T : struct
    {
        private AllDirectionStyleValue<T>? value = null;

        public override AllDirectionStyleValue<T>? Value 
        { 
            get => value;
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    OnValueChanged();
                }
            }
        }

        internal AllDirectionProperty(string name, string initialValue = null)
            : base(name)
        {
            Value = initialValue;
        }

        internal AllDirectionProperty(string name, T? initialValue = null)
            : base(name)
        {
            Value = initialValue;
        }

        internal AllDirectionProperty(string name, Tuple<T, T>? initialValue = null)
            : base(name)
        {
            Value = initialValue;
        }

        internal AllDirectionProperty(string name, Tuple<T, T, T>? initialValue = null)
            : base(name)
        {
            Value = initialValue;
        }

        internal AllDirectionProperty(string name, Tuple<T, T, T, T>? initialValue = null)
            : base(name)
        {
            Value = initialValue;
        }
    }
}
