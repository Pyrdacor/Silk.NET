using System;

namespace Silk.NET.UI.Properties
{
    public class EnumProperty : ControlProperty<int?>
    {
        private Type enumType;
        private int? value = null;

        public override int? Value 
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

        internal EnumProperty(string name, Type enumType, int? initialValue = null)
            : base(name)
        {
            this.enumType = enumType;
            value = initialValue;
        }
    }
}
