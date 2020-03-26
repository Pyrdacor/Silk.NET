namespace Silk.NET.UI.Properties
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
                    OnValueChanged();
                }
            }
        }

        internal IntProperty(string name, int? initialValue = null)
            : base(name)
        {
            _value = initialValue;
        }
    }
}
