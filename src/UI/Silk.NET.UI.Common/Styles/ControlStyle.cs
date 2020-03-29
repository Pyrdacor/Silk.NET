using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Silk.NET.UI
{
    public class ControlStyle
    {
        private static readonly Dictionary<string, IControlProperty> DefaultStyleProperties = new Dictionary<string, IControlProperty>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, IControlProperty> _styleProperties = new Dictionary<string, IControlProperty>(StringComparer.OrdinalIgnoreCase);

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

                if (Util.CheckGenericType(fieldType, typeof(Nullable<>)))
                {
                    checkType = fieldType.GenericTypeArguments[0];
                }

                if (checkType.IsPrimitive || checkType.IsEnum ||
                    Util.CheckGenericType(checkType, typeof(AllDirectionStyleValue<>)) ||
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

        private static IControlProperty CreateProperty<T>(string name, T value)
        {
            var defaultProperty = DefaultStyleProperties[name];
            var property = CreateProperty(name, defaultProperty.GetPropertyType(), defaultProperty.GetValue());
            property.SetValue<T>(value);
            return property;
        }

        private static IControlProperty CreateProperty(string name, Type fieldType, object value)
        {
            if (Object.ReferenceEquals(value, null))
                return null;

            if (Util.CheckGenericType(fieldType, typeof(Nullable<>)))
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
            else if (Util.CheckGenericType(fieldType, typeof(AllDirectionStyleValue<>)))
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

        public void BindProperty<T>(string name, Observable<T> variable)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (!StylePropertyNames.Any(n => string.Compare(n, name, true) == 0))
                throw new ArgumentException($"No style property with the name `{name}` exists.");

            if (variable == null)
                throw new ArgumentNullException(nameof(variable));

            _styleProperties[name].Bind<T>(variable);
        }

        internal void SetProperty<T>(string name, T value)
        {
            if (Object.ReferenceEquals(value, null)) // don't use "== null" here
            {
                _styleProperties.Remove(name);
                return;
            }

            if (_styleProperties.ContainsKey(name))
                _styleProperties[name].SetValue<T>(value);
            else
                _styleProperties.Add(name, CreateProperty<T>(name, value));
        }

        /// <summary>
        /// Retrieves the value of the given style property.
        /// 
        /// Some properties have the same meaning like:
        /// - Background.Color
        /// - BackgroundColor
        /// 
        /// If you pass the non-hierarchy version like
        /// BackgroundColor this value is returned.
        /// If it wasn't set to a valid value the default
        /// value for this style property is returned.
        /// 
        /// If you pass the hierarchy version like
        /// Background.Color this value is returned.
        /// If it wasn't set to a valid value the
        /// non-hierarchy version is checked like above
        /// and eventually the default value is returned
        /// if neither of the properties is set to a valid value.
        /// 
        /// This step is performed for any hierarchy level:
        /// So this is the order of checking for more than
        /// one sub-level:
        /// A.B.C.D -> AB.C.D -> ABC.D -> ABCD -> default value of A.B.C.D
        /// 
        /// The case of the name is ignored so the following are the same:
        /// backgroundcolor
        /// backgroundColor
        /// BackgroundColor
        /// BACKGROUNDCOLOR
        /// background.Color
        /// ...
        /// </summary>
        /// <param name="name">Full qualified name of the style property</param>
        /// <typeparam name="T">Type of the expected value</typeparam>
        /// <returns>The value of the style property</returns>
        public T Get<T>(string name)
        {
            return Get<T>(name, () => GetDefault<T>(name));
        }

        /// <summary>
        /// This is like <see cref="Get<T>(string)"/> but instead of
        /// returning the internal default value in case the property
        /// is not set, the given defaultValue is returned.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>(string name, T defaultValue)
        {
            return Get<T>(name, () => defaultValue);
        }

        private T Get<T>(string name, Func<T> defaultValueProvider)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            name = name.ToLower();

            if (!_styleProperties.ContainsKey(name))
            {
                int dotPosition = name.IndexOf('.');

                if (dotPosition == -1)
                {
                    return defaultValueProvider();
                }
                else
                {
                    return Get<T>(name.Remove(dotPosition, 1), defaultValueProvider);
                }
            }

            var property = _styleProperties[name];

            return property.HasValue ? property.ConvertTo<T>() : defaultValueProvider();
        }

        /// <summary>
        /// Retrieves the default value for the given style property.
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetDefault<T>(string name)
        {
            name = name.ToLower();

            if (!DefaultStyleProperties.ContainsKey(name))
                return default(T);

            var property = DefaultStyleProperties[name];

            return property.HasValue ? property.ConvertTo<T>() : default(T);
        }

        internal bool SetStyleProperty(string name, object value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (!StylePropertyNames.Any(n => string.Compare(n, name, true) == 0))
                throw new ArgumentException($"No style property with the name `{name}` exists.");

            if (!_styleProperties.ContainsKey(name))
            {
                var property = CreateProperty(name, DefaultStyleProperties[name].GetPropertyType(), value);

                if (property != null)
                {
                    _styleProperties.Add(name, property);
                    return true;
                }
            }

            return false;
        }
    }
}