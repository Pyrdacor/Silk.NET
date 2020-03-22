using System;
using System.Drawing;

namespace Silk.NET.UI.Renderer.OpenGL
{
    internal interface IRenderNode
    {
        void Delete();
    }

    internal abstract class RenderNode : IRenderNode
    {
        int x = short.MaxValue;
        int y = short.MaxValue;
        bool visible = false;
        RenderLayer layer = null;
        bool visibleRequest = false;
        bool deleted = false;
        bool notOnScreen = true;
        readonly RenderDimensionReference renderDimensionReference = null;

        protected RenderNode(LayerShape shape, int width, int height, RenderDimensionReference renderDimensionReference)
        {
            Shape = shape;
            Width = width;
            Height = height;
            this.renderDimensionReference = renderDimensionReference;
        }

        public LayerShape Shape { get; } = LayerShape.Rect;

        public bool Visible
        {
            get => visible && !deleted && !notOnScreen;
            set
            {
                if (deleted)
                    return;

                if (layer == null)
                {
                    visibleRequest = value;
                    visible = false;
                    return;
                }

                visibleRequest = false;

                if (visible == value)
                    return;

                visible = value;
                
                if (Visible)
                    AddToLayer();
                else if (!visible)
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
            get => layer;
            set
            {
                if (layer == value)
                    return;

                if (layer != null && Visible)
                    RemoveFromLayer();

                layer = value;

                if (layer != null && visibleRequest && !deleted)
                {
                    visible = true;
                    visibleRequest = false;
                    CheckOnScreen();
                }

                if (layer == null)
                {
                    visibleRequest = false;
                    visible = false;
                    notOnScreen = true;
                }

                if (layer != null && Visible)
                    AddToLayer();
            }
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

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
            bool oldNotOnScreen = notOnScreen;
            bool oldVisible = Visible;

            notOnScreen = !virtualScreen.IntersectsWith(new Rect(X, Y, Width, Height));

            if (oldNotOnScreen != notOnScreen)
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
            if (!deleted)
            {
                RemoveFromLayer();
                deleted = true;
                visible = false;
                visibleRequest = false;
            }
        }

        public int X
        {
            get => x;
            set
            {
                if (x == value)
                    return;

                x = value;

                if (!deleted)
                {
                    if (!CheckOnScreen())
                        UpdatePosition();
                }
            }
        }

        public int Y
        {
            get => y;
            set
            {
                if (y == value)
                    return;

                y = value;

                if (!deleted)
                {
                    if (!CheckOnScreen())
                        UpdatePosition();
                }
            }
        }
    }
}
