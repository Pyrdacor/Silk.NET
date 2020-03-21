using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Silk.NET.UI.Common
{
    using Properties;

    public class ControlStyle
    {
        private static readonly Dictionary<string, IControlProperty> DefaultStyleProperties = new Dictionary<string, IControlProperty>();
        private readonly Dictionary<string, IControlProperty> styleProperties = new Dictionary<string, IControlProperty>();

        static ControlStyle()
        {
            GetDefaultStylesFromType("", typeof(Style));
        }

        internal static IEnumerable<string> StylePropertyNames => DefaultStyleProperties.Keys;

        private static void GetDefaultStylesFromType(string prefix, Type type)
        {
            foreach (var field in type.GetFields())
            {
                var fieldType = field.FieldType;

                if (fieldType.IsPrimitive || !fieldType.IsEnum ||
                    fieldType == typeof(AllDirectionStyleValue<>) ||
                    fieldType == typeof(ColorValue)
                    )
                {
                    var defaultValueAttribute = field.GetCustomAttribute(typeof(DefaultValueAttribute));
                    object defaultValue;

                    if (defaultValueAttribute != null)
                        defaultValue = (defaultValueAttribute as DefaultValueAttribute).Value;
                    else if (fieldType.IsValueType)
                        defaultValue = Activator.CreateInstance(fieldType);
                    else
                        defaultValue = null;
                    
                    string name = prefix + field.Name;
                    DefaultStyleProperties.Add(name, CreateProperty(name, fieldType, defaultValue));
                }
                else
                {
                    GetDefaultStylesFromType(prefix + field.Name + ".", fieldType);
                }
            }
        }

        private static IControlProperty CreateProperty(string name, Type fieldType, object value)
        {
            if (fieldType == typeof(int))
            {
                return new IntProperty(name, (int?)value);
            }
            else if (fieldType == typeof(bool))
            {
                return new BoolProperty(name, (bool?)value);
            }
            else if (fieldType == typeof(string))
            {
                return new StringProperty(name, (string)value);
            }
            else if (fieldType == typeof(ColorValue))
            {
                return new ColorProperty(name, (ColorValue?)value);
            }
            else if (fieldType == typeof(AllDirectionStyleValue<>))
            {
                Type propertyBaseType = typeof(AllDirectionProperty<>);
                Type[] typeArgs = { fieldType };
                var propertyType = propertyBaseType.MakeGenericType(typeArgs);
                return (IControlProperty)Activator.CreateInstance(propertyType, new object[] { value });
            }
            else
            {
                throw new ArgumentException($"Unsupported property type `{fieldType.Name}` for style `{name}`.");
            }
        }

        public dynamic this[string name]
        {
            get
            {
                if (!styleProperties.ContainsKey(name))
                {
                    return null;
                }

                return styleProperties[name];
            }
        }

        public ControlProperty<T> Get<T>(string name)
        {
            return this[name] as ControlProperty<T>;
        }

        internal void SetProperty(string name, object value)
        {
            if (!styleProperties.ContainsKey(name)) // only set once
                styleProperties.Add(name, CreateProperty(name, value.GetType(), value));
        }
    }
}