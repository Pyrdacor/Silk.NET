namespace Silk.NET.UI.Properties
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
                    OnValueChanged();
                }
            }
        }

        internal BoolProperty(string name, bool? initialValue = null)
            : base(name)
        {
            _value = initialValue;
        }
    }
}
