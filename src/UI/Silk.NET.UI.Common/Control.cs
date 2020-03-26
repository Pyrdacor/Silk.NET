using System;
using System.Collections.Generic;
using System.Drawing;

namespace Silk.NET.UI
{
    public abstract class Control
    {
        private Component _parent;
        internal virtual ControlRenderer ControlRenderer
        {
            get => Parent != null ? Parent.ControlRenderer : null;
        }
        private readonly ControlStyle style = new ControlStyle();
        public ControlStyle Style => style;


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

        private Dictionary<string, IControlProperty> _controlProperties = new Dictionary<string, IControlProperty>();        

        #region State

        private BoolProperty _visible = new BoolProperty(nameof(Visible), true);
        private BoolProperty _enabled = new BoolProperty(nameof(Enabled), true);

        public bool Visible
        {
            get
            {
                if (Parent == null && !(this is RootComponent))
                    return false;

                return _visible.Value ?? true;
            }
            set => _visible.Value = value;
        }
        public bool Enabled
        {
            get => _enabled.Value ?? true;
            set => _enabled.Value = value;
        }

        #endregion

        #region Metrics

        private IntProperty _width = new IntProperty(nameof(Width), 0);
        private IntProperty _height = new IntProperty(nameof(Height), 0);
        private IntProperty _x = new IntProperty(nameof(X), 0);
        private IntProperty _y = new IntProperty(nameof(Y), 0);

        public int X
        {
            get => _x.Value ?? 0;
            set => _x.Value = value;
        }
        public int Y
        {
            get => _y.Value ?? 0;
            set => _y.Value = value;
        }
        public int Width
        {
            get => _width.Value ?? 0;
            set => _width.Value = value;
        }
        public int Height
        {
            get => _height.Value ?? 0;
            set => _height.Value = value;
        }
        public Point Location
        {
            get => new Point(X, Y);
            set
            {
                int oldX = X;
                int oldY = Y;
                using (new DisableChangeEventContext(_x, _y))
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
                using (new DisableChangeEventContext(_width, _height))
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
            get => _parent;
            internal set
            {
                if (_parent != value)
                {
                    _parent = value;
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

            // Register control properties
            RegisterControlProperty(_x);
            RegisterControlProperty(_y);
            RegisterControlProperty(_width);
            RegisterControlProperty(_height);

            _x.ValueChanged += OnPositionChanged;
            _y.ValueChanged += OnPositionChanged;
            _width.ValueChanged += OnSizeChanged;
            _height.ValueChanged += OnSizeChanged;
        }

        internal virtual void DestroyControl()
        {
            OnDestroy();

            if (Parent != null)
                Parent.InternalChildren.Remove(this);

            _x.ValueChanged -= OnPositionChanged;
            _y.ValueChanged -= OnPositionChanged;
            _width.ValueChanged -= OnSizeChanged;
            _height.ValueChanged -= OnSizeChanged;

            // TODO: remove from renderer?
        }

        protected void RegisterControlProperty<T>(ControlProperty<T> property)
        {
            _controlProperties.Add(property.Name, property);
        }

        protected virtual void OnRender(RenderEventArgs args)
        {
            foreach (var child in InternalChildren)
                child.OnRender(args);
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
            var args = new RenderEventArgs(ControlRenderer);
            OnRender(args);
            Render?.Invoke(this, args);
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
