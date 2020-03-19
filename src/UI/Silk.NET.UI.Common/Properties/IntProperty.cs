namespace Silk.NET.UI.Common.Properties
{
    public class IntProperty : ComponentProperty<int?>
    {
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

        internal IntProperty(string name, int? initialValue = null)
            : base(name)
        {
            Value = initialValue;
        }
    }
}
