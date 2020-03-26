using System.Drawing;

namespace Silk.NET.UI.Properties
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
                    OnValueChanged();
                }
            }
        }

        internal ColorProperty(string name, string initialValue = null)
            : base(name)
        {
            _value = initialValue;
        }

        internal ColorProperty(string name, int? initialValue = null)
            : base(name)
        {
            _value = initialValue;
        }

        internal ColorProperty(string name, Color? initialValue = null)
            : base(name)
        {
            _value = initialValue;
        }

        internal ColorProperty(string name, ColorValue? initialValue = null)
            : base(name)
        {
            _value = initialValue;
        }
    }
}
