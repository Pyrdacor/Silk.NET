using System.Collections.Generic;

namespace Silk.NET.UI.Common
{
    public abstract class Control
    {
        public string? Id { get; internal set; }
        public List<string> Classes { get; } = new List<string>();
        internal ControlList InternalChildren { get; }

        protected Control(string? id)
        {
            Id = id;
            InternalChildren = new ControlList(this);
        }
    }

    public static class ControlExtensions
    {
        public static Control WithClasses(this Control control, params string[] classes)
        {
            control.Classes.AddRange(classes);
            return control;
        }

        public static void AddTo(this Control control, Component component)
        {
            component.InternalChildren.Add(control);
        }
    }
}
