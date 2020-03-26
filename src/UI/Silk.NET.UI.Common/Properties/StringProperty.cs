namespace Silk.NET.UI.Properties
{
    public class StringProperty : ControlProperty<string>
    {
        private string _value = null;

        public override string Value 
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

        internal StringProperty(string name, string initialValue = null)
            : base(name)
        {
            _value = initialValue;
        }
    }
}
