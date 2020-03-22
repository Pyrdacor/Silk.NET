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
        Shapes, // colored shapes / custom drawings
    }

    internal enum LayerShape
    {
        Rect,
        Polygon
    }

    internal class RenderLayer : IDisposable
    {
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
        }

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

        readonly RenderBuffer renderBuffer = null;
        readonly Texture texture = null;
        readonly int layerIndex = 0;
        bool disposed = false;

        public RenderLayer(Layer layer, Texture texture, Color? colorKey = null, Color? colorOverlay = null)
        {
            renderBuffer = new RenderBuffer(layer == Layer.Shapes ? LayerShape.Polygon : LayerShape.Rect, layer == Layer.Images);

            Layer = layer;
            this.texture = texture;
            ColorKey = colorKey;
            ColorOverlay = colorOverlay;
            layerIndex = Misc.Round(Math.Log((int)layer, 2.0));
        }

        /*bool SupportZoom =>
            Layer != ...;*/

        public void Render()
        {
            if (!Visible)
                return;

            if (Layer == Layer.Images)
            {
                if (texture == null) // TODO: error?
                    return;

                var textureShader = TextureShader.Instance;

                textureShader.UpdateMatrices(SupportZoom);
                textureShader.SetSampler(0); // we use texture unit 0 -> see Gl.ActiveTexture below
                State.Gl.ActiveTexture(GLEnum.Texture0);
                texture.Bind();

                textureShader.SetAtlasSize((uint)texture.Width, (uint)texture.Height);
                textureShader.SetZ(Global.LayerBaseZ[layerIndex]);

                if (ColorKey == null)
                    textureShader.SetColorKey(1.0f, 0.0f, 1.0f);
                else
                    textureShader.SetColorKey(ColorKey.R / 255.0f, ColorKey.G / 255.0f, ColorKey.B / 255.0f);

                if (ColorOverlay == null)
                    textureShader.SetColorOverlay(1.0f, 1.0f, 1.0f, 1.0f);
                else
                    textureShader.SetColorOverlay(ColorOverlay.R / 255.0f, ColorOverlay.G / 255.0f, ColorOverlay.B / 255.0f, ColorOverlay.A / 255.0f);
            }
            else
            {
                var colorShader = ColorShader.Instance;

                colorShader.UpdateMatrices(SupportZoom);
                colorShader.SetZ(Global.LayerBaseZ[layerIndex]);
            }
            
            renderBuffer.Render();
        }

        public int GetDrawIndex(Sprite sprite)
        {
            return renderBuffer.GetDrawIndex(sprite, PositionTransformation, SizeTransformation);
        }

        public void FreeDrawIndex(int index)
        {
            renderBuffer.FreeDrawIndex(index);
        }

        public void UpdatePosition(int index, Sprite sprite)
        {
            renderBuffer.UpdatePosition(index, sprite, PositionTransformation, SizeTransformation);
        }

        public void UpdateTextureAtlasOffset(int index, Sprite sprite)
        {
            renderBuffer.UpdateTextureAtlasOffset(index, sprite);
        }

        public void UpdateDisplayLayer(int index, byte displayLayer)
        {
            renderBuffer.UpdateDisplayLayer(index, displayLayer);
        }

        public void TestNode(RenderNode node)
        {
            if (RenderNode.Shape != renderBuffer.Shape)
                throw new InvalidOperationException($"Only nodes with shape {Enum.GetName(typeof(LayerShape), renderBuffer.Shape)} are allowed for this layer.");
        }

        public void Dispose()
        {
            Dispose(true);
        }

        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    renderBuffer?.Dispose();
                    texture?.Dispose();
                    Visible = false;
                    disposed = true;
                }
            }
        }
    }
}
