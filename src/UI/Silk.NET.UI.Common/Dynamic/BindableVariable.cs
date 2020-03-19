namespace Silk.NET.UI.Common.Dynamic
{
    public class BindableVariable
    {
        public object Value { get; } = null;

        private BindableVariable(object value)
        {
            Value = value;
        }

        public static implicit operator BindableVariable(string value)
        {
            return new BindableVariable(value);
        }

        public static implicit operator BindableVariable(bool? value)
        {
            return new BindableVariable(value);
        }

        public static implicit operator BindableVariable(int? value)
        {
            return new BindableVariable(value);
        }

        public static implicit operator BindableVariable(double? value)
        {
            return new BindableVariable(value);
        }

        public static implicit operator BindableVariable(ColorValue? value)
        {
            return new BindableVariable(value);
        }

        public T? AsValue<T>() where T : struct => Value != null && Value.GetType() == typeof(T) ? (T)Value : (T?)null;
        public T AsObject<T>() where T : class => Value != null && Value.GetType() == typeof(T) ? (T)Value : null;
        public string AsString() => Value as string;
        public bool? AsBool() => Value != null && Value.GetType() == typeof(bool) ? (bool)Value : (bool?)null;
        public int? AsInt() => Value != null && Value.GetType() == typeof(int) ? (int)Value : (int?)null;
        public double? AsDouble() => Value != null && Value.GetType() == typeof(double) ? (double)Value : (double?)null;
        public ColorValue? AsColor() => Value != null && Value.GetType() == typeof(ColorValue) ? (ColorValue)Value : (ColorValue?)null;
    }
}
