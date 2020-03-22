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

        internal AllDirectionProperty(string name)
            : base(name)
        {

        }

        internal AllDirectionProperty(string name, AllDirectionStyleValue<T>? initialValue)
            : base(name)
        {
            value = initialValue;
        }

        internal AllDirectionProperty(string name, AllDirectionStyleValue<T> initialValue)
            : base(name)
        {
            value = initialValue;
        }

        internal AllDirectionProperty(string name, string initialValue)
            : base(name)
        {
            value = initialValue;
        }

        internal AllDirectionProperty(string name, T? initialValue)
            : base(name)
        {
            value = initialValue;
        }

        internal AllDirectionProperty(string name, T initialValue)
            : base(name)
        {
            value = initialValue;
        }

        internal AllDirectionProperty(string name, Tuple<T, T> initialValue)
            : base(name)
        {
            value = initialValue;
        }

        internal AllDirectionProperty(string name, Tuple<T, T, T> initialValue)
            : base(name)
        {
            value = initialValue;
        }

        internal AllDirectionProperty(string name, Tuple<T, T, T, T> initialValue)
            : base(name)
        {
            value = initialValue;
        }
    }
}
