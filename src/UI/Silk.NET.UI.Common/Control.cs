using System;
using System.Collections.Generic;
using System.Drawing;

namespace Silk.NET.UI
{
    using Input.Common;

    public abstract class Control
    {
        private Component _parent;
        internal virtual ControlRenderer ControlRenderer
        {
            get => Parent != null ? Parent.ControlRenderer : null;
        }
        internal virtual InputEventManager InputEventManager
        {
            get => Parent != null ? Parent.InputEventManager : null;
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
        /// <summary>
        /// Location relative to the parent control.
        /// </summary>
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
        /// <summary>
        /// Absolute location which is relative to the main view.
        /// </summary>
        public Point AbsoluteLocation
        {
            get => Parent == null ? Location : Parent.AbsoluteLocation.Add(Location);
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
        public Rectangle ClientRectangle
        {
            get => new Rectangle(Location, Size);
            set
            {
                Location = value.Location;
                Size = value.Size;
            }
        }
        /// <summary>
        /// Absolute rectangle which is positioned relative to the main view.
        /// </summary>
        public Rectangle AbsoluteRectangle
        {
            get => new Rectangle(AbsoluteLocation, Size);
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


        #region Events

        public event PropagatedEventHandler ParentChanged;
        public event PropagatedEventHandler VisibilityChanged;
        public event PropagatedEventHandler EnabledChanged;
        public event PropagatedEventHandler FocusedChanged;
        public event PropagatedEventHandler GainFocus;
        public event PropagatedEventHandler LostFocus;
        public event PropagatedEventHandler HoveredChanged;
        public event PropagatedEventHandler Enter;
        public event PropagatedEventHandler Leave;
        public event MouseMoveEventHandler MouseMove;
        public event MouseButtonEventHandler MouseDown;
        public event MouseButtonEventHandler MouseUp;
        public event MouseButtonEventHandler MouseClick;
        public event MouseButtonEventHandler MouseDoubleClick;
        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;


        internal virtual void OnParentChanged()
        {
            ParentChanged?.Invoke(this, new PropagatedEventArgs());
            CheckStyleChanges();
        }

        internal virtual void OnVisibilityChanged()
        {
            VisibilityChanged?.Invoke(this, new PropagatedEventArgs());
            CheckStyleChanges();
        }

        internal virtual void OnEnabledChanged()
        {
            EnabledChanged?.Invoke(this, new PropagatedEventArgs());
            CheckStyleChanges();
        }

        internal virtual void OnFocusedChanged()
        {
            FocusedChanged?.Invoke(this, new PropagatedEventArgs());

            if (Focused)
                GainFocus?.Invoke(this, new PropagatedEventArgs());
            else
                LostFocus?.Invoke(this, new PropagatedEventArgs());

            CheckStyleChanges();
        }

        internal virtual void OnHoveredChanged()
        {
            HoveredChanged?.Invoke(this, new PropagatedEventArgs());

            if (Hovered)
                Enter?.Invoke(this, new PropagatedEventArgs());
            else
                Leave?.Invoke(this, new PropagatedEventArgs());

            CheckStyleChanges();
        }

        internal void OnMouseMove(MouseMoveEventArgs args)
        {
            MouseMove?.Invoke(this, args);

            Hovered = ClientRectangle.Contains(args.X, args.Y);
        }

        internal void OnMouseDown(MouseButtonEventArgs args)
        {
            MouseDown?.Invoke(this, args);
        }

        internal void OnMouseUp(MouseButtonEventArgs args)
        {
            MouseUp?.Invoke(this, args);
        }

        internal void OnMouseClick(MouseButtonEventArgs args)
        {
            MouseClick?.Invoke(this, args);
        }

        internal void OnMouseDoubleClick(MouseButtonEventArgs args)
        {
            MouseDoubleClick?.Invoke(this, args);
        }

        internal void OnKeyDown(KeyEventArgs args)
        {
            KeyDown?.Invoke(this, args);
        }

        internal void OnKeyUp(KeyEventArgs args)
        {
            KeyUp?.Invoke(this, args);
        }

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

        internal void DestroyControl()
        {
            foreach (var child in InternalChildren)
                child.DestroyControl();

            OnDestroy();

            if (Parent != null)
                Parent.InternalChildren.Remove(this);

            _x.ValueChanged -= OnPositionChanged;
            _y.ValueChanged -= OnPositionChanged;
            _width.ValueChanged -= OnSizeChanged;
            _height.ValueChanged -= OnSizeChanged;

            InputEventManager?.UnregisterControl(this);

            DestroyView();
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

        internal void InitControl()
        {
            InputEventManager?.RegisterControl(this);

            // TODO
            OnInit();

            _visible.ValueChanged += OnVisibilityChanged;
            _enabled.ValueChanged += OnEnabledChanged;
            _focused.ValueChanged += OnFocusedChanged;
            _hovered.ValueChanged += OnHoveredChanged;

            OnAfterContentInit();

            InitView();

            foreach (var child in InternalChildren)
                child.InitControl();

            OnAfterViewInit();
        }

        internal virtual void InitView()
        {

        }

        internal virtual void DestroyView()
        {

        }

        internal void RenderControl()
        {
            var args = new RenderEventArgs(ControlRenderer);
            OnRender(args);
            Render?.Invoke(this, args);
        }

        internal abstract void CheckStyleChanges();

        protected void OverrideStyle<T>(string name, T value)
        {
            // AllDirectionStyleValue<ColorValue> will fail to convert from/to System.Drawing.Color
            // so we will convert colors to a ColorValue here.
            if (typeof(T) == typeof(System.Drawing.Color))
            {
                OverrideStyle(name, new ColorValue((Color)(object)value));
                return;
            }

            Style.SetProperty(name, value);
        }

        protected void OverrideStyleIfUndefined<T>(string name, T value)
        {
            // AllDirectionStyleValue<ColorValue> will fail to convert from/to System.Drawing.Color
            // so we will convert colors to a ColorValue here.
            if (typeof(T) == typeof(System.Drawing.Color))
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
