using System.Drawing;

namespace Silk.NET.UI.Common.Properties
{
    public class ColorProperty : ControlProperty<ColorValue?>
    {
        private ColorValue? value = null;

        public override ColorValue? Value 
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

        internal ColorProperty(string name, string initialValue = null)
            : base(name)
        {
            value = initialValue;
        }

        internal ColorProperty(string name, int? initialValue = null)
            : base(name)
        {
            value = initialValue;
        }

        internal ColorProperty(string name, Color? initialValue = null)
            : base(name)
        {
            value = initialValue;
        }

        internal ColorProperty(string name, ColorValue? initialValue = null)
            : base(name)
        {
            value = initialValue;
        }
    }
}
