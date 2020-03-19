using System.Collections.Generic;

namespace Silk.NET.UI.Common
{
    using Properties;

    public abstract class Control
    {
        private IntProperty width = new IntProperty("Width", 0);
        private IntProperty height = new IntProperty("Height", 0);

        public string Id { get; internal set; }
        public List<string> Classes { get; } = new List<string>();
        internal ControlList InternalChildren { get; }
        public Component Parent { get; private set; }
        public int Width
        {
            get => width.Value ?? 0;
            set => width.Value = value;
        }
        public int Height
        {
            get => height.Value ?? 0;
            set => height.Value = value;
        }

        protected Control(string id)
        {
            Id = id;
            InternalChildren = new ControlList(this);
        }

        internal virtual void Destroy()
        {
            if (Parent != null)
                Parent.InternalChildren.Remove(this);

            // TODO: remove from renderer?
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
