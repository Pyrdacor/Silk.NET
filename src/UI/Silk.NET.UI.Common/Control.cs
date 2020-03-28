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
        internal protected ControlStyle Style => style;


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
        private BoolProperty _hovered = new BoolProperty(nameof(Hovered), false);
        private BoolProperty _focused = new BoolProperty(nameof(Focused), false);

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
        public bool Hovered
        {
            get => Enabled && Visible && _hovered.HasValue && _hovered.Value.Value;
            set => _hovered.Value = value && Enabled && Visible;
        }
        public bool Focused
        {
            get => Enabled && Visible && _focused.HasValue && _focused.Value.Value;
            set => _focused.Value = value && Enabled && Visible;
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
        public List<string> Classes { get; } = new List<string>(); // TODO: make it readonly?
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

        protected void OverrideStyle<T>(string name, T value)
        {
            Style.SetProperty(name, value);
        }

        protected void OverrideStyleIfUndefined<T>(string name, T value)
        {
            var type = typeof(T);

            // AllDirectionStyleValue<ColorValue> will fail to convert from/to System.Drawing.Color
            // so we will convert colors to a ColorValue here.
            if (type == typeof(System.Drawing.Color))
            {
                OverrideStyleIfUndefined(name, new ColorValue((Color)(object)value));
                return;
            }

            Style.SetProperty(name, Style.Get<T>(name, value));
        }

        private static void DrawSetBorder(ref int? renderRef, ControlRenderer renderer, StlyeDirection direction,
            bool inset, ColorValue color, int lineSize, Rectangle rectangle)
        {
            var points = direction switch
            {
                StlyeDirection.Top => new Point[4]
                {
                    new Point(rectangle.Left, rectangle.Top),
                    new Point(rectangle.Right, rectangle.Top),
                    new Point(rectangle.Right - lineSize, rectangle.Top + lineSize),
                    new Point(rectangle.Left + lineSize, rectangle.Top + lineSize),                    
                },
                StlyeDirection.Right => new Point[4]
                {
                    new Point(rectangle.Right - lineSize, rectangle.Top + lineSize),
                    new Point(rectangle.Right, rectangle.Top),
                    new Point(rectangle.Right, rectangle.Bottom),
                    new Point(rectangle.Right - lineSize, rectangle.Bottom - lineSize),
                },
                StlyeDirection.Bottom => new Point[4]
                {
                    new Point(rectangle.Left + lineSize, rectangle.Bottom - lineSize),
                    new Point(rectangle.Right - lineSize, rectangle.Bottom - lineSize),
                    new Point(rectangle.Right, rectangle.Bottom),
                    new Point(rectangle.Left, rectangle.Bottom),                    
                },
                StlyeDirection.Left => new Point[4]
                {
                    new Point(rectangle.Left, rectangle.Top),
                    new Point(rectangle.Left + lineSize, rectangle.Top + lineSize),
                    new Point(rectangle.Left + lineSize, rectangle.Bottom - lineSize),
                    new Point(rectangle.Left, rectangle.Bottom),
                },
                _ => throw new ArgumentException("Invalid style direction.")
            };

            ColorValue drawColor;

            if (inset)
                drawColor = (int)direction % 3 == 0 ? color.Darken(0.5f) : color.Lighten(0.25f);
            else
                drawColor = (int)direction % 3 == 0 ? color.Lighten(0.25f) : color.Darken(0.5f);

            renderRef = renderer.FillPolygon(renderRef, drawColor, points);
        }

        protected static void DrawBorder(ref int? renderRef, ControlRenderer renderer, StlyeDirection direction,
            BorderLineStyle lineStyle, ColorValue color, int lineSize, Rectangle rectangle)
        {
            if (lineStyle == BorderLineStyle.Inset || lineStyle == BorderLineStyle.Outset)
            {
                DrawSetBorder(ref renderRef, renderer, direction, lineStyle == BorderLineStyle.Inset,
                    color, lineSize, rectangle);
                return;
            }

            int x = direction switch
            {
                StlyeDirection.Right => rectangle.X + rectangle.Width - lineSize,
                _ => rectangle.X
            };
            int y = direction switch
            {
                StlyeDirection.Top => rectangle.Y,
                StlyeDirection.Bottom => rectangle.Y + rectangle.Height - lineSize,
                _ => lineSize
            };
            int width = (int)direction % 2 == 0 ? rectangle.Width : lineSize;
            int height = (int)direction % 2 == 0 ? lineSize : rectangle.Height - 2 * lineSize;

            switch (lineStyle)
            {
                case BorderLineStyle.Solid:
                    renderRef = renderer.DrawRectangleLine(renderRef, x, y, width, height, color, LineStyle.Solid);
                    break;
                case BorderLineStyle.Dotted:
                    renderRef = renderer.DrawRectangleLine(renderRef, x, y, width, height, color, LineStyle.Dotted);
                    break;
                case BorderLineStyle.Dashed:
                    renderRef = renderer.DrawRectangleLine(renderRef, x, y, width, height, color, LineStyle.Dashed);
                    break;
                case BorderLineStyle.Double:
                case BorderLineStyle.Groove:
                case BorderLineStyle.Ridge:
                    // TODO
                    break;
                case BorderLineStyle.None:
                default:
                    break;
            }
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
