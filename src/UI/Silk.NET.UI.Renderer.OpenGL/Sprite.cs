using System.Drawing;
using Silk.NET.OpenGL;

namespace Silk.NET.UI.Renderer.OpenGL
{
    /// <summary>
    /// A sprite has a fixed size and an offset into the layer's texture atlas or no texture at all.
    /// The layer will sort sprites by size and then by the texture atlas offset.
    /// </summary>
    internal class Sprite : RenderNode
    {
        protected int? _drawIndex = null;
        private Point? _textureAtlasOffset = null;
        private uint _displayLayer = 0;

        public Sprite(int width, int height, int textureAtlasX, int textureAtlasY, RenderDimensionReference renderDimensionReference)
            : base(width, height, renderDimensionReference)
        {
            _textureAtlasOffset = new Point(textureAtlasX, textureAtlasY);
        }

        public Sprite(int width, int height, RenderDimensionReference renderDimensionReference)
            : base(width, height, renderDimensionReference)
        {

        }

        public Point? TextureAtlasOffset
        {
            get => _textureAtlasOffset;
            set
            {
                if (_textureAtlasOffset == value)
                    return;

                _textureAtlasOffset = value;

                UpdateTextureAtlasOffset();
            }
        }

        public uint DisplayLayer
        {
            get => _displayLayer;
            set
            {
                if (_displayLayer == value)
                    return;

                _displayLayer = value;

                UpdateDisplayLayer();
            }
        }

        public override int VerticesPerNode => 6; // 2 triangles with 3 vertices each
        public override PrimitiveType PrimitiveType => PrimitiveType.Triangles;

        protected override void AddToLayer()
        {
            _drawIndex = Layer.GetDrawIndex(this);
        }

        protected override void RemoveFromLayer()
        {
            if (_drawIndex.HasValue)
            {
                Layer.FreeDrawIndex(_drawIndex.Value);
                _drawIndex = null;
            }
        }

        protected override void UpdatePosition()
        {
            if (_drawIndex.HasValue)
                Layer.UpdatePosition(_drawIndex.Value, this);
        }

        protected virtual void UpdateTextureAtlasOffset()
        {
            if (_drawIndex.HasValue && _textureAtlasOffset != null)
                Layer.UpdateTextureAtlasOffset(_drawIndex.Value, this);
        }

        public override Point[] ProvideVertexPositions()
        {
            return new Point[]
            {
                new Point(X, Y),
                new Point(X + Width, Y),
                new Point(X + Width, Y + Height),
                new Point(X, Y + Height)
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
            if (_drawIndex.HasValue)
                Layer.UpdateDisplayLayer(_drawIndex.Value, _displayLayer);
        }
    }
}
