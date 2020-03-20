namespace Silk.NET.UI.Common.Properties
{
    public class BoolProperty : ControlProperty<bool?>
    {
        private bool? value = null;

        public override bool? Value 
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

        internal BoolProperty(string name, bool? initialValue = null)
            : base(name)
        {
            Value = initialValue;
        }
    }
}
