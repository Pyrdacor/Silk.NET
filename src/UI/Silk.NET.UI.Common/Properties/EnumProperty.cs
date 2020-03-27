using System;

namespace Silk.NET.UI
{
    public class EnumProperty : ControlProperty<int?>
    {
        private Type _enumType;
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

        internal EnumProperty(string name, Type enumType, int? initialValue = null)
            : base(name)
        {
            _enumType = enumType;
            _value = initialValue;
            HasValue = _value != null;
        }

        internal override U ConvertTo<U>()
        {
            var type = typeof(U);

            if (type == _enumType)
                return (U)(object)(_value.HasValue ? _value.Value : throw new InvalidCastException());
            else if (Util.CheckGenericType(type, typeof(Nullable<>)) && type.GenericTypeArguments[0] == _enumType)
                return (U)(object)(_value.HasValue ? Enum.ToObject(_enumType, _value.Value) : null);
            else if (type == typeof(string))
                return (U)(object)(_value.HasValue ? _value.Value.ToString() : null);
            else if (type == typeof(int))
                return (U)(object)(_value.HasValue ? _value.Value : throw new InvalidCastException());
            else
                throw new InvalidCastException();
        }
    }
}
