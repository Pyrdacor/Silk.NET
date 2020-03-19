using System.Collections.Generic;
using System.Drawing;

namespace Silk.NET.UI.Common
{
    using Properties;

    public abstract class Control
    {
        private IntProperty width = new IntProperty("Width", 0);
        private IntProperty height = new IntProperty("Height", 0);
        private IntProperty x = new IntProperty("X", 0);
        private IntProperty y = new IntProperty("Y", 0);

        public string Id { get; internal set; }
        public List<string> Classes { get; } = new List<string>();
        internal ControlList InternalChildren { get; }
        public Component Parent { get; private set; }
        public int X
        {
            get => x.Value ?? 0;
            set => x.Value = value;
        }
        public int Y
        {
            get => y.Value ?? 0;
            set => y.Value = value;
        }
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
        public Point Position
        {
            get => new Point(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        public Size Size
        {
            get => new Size(Width, Height);
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
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
