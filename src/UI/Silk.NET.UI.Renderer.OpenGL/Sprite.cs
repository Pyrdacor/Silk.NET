using System.Drawing;

namespace Silk.NET.UI.Renderer.OpenGL
{
    /// <summary>
    /// A sprite has a fixed size and an offset into the layer's texture atlas or no texture at all.
    /// The layer will sort sprites by size and then by the texture atlas offset.
    /// </summary>
    internal class Sprite : RenderNode
    {
        protected int? drawIndex = null;
        Point textureAtlasOffset = null;
        byte displayLayer = 0;

        public Sprite(int width, int height, int textureAtlasX, int textureAtlasY, RenderDimensionReference renderDimensionReference)
            : base(Shape.Rect, width, height, renderDimensionReference)
        {
            textureAtlasOffset = new Point(textureAtlasX, textureAtlasY);
        }

        public Sprite(int width, int height, RenderDimensionReference renderDimensionReference)
            : base(Shape.Rect, width, height, renderDimensionReference)
        {

        }

        public Point TextureAtlasOffset
        {
            get => textureAtlasOffset;
            set
            {
                if (textureAtlasOffset == value)
                    return;

                textureAtlasOffset = new Point(value);

                UpdateTextureAtlasOffset();
            }
        }

        public byte DisplayLayer
        {
            get => displayLayer;
            set
            {
                if (displayLayer == value)
                    return;

                displayLayer = value;

                UpdateDisplayLayer();
            }
        }

        protected override void AddToLayer()
        {
            drawIndex = Layer.GetDrawIndex(this);
        }

        protected override void RemoveFromLayer()
        {
            if (drawIndex.HasValue)
            {
                Layer.FreeDrawIndex(drawIndex.Value);
                drawIndex = null;
            }
        }

        protected override void UpdatePosition()
        {
            if (drawIndex.HasValue)
                Layer.UpdatePosition(drawIndex.Value, this);
        }

        protected virtual void UpdateTextureAtlasOffset()
        {
            if (drawIndex.HasValue)
                Layer.UpdateTextureAtlasOffset(drawIndex.Value, this);
        }

        public override Point[] ProvideVertexPositions()
        {
            return new Point[]
            {
                { X, Y },
                { X + Width, Y },
                { X + Width, Y + Height },
                { X, Y + Height }
            };
        }

        public override void Resize(int width, int height)
        {
            if (Width == width && Height == height)
                return;

            base.Resize(width, height);

            UpdatePosition();
            UpdateTextureAtlasOffset();
        }

        protected override void UpdateDisplayLayer()
        {
            if (drawIndex.HasValue)
                Layer.UpdateDisplayLayer(drawIndex.Value, displayLayer);
        }
    }
}
