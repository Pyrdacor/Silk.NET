using System;

namespace Silk.NET.UI
{
    public class IntProperty : ControlProperty<int?>
    {
        private int? _value = null;

        public override int? Value 
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

        internal IntProperty(string name, int? initialValue = null)
            : base(name)
        {
            _value = initialValue;
            HasValue = _value != null;
        }

        internal override U ConvertTo<U>()
        {
            var type = typeof(U);

            if (type == typeof(int))
                return (U)(object)(_value.HasValue ? _value.Value : throw new InvalidCastException());
            else if (type == typeof(int?))
                return (U)(object)_value;
            else if (type == typeof(string))
                return (U)(object)(_value.HasValue ? _value.Value.ToString() : null);
            else if (type == typeof(bool))
                return (U)(object)(_value.HasValue ? _value.Value != 0 : false);
            else
                throw new InvalidCastException();
        }
    }
}
