using System;
using System.Linq;
using System.Collections.Generic;

namespace Silk.NET.UI
{
    public abstract class Styles
    {
        private readonly Dictionary<Selector, Style> _styles = new Dictionary<Selector, Style>();

        protected void Add(Selector selector, Style style)
        {
            // styles that are set later with the same selector will override previous styles
            if (_styles.ContainsKey(selector))
                _styles[selector] = MergeStyles(_styles[selector], style);
            else
                _styles[selector] = style;
        }

        private Style MergeStyles(Style oldStyle, Style newStyle)
        {
            var mergedStyle = new Style();

            CopyFields(oldStyle, mergedStyle);
            CopyFields(newStyle, mergedStyle);

            return mergedStyle;
        }

        private static void CopyFields(Style source, Style target)
        {
            Type type = typeof(Style);
            var fields = type.GetFields();

            foreach (var field in fields)
            {
                var value = field.GetValue(source);

                if (value != null)
                    field.SetValue(target, value);
            }
        }

        private static object GetStylePropertyValue(object parent, string name)
        {
            var type = parent.GetType();

            if (Util.CheckGenericType(type, typeof(Nullable<>)))
                type = type.GenericTypeArguments[0];

            int dotIndex = name.IndexOf(".");
            
            if (dotIndex != -1)
            {
                var newParent = type.GetField(name.Substring(0, dotIndex)).GetValue(parent);

                if (newParent == null)
                    return null;

                return GetStylePropertyValue(newParent, name.Substring(dotIndex + 1));
            }

            return type.GetField(name).GetValue(parent);
        }

        private static IEnumerable<KeyValuePair<string, object>> EnumerateFields(Style style)
        {
            return ControlStyle.StylePropertyNames
                .Select(name => new KeyValuePair<string, object>(name, GetStylePropertyValue(style, name)))
                .Where(result => result.Value != null);
        }

        internal void Apply(Component component)
        {
            var styleList = _styles.ToList();
            styleList.Sort(new StyleComparer()); // sort by selector priority

            foreach (var style in styleList)
            {
                var matchingControls = component.FindMatchingControls(component, style.Key);

                foreach (var control in matchingControls)
                {
                    foreach (var field in EnumerateFields(style.Value))
                    {
                        control.Style.SetProperty(field.Key, field.Value);
                    }
                }
            }
        }

        private class StyleComparer : IComparer<KeyValuePair<Selector, Style>>
        {
            public int Compare(KeyValuePair<Selector, Style> lhs, KeyValuePair<Selector, Style> rhs)
            {
                // higher values win
                return rhs.Key.Priority.CompareTo(lhs.Key.Priority);
            }
        }
    }
}
