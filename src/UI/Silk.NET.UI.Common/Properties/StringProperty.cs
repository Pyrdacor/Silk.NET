using System;

namespace Silk.NET.UI
{
    public class StringProperty : ControlProperty<string>
    {
        private string _value = null;

        public override string Value 
        { 
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    HasValue = _value != null;
                    OnValueChanged();
                }
            }
        }

        internal StringProperty(string name, string initialValue = null)
            : base(name)
        {
            _value = initialValue;
            HasValue = _value != null;
        }

        internal override U ConvertTo<U>()
        {
            var type = typeof(U);

            if (type == typeof(string))
                return (U)(object)_value;
            else
                throw new InvalidCastException();
        }
    }
}
