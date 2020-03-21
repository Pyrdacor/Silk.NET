using System;
using System.Collections.Generic;
using System.Drawing;

namespace Silk.NET.UI.Common
{
    using Properties;

    public abstract class Control
    {
        private Component parent;
        private ControlRenderer controlRenderer;
        internal virtual IControlRenderer ControlRenderer
        {
            get => Parent != null ? Parent.ControlRenderer : null;
        }
        private readonly ControlStyle style = new ControlStyle();
        internal ControlStyle Style => style;


        #region Lifecycle Hooks

        public event EventHandler Init;
        public event EventHandler AfterContentInit;
        public event EventHandler AfterViewInit;
        public event RenderEventHandler Render;
        public event EventHandler Destroy;

        protected virtual void OnInit()
        {
            Init?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnAfterContentInit()
        {
            AfterContentInit?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnAfterViewInit()
        {
            AfterViewInit?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnDestroy()
        {
            Destroy?.Invoke(this, EventArgs.Empty);
        }

        #endregion


        #region Control Properties

        private Dictionary<string, IControlProperty> controlProperties = new Dictionary<string, IControlProperty>();        

        #region State

        private BoolProperty visible = new BoolProperty(nameof(Visible), true);
        private BoolProperty enabled = new BoolProperty(nameof(Enabled), true);

        public bool Visible
        {
            get
            {
                if (Parent == null && !(this is RootComponent))
                    return false;

                return visible.Value ?? true;
            }
            set => visible.Value = value;
        }
        public bool Enabled
        {
            get => enabled.Value ?? true;
            set => enabled.Value = value;
        }

        #endregion

        #region Metrics

        private IntProperty width = new IntProperty(nameof(Width), 0);
        private IntProperty height = new IntProperty(nameof(Height), 0);
        private IntProperty x = new IntProperty(nameof(X), 0);
        private IntProperty y = new IntProperty(nameof(Y), 0);

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
        public Point Location
        {
            get => new Point(X, Y);
            set
            {
                int oldX = X;
                int oldY = Y;
                using (new DisableChangeEventContext(x, y))
                {
                    X = value.X;
                    Y = value.Y;
                }
                if (oldX != X || oldY != Y)
                    OnPositionChanged();
            }
        }
        public Size Size
        {
            get => new Size(Width, Height);
            set
            {
                int oldWidth = Width;
                int oldHeight = Height;
                using (new DisableChangeEventContext(width, height))
                {
                    Width = value.Width;
                    Height = value.Height;
                }
                if (oldWidth != Width || oldHeight != Height)
                    OnSizeChanged();
            }
        }
        /// <summary>
        /// Rectangular area relative to the parent control.
        /// </summary>
        /// <value></value>
        public Rectangle ClientRectangle
        {
            get => new Rectangle(Location, Size);
            set
            {
                Location = value.Location;
                Size = value.Size;
            }
        }

        protected void OnPositionChanged()
        {
            PositionChanged?.Invoke(this, EventArgs.Empty);
        }

        protected void OnSizeChanged()
        {
            SizeChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler PositionChanged;
        public event EventHandler SizeChanged;

        #endregion

        #endregion


        public string Id { get; internal set; }
        public List<string> Classes { get; } = new List<string>();
        internal ControlList InternalChildren { get; }
        public Component Parent
        {
            get => parent;
            internal set
            {
                if (parent != value)
                {
                    parent = value;
                    OnParentChanged();
                }
            }
        }
        public event EventHandler ParentChanged;
        private void OnParentChanged()
        {
            ParentChanged?.Invoke(this, EventArgs.Empty);
        }

        protected Control(string id)
        {
            Id = id;
            InternalChildren = new ControlList(this);
            controlRenderer = new ControlRenderer(this, ControlRenderer);

            // Register control properties
            RegisterControlProperty(x);
            RegisterControlProperty(y);
            RegisterControlProperty(width);
            RegisterControlProperty(height);

            x.ValueChanged += OnPositionChanged;
            y.ValueChanged += OnPositionChanged;
            width.ValueChanged += OnSizeChanged;
            height.ValueChanged += OnSizeChanged;
        }

        internal virtual void DestroyControl()
        {
            OnDestroy();

            if (Parent != null)
                Parent.InternalChildren.Remove(this);

            x.ValueChanged -= OnPositionChanged;
            y.ValueChanged -= OnPositionChanged;
            width.ValueChanged -= OnSizeChanged;
            height.ValueChanged -= OnSizeChanged;

            // TODO: remove from renderer?
        }

        protected void RegisterControlProperty<T>(ControlProperty<T> property)
        {
            controlProperties.Add(property.Name, property);
        }

        protected virtual void OnRender(RenderEventArgs args)
        {
            Render?.Invoke(this, args);
        }

        // TODO: the two following methods have to be called from the component manager

        internal void InitControl()
        {
            // TODO
            OnInit();

            OnAfterContentInit();
            InitView();
            OnAfterViewInit();
        }

        internal virtual void InitView()
        {

        }

        internal void RenderControl()
        {
            OnRender(new RenderEventArgs(controlRenderer));
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
            control.Parent = component;
        }
    }
}
