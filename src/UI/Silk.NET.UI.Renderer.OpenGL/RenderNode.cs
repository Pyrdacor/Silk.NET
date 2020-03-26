using System.Drawing;
using Silk.NET.OpenGL;

namespace Silk.NET.UI.Renderer.OpenGL
{
    internal interface IRenderNode
    {
        void Delete();
    }

    internal abstract class RenderNode : IRenderNode
    {
        private int _x = short.MaxValue;
        private int _y = short.MaxValue;
        private bool _visible = false;
        private RenderLayer _layer = null;
        private bool _visibleRequest = false;
        private bool _deleted = false;
        private bool _notOnScreen = true;
        private readonly RenderDimensionReference _renderDimensionReference = null;

        protected RenderNode(int width, int height, RenderDimensionReference renderDimensionReference)
        {
            Width = width;
            Height = height;
            _renderDimensionReference = renderDimensionReference;
        }

        public bool Visible
        {
            get => _visible && !_deleted && !_notOnScreen;
            set
            {
                if (_deleted)
                    return;

                if (_layer == null)
                {
                    _visibleRequest = value;
                    _visible = false;
                    return;
                }

                _visibleRequest = false;

                if (_visible == value)
                    return;

                _visible = value;
                
                if (Visible)
                    AddToLayer();
                else if (!_visible)
                    RemoveFromLayer();
            }
        }

        public Color Color
        {
            get;
            set;
        } = Color.White;

        public RenderLayer Layer
        {
            get => _layer;
            set
            {
                if (_layer == value)
                    return;

                if (_layer != null && Visible)
                    RemoveFromLayer();

                _layer = value;

                if (_layer != null && _visibleRequest && !_deleted)
                {
                    _visible = true;
                    _visibleRequest = false;
                    CheckOnScreen();
                }

                if (_layer == null)
                {
                    _visibleRequest = false;
                    _visible = false;
                    _notOnScreen = true;
                }

                if (_layer != null && Visible)
                    AddToLayer();
            }
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public abstract int VerticesPerNode { get; }
        public abstract PrimitiveType PrimitiveType { get; }

        public virtual void Resize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        protected abstract void AddToLayer();

        protected abstract void RemoveFromLayer();

        protected abstract void UpdatePosition();

        public abstract Point[] ProvideVertexPositions();

        protected abstract void UpdateDisplayLayer();

        bool CheckOnScreen()
        {
            bool oldNotOnScreen = _notOnScreen;
            bool oldVisible = Visible;

            _notOnScreen = !_renderDimensionReference.IntersectsWith(new Rectangle(X, Y, Width, Height));

            if (oldNotOnScreen != _notOnScreen)
            {
                if (oldVisible != Visible)
                {
                    if (Visible)
                        AddToLayer();
                    else
                        RemoveFromLayer();

                    return true; // handled
                }
            }

            return false;
        }

        public virtual void Delete()
        {
            if (!_deleted)
            {
                RemoveFromLayer();
                _deleted = true;
                _visible = false;
                _visibleRequest = false;
            }
        }

        public int X
        {
            get => _x;
            set
            {
                if (_x == value)
                    return;

                _x = value;

                if (!_deleted)
                {
                    if (!CheckOnScreen())
                        UpdatePosition();
                }
            }
        }

        public int Y
        {
            get => _y;
            set
            {
                if (_y == value)
                    return;

                _y = value;

                if (!_deleted)
                {
                    if (!CheckOnScreen())
                        UpdatePosition();
                }
            }
        }
    }
}
