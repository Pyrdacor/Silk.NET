using System;

namespace Silk.NET.UI.Properties
{
    public class AllDirectionProperty<T> : ControlProperty<AllDirectionStyleValue<T>?> where T : struct
    {
        private AllDirectionStyleValue<T>? _value = null;

        public override AllDirectionStyleValue<T>? Value 
        { 
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
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
            _value = initialValue;
        }

        internal AllDirectionProperty(string name, AllDirectionStyleValue<T> initialValue)
            : base(name)
        {
            _value = initialValue;
        }

        internal AllDirectionProperty(string name, string initialValue)
            : base(name)
        {
            _value = initialValue;
        }

        internal AllDirectionProperty(string name, T? initialValue)
            : base(name)
        {
            _value = initialValue;
        }

        internal AllDirectionProperty(string name, T initialValue)
            : base(name)
        {
            _value = initialValue;
        }

        internal AllDirectionProperty(string name, Tuple<T, T> initialValue)
            : base(name)
        {
            _value = initialValue;
        }

        internal AllDirectionProperty(string name, Tuple<T, T, T> initialValue)
            : base(name)
        {
            _value = initialValue;
        }

        internal AllDirectionProperty(string name, Tuple<T, T, T, T> initialValue)
            : base(name)
        {
            _value = initialValue;
        }
    }
}
