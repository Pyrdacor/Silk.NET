using System;
namespace Silk.NET.UI.Common
{
    public class StyleValue
    {
        private object value;

        public StyleValue(string value)
        {
            this.value = value;
        }

        public StyleValue(double value)
        {
            this.value = value;
        }

        public StyleValue(bool value)
        {
            this.value = value;
        }

        public StyleValue(params StyleValue[] value)
        {
            this.value = value;
        }

        public static implicit operator string(StyleValue value)
        {
            return value.value.ToString();
        }

        public static implicit operator double(StyleValue value)
        {
            if (value.value.GetType() == typeof(double))
                return (double)value.value;

            return double.Parse(value.value.ToString());
        }

        public static implicit operator int(StyleValue value)
        {
            if (value.value.GetType() == typeof(double))
                return (int)Math.Round((double)value.value);

            return int.Parse(value.value.ToString());
        }

        public static implicit operator bool(StyleValue value)
        {
            if (value.value.GetType() == typeof(bool))
                return (bool)value.value;

            var boolString = value.value.ToString().ToLower();
            return !string.IsNullOrWhiteSpace(boolString) &&
                boolString != "0" && boolString != "false" &&
                boolString != "no" && boolString != "null";
        }

        public static implicit operator StyleValue[](StyleValue value)
        {
            if (value.value.GetType() == typeof(StyleValue[]))
                return (StyleValue[])value.value;

            return new StyleValue[1] { value };
        }
    }
}