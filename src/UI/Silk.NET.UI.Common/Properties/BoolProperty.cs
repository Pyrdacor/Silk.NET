using System;

namespace Silk.NET.UI
{
    public class BoolProperty : ControlProperty<bool?>
    {
        private bool? _value = null;

        public override bool? Value 
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

        internal BoolProperty(string name, bool? initialValue = null)
            : base(name)
        {
            _value = initialValue;
            HasValue = _value != null;
        }

        internal override U ConvertTo<U>()
        {
            var type = typeof(U);

            if (type == typeof(bool))
                return (U)(object)(_value.HasValue ? _value.Value : throw new InvalidCastException());
            else if (type == typeof(bool?))
                return (U)(object)_value;
            else if (type == typeof(string))
                return (U)(object)(_value.HasValue ? _value.Value.ToString() : null);
            else if (type == typeof(int))
                return (U)(object)(_value.HasValue ? (_value.Value ? 1 : 0) : throw new InvalidCastException());
            else
                throw new InvalidCastException();
        }
    }
}
