using System.Drawing;

namespace Silk.NET.UI.Renderer.OpenGL
{
    internal class Shadow : Shape
    {
        private uint _blurRadius = 0;

        public Shadow(ControlRenderer controlRenderer,
            RenderDimensionReference renderDimensionReference,
            int x, int y, int width, int height, uint blurRadius)
            : base(controlRenderer, renderDimensionReference,
                controlRenderer.ShadowRenderLayer,
                new Point[4]
                {
                    new Point(x, y),
                    new Point(x + width, y),
                    new Point(x + width, y + height),
                    new Point(x, y + height)
                })
        {
            _blurRadius = blurRadius;
        }

        protected void UpdateBlurRadius()
        {
            if (_drawIndex.HasValue)
                Layer.UpdateBlurRadius(_drawIndex.Value, _blurRadius);
        }

        public uint BlurRadius
        {
            get => _blurRadius;
            set
            {
                if (_blurRadius == value)
                    return;

                _blurRadius = value;

                UpdateBlurRadius();
            }
        }
    }
}