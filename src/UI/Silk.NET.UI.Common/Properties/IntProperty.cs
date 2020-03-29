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

        internal override void SetValue(object value)
        {
            if (Object.ReferenceEquals(value, null))
            {
                Value = null;
                return;
            }
            
            var type = value.GetType();

            if (type == typeof(int))
                Value = (int)value;
            else if (type == typeof(int?))
                Value = (int?)value;
            else if (type == typeof(string))
                Value = int.Parse((string)value);
            else if (type == typeof(bool))
                Value = (bool)value ? 1 : 0;
            else
                throw new InvalidCastException();
        }

        internal override bool IsEqual<U>(U value)
        {
            var type = typeof(U);

            if (type == typeof(int))
                return _value == (int)(object)value;
            else if (type == typeof(int?))
            {
                var nullableInt = (int?)(object)value;
                if (nullableInt.HasValue != _value.HasValue)
                    return false;
                return nullableInt == _value;
            }
            else if (type == typeof(string))
            {
                string stringValue = (string)(object)value;

                if (stringValue == null)
                    return !_value.HasValue;
                else if (!_value.HasValue)
                    return false;

                return _value.Value.ToString() == stringValue;
            }
            else if (type == typeof(bool))
                return _value.HasValue && (_value.Value != 0) == (bool)(object)value;
            else
                return false;
        }
    }
}
