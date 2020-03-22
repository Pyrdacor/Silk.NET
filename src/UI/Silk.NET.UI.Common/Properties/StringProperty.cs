namespace Silk.NET.UI.Common.Properties
{
    public class StringProperty : ControlProperty<string>
    {
        private string value = null;

        public override string Value 
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

        internal StringProperty(string name, string initialValue = null)
            : base(name)
        {
            value = initialValue;
        }
    }
}
