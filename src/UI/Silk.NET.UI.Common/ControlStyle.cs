using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Silk.NET.UI
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
                var checkType = fieldType;

                if (CheckGenericType(fieldType, typeof(Nullable<>)))
                {
                    checkType = fieldType.GenericTypeArguments[0];
                }

                if (checkType.IsPrimitive || checkType.IsEnum ||
                    CheckGenericType(checkType, typeof(AllDirectionStyleValue<>)) ||
                    checkType == typeof(ColorValue)
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
                    var property = CreateProperty(name, fieldType, defaultValue);

                    if (property != null)
                        DefaultStyleProperties.Add(name, property);
                }
                else
                {
                    GetDefaultStylesFromType(prefix + field.Name + ".", checkType);
                }
            }
        }

        internal static bool CheckGenericType(Type typeToCheck, Type baseType)
        {
            return typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == baseType;
        }

        private static IControlProperty CreateProperty(string name, Type fieldType, object value)
        {
            if (value == null)
                return null;

            if (CheckGenericType(fieldType, typeof(Nullable<>)))
            {
                return CreateProperty(name, fieldType.GenericTypeArguments[0], value);
            }
            else if (fieldType == typeof(int))
            {
                return new IntProperty(name, (int?)(int)value);
            }
            else if (fieldType == typeof(bool))
            {
                return new BoolProperty(name, (bool?)(bool)value);
            }
            else if (fieldType == typeof(string))
            {
                return new StringProperty(name, (string)value);
            }
            else if (fieldType.IsEnum)
            {
                return new EnumProperty(name, fieldType, (int?)(int)value);
            }
            else if (fieldType == typeof(ColorValue))
            {
                ColorValue? colorValue = null;
                var valueType = value.GetType();

                if (valueType == typeof(string))
                    colorValue = (string)value;
                else if (valueType == typeof(int))
                    colorValue = (int)value;
                else if (valueType == typeof(System.Drawing.Color))
                    colorValue = (int)value;
                else if (valueType == typeof(ColorValue))
                    colorValue = (ColorValue)value;
                else
                    colorValue = (ColorValue?)value;

                return new ColorProperty(name, colorValue);
            }
            else if (CheckGenericType(fieldType, typeof(AllDirectionStyleValue<>)))
            {
                Type propertyBaseType = typeof(AllDirectionProperty<>);
                Type[] typeArgs = { fieldType.GenericTypeArguments[0] };
                var propertyType = propertyBaseType.MakeGenericType(typeArgs);
                return (IControlProperty)Activator.CreateInstance(propertyType, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { name, value }, null);
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
            {
                var property = CreateProperty(name, value.GetType(), value);

                if (property != null)
                    styleProperties.Add(name, property);
            }
        }
    }
}