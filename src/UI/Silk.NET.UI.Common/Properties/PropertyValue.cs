namespace Silk.NET.UI
{
    public class PropertyValue<T>
    {
        public T Value { get; }

        internal PropertyValue(T value)
        {
            Value = value;
        }
    }
}