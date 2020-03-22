using System;
using System.Drawing;

namespace Silk.NET.UI.Renderer.OpenGL
{
    public class RenderDimensionReference
    {
        public int Width { get; private set; } = 0;
        public int Height { get; private set; } = 0;

        public void SetDimensions(int width, int height)
        {
            if (this.width != width || this.height != height)
            {
                this.width = width;
                this.height = height;
                DimensionsChanged?.Invoke();
            }
        }

        internal event Action DimensionsChanged;

        public bool IntersectsWith(Rectangle rect)
        {
            return new Rectangle(0, 0, Width, Height).IntersectsWith(rect);
        }
    }
}