using System;
using System.Drawing;
using Silk.NET.OpenGL;

namespace Silk.NET.UI.Renderer.OpenGL
{
    public delegate Point PositionTransformation(Point position);
    public delegate Size SizeTransformation(Size size);

    internal enum Layer
    {
        None,
        Controls, // controls (colors only)
        Images, // images
        Triangles, // colored triangle shapes / custom drawings
        Ellipsis, // colored elliptic shapes / custom drawings
        RoundRects, // colored rounded rect shapes / custom drawings
    }

    internal class RenderLayer : IDisposable
    {
        private bool _disposed = false;
        private Texture _texture = null;
        private readonly RenderBuffer _renderBuffer = null;

        public Layer Layer { get; } = Layer.None;

        public Color? ColorKey
        {
            get;
            set;
        } = null;

        public Color? ColorOverlay
        {
            get;
            set;
        } = null;

        public bool Visible
        {
            get;
            set;
        } = true;

        public PositionTransformation PositionTransformation
        {
            get;
            set;
        } = null;

        public SizeTransformation SizeTransformation
        {
            get;
            set;
        } = null;

        public float Z
        {
            get;
            set;
        } = 0.0f;


        public RenderLayer(Layer layer, Texture texture, Color? colorKey = null, Color? colorOverlay = null)
        {
            if (layer == Layer.None)
                throw new ArgumentException($"Layer `{nameof(Layer.None)}` can not be used as a type of real render layers.");

            _renderBuffer = new RenderBuffer(layer == Layer.Images,
                layer switch
                {
                    Layer.Triangles => Shape.TriangleVertices,
                    Layer.Ellipsis => Shape.EllipseVertices,
                    Layer.RoundRects => Shape.RoundRectVertices,
                    _ => 6 // 2 triangles with 3 vertices each
                },
                layer switch
                {
                    Layer.Ellipsis => PrimitiveType.TriangleFan,
                    Layer.RoundRects => PrimitiveType.TriangleFan,
                    _ => PrimitiveType.Triangles
                });

            Layer = layer;
            _texture = texture;
            ColorKey = colorKey;
            ColorOverlay = colorOverlay;
        }

        public bool SupportZoom => false; // TODO

        public void Render()
        {
            if (!Visible)
                return;

            if (Layer == Layer.Images)
            {
                if (_texture == null) // TODO: error?
                    return;

                var textureShader = TextureShader.Instance;

                textureShader.UpdateMatrices(SupportZoom);
                textureShader.SetSampler(0); // we use texture unit 0 -> see Gl.ActiveTexture below
                State.Gl.ActiveTexture(GLEnum.Texture0);
                _texture.Bind();

                textureShader.SetAtlasSize((uint)_texture.Width, (uint)_texture.Height);
                textureShader.SetZ(Z);

                if (ColorKey == null)
                    textureShader.SetColorKey(1.0f, 0.0f, 1.0f);
                else
                    textureShader.SetColorKey(ColorKey.Value.R / 255.0f, ColorKey.Value.G / 255.0f, ColorKey.Value.B / 255.0f);

                if (ColorOverlay == null)
                    textureShader.SetColorOverlay(1.0f, 1.0f, 1.0f, 1.0f);
                else
                    textureShader.SetColorOverlay(ColorOverlay.Value.R / 255.0f, ColorOverlay.Value.G / 255.0f, ColorOverlay.Value.B / 255.0f, ColorOverlay.Value.A / 255.0f);
            }
            else
            {
                var colorShader = ColorShader.Instance;

                colorShader.UpdateMatrices(SupportZoom);
                colorShader.SetZ(Z);
            }
            
            _renderBuffer.Render();
        }

        public int GetDrawIndex(Sprite sprite)
        {
            return _renderBuffer.GetDrawIndex(sprite, PositionTransformation, SizeTransformation);
        }

        public int GetDrawIndex(Shape shape)
        {
            return _renderBuffer.GetDrawIndex(shape, PositionTransformation, SizeTransformation);
        }

        public void FreeDrawIndex(int index)
        {
            _renderBuffer.FreeDrawIndex(index);
        }

        public void UpdatePosition(int index, Sprite sprite)
        {
            _renderBuffer.UpdatePosition(index, sprite, PositionTransformation, SizeTransformation);
        }

        public void UpdatePosition(int index, Shape shape)
        {
            _renderBuffer.UpdatePosition(index, shape, PositionTransformation, SizeTransformation);
        }

        public void UpdateTextureAtlasOffset(int index, Sprite sprite)
        {
            _renderBuffer.UpdateTextureAtlasOffset(index, sprite);
        }

        public void UpdateDisplayLayer(int index, uint displayLayer)
        {
            _renderBuffer.UpdateDisplayLayer(index, displayLayer);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _renderBuffer?.Dispose();
                    _texture?.Dispose();
                    Visible = false;
                    _disposed = true;
                }
            }
        }
    }
}
