using System;
using System.Drawing;

namespace Silk.NET.UI
{
    public class ColorProperty : ControlProperty<ColorValue?>
    {
        private ColorValue? _value = null;

        public override ColorValue? Value 
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

        internal ColorProperty(string name, string initialValue = null)
            : base(name)
        {
            _value = initialValue;
            HasValue = _value != null;
        }

        internal ColorProperty(string name, int? initialValue = null)
            : base(name)
        {
            _value = initialValue;
            HasValue = _value != null;
        }

        internal ColorProperty(string name, Color? initialValue = null)
            : base(name)
        {
            _value = initialValue;
            HasValue = _value != null;
        }

        internal ColorProperty(string name, ColorValue? initialValue = null)
            : base(name)
        {
            _value = initialValue;
            HasValue = _value != null;
        }

        internal override U ConvertTo<U>()
        {
            var type = typeof(U);

            if (type == typeof(ColorValue))
                return (U)(object)(_value.HasValue ? _value.Value : throw new InvalidCastException());
            else if (type == typeof(ColorValue?))
                return (U)(object)_value;
            else if (type == typeof(string))
                return (U)(object)(_value.HasValue ? _value.Value.ToString() : null);
            else if (type == typeof(int))
                return (U)(object)(_value.HasValue ? _value.Value.ToInt() : 0);
            else
                throw new InvalidCastException();
        }
    }
}
