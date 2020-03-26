using System;

namespace Silk.NET.UI
{
    public class EnumProperty : ControlProperty<int?>
    {
        private Type _enumType; // TODO: needed?
        private int? _value = null;

        public override int? Value 
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

        internal EnumProperty(string name, Type enumType, int? initialValue = null)
            : base(name)
        {
            _enumType = enumType;
            _value = initialValue;
        }
    }
}
