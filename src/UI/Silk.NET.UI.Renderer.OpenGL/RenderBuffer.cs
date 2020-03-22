using System;
using System.Drawing;
using Silk.NET.OpenGL;

namespace Silk.NET.UI.Renderer.OpenGL
{
    internal class RenderBuffer : IDisposable
    {
        bool disposed = false;

        readonly VertexArrayObject vertexArrayObject = null;
        readonly PositionBuffer positionBuffer = null;
        readonly PositionBuffer textureAtlasOffsetBuffer = null;
        readonly ColorBuffer colorBuffer = null;
        readonly LayerBuffer layerBuffer = null;
        readonly IndexBuffer indexBuffer = null;

        public RenderBuffer(LayerShape shape, bool supportTextures)
        {
            // TODO: shape

            vertexArrayObject = new VertexArrayObject(TextureShader.Instance.ShaderProgram);

            indexBuffer = new IndexBuffer();
            positionBuffer = new PositionBuffer(false);            
            colorBuffer = new ColorBuffer(true);
            layerBuffer = new LayerBuffer(true);

            vertexArrayObject.AddBuffer("index", indexBuffer);
            vertexArrayObject.AddBuffer(ColorShader.DefaultPositionName, positionBuffer);
            vertexArrayObject.AddBuffer(TextureShader.DefaultColorOverlayName, colorBuffer);
            vertexArrayObject.AddBuffer(ColorShader.DefaultLayerName, layerBuffer);

            if (supportTextures)
            {
                textureAtlasOffsetBuffer = new PositionBuffer(false);
                vertexArrayObject.AddBuffer(TextureShader.DefaultTexCoordName, textureAtlasOffsetBuffer);
            }
        }

        public int GetDrawIndex(Sprite sprite, PositionTransformation positionTransformation,
            SizeTransformation sizeTransformation)
        {
            var position = new Point(sprite.X, sprite.Y);
            var size = new Size(sprite.Width, sprite.Height);

            if (positionTransformation != null)
                position = positionTransformation(position);

            if (sizeTransformation != null)
                size = sizeTransformation(size);

            int index = positionBuffer.Add((short)position.X, (short)position.Y);
            positionBuffer.Add((short)(position.X + size.Width), (short)position.Y, index + 1);
            positionBuffer.Add((short)(position.X + size.Width), (short)(position.Y + size.Height), index + 2);
            positionBuffer.Add((short)position.X, (short)(position.Y + size.Height), index + 3);

            indexBuffer.InsertQuad(index / 4);

            int colorBufferIndex = colorBuffer.Add(sprite.Color);

            if (colorBufferIndex != index)
                throw new IndexOutOfRangeException("Invalid color buffer index");

            colorBuffer.Add(sprite.Color, colorBufferIndex + 1);
            colorBuffer.Add(sprite.Color, colorBufferIndex + 2);
            colorBuffer.Add(sprite.Color, colorBufferIndex + 3);

            int textureAtlasOffsetBufferIndex = textureAtlasOffsetBuffer.Add((short)sprite.TextureAtlasOffset.X, (short)sprite.TextureAtlasOffset.Y);

            if (textureAtlasOffsetBufferIndex != index)
                throw new IndexOutOfRangeException("Invalid texture atlas offset buffer index");

            textureAtlasOffsetBuffer.Add((short)(sprite.TextureAtlasOffset.X + sprite.Width), (short)sprite.TextureAtlasOffset.Y, textureAtlasOffsetBufferIndex + 1);
            textureAtlasOffsetBuffer.Add((short)(sprite.TextureAtlasOffset.X + sprite.Width), (short)(sprite.TextureAtlasOffset.Y + sprite.Height), textureAtlasOffsetBufferIndex + 2);
            textureAtlasOffsetBuffer.Add((short)sprite.TextureAtlasOffset.X, (short)(sprite.TextureAtlasOffset.Y + sprite.Height), textureAtlasOffsetBufferIndex + 3);

            byte layer = sprite.DisplayLayer;
            int layerBufferIndex = layerBuffer.Add(layer);

            if (layerBufferIndex != index)
                throw new IndexOutOfRangeException("Invalid layer buffer index");

            layerBuffer.Add(layer, layerBufferIndex + 1);
            layerBuffer.Add(layer, layerBufferIndex + 2);
            layerBuffer.Add(layer, layerBufferIndex + 3);

            return index;
        }

        public int GetDrawIndex(Shape shape, PositionTransformation positionTransformation,
            SizeTransformation sizeTransformation)
        {
            var vertexPositions = shape.ProvideVertexPositions();

            if (vertexPositions.Length == 0)
                return -1;

            // TODO: are the transformations possible with shapes?
            /*if (positionTransformation != null)
                position = positionTransformation(position);

            if (sizeTransformation != null)
                size = sizeTransformation(size);*/

            int index = positionBuffer.Add((short)vertexPositions[0].X, (short)vertexPositions[0].Y);
            for (int i = 1; i < vertexPositions.Length; ++i)
                positionBuffer.Add((short)vertexPositions[i].X, (short)vertexPositions[0].Y, index + i);

            indexBuffer.InsertVertices(index / 2, vertexPositions.Length);

            int colorBufferIndex = colorBuffer.Add(sprite.Color);

            if (colorBufferIndex != index)
                throw new IndexOutOfRangeException("Invalid color buffer index");

            colorBuffer.Add(sprite.Color, colorBufferIndex + 1);
            colorBuffer.Add(sprite.Color, colorBufferIndex + 2);
            colorBuffer.Add(sprite.Color, colorBufferIndex + 3);

            int textureAtlasOffsetBufferIndex = textureAtlasOffsetBuffer.Add((short)sprite.TextureAtlasOffset.X, (short)sprite.TextureAtlasOffset.Y);

            if (textureAtlasOffsetBufferIndex != index)
                throw new IndexOutOfRangeException("Invalid texture atlas offset buffer index");

            textureAtlasOffsetBuffer.Add((short)(sprite.TextureAtlasOffset.X + sprite.Width), (short)sprite.TextureAtlasOffset.Y, textureAtlasOffsetBufferIndex + 1);
            textureAtlasOffsetBuffer.Add((short)(sprite.TextureAtlasOffset.X + sprite.Width), (short)(sprite.TextureAtlasOffset.Y + sprite.Height), textureAtlasOffsetBufferIndex + 2);
            textureAtlasOffsetBuffer.Add((short)sprite.TextureAtlasOffset.X, (short)(sprite.TextureAtlasOffset.Y + sprite.Height), textureAtlasOffsetBufferIndex + 3);

            byte layer = sprite.DisplayLayer;
            int layerBufferIndex = layerBuffer.Add(layer);

            if (layerBufferIndex != index)
                throw new IndexOutOfRangeException("Invalid layer buffer index");

            layerBuffer.Add(layer, layerBufferIndex + 1);
            layerBuffer.Add(layer, layerBufferIndex + 2);
            layerBuffer.Add(layer, layerBufferIndex + 3);

            return index;
        }

        public void UpdatePosition(int index, RenderNode renderNode,
            PositionTransformation positionTransformation, SizeTransformation sizeTransformation)
        {
            var position = new Position(renderNode.X, renderNode.Y);
            var size = new Size(renderNode.Width, renderNode.Height);

            if (positionTransformation != null)
                position = positionTransformation(position);

            if (sizeTransformation != null)
                size = sizeTransformation(size);

            positionBuffer.Update(index, (short)position.X, (short)position.Y);
            positionBuffer.Update(index + 1, (short)(position.X + size.Width), (short)position.Y);
            positionBuffer.Update(index + 2, (short)(position.X + size.Width), (short)(position.Y + size.Height));
            positionBuffer.Update(index + 3, (short)position.X, (short)(position.Y + size.Height));

            if (Shape != Shape.Triangle && baseLineBuffer != null)
            {
                ushort baseLine = (ushort)(position.Y + size.Height + baseLineOffset);

                baseLineBuffer.Update(index, baseLine);
                baseLineBuffer.Update(index + 1, baseLine);
                baseLineBuffer.Update(index + 2, baseLine);
                baseLineBuffer.Update(index + 3, baseLine);
            }
        }

        public void UpdateTextureAtlasOffset(int index, Sprite sprite)
        {
            if (textureAtlasOffsetBuffer == null)
                return;

            textureAtlasOffsetBuffer.Update(index, (short)sprite.TextureAtlasOffset.X, (short)sprite.TextureAtlasOffset.Y);
            textureAtlasOffsetBuffer.Update(index + 1, (short)(sprite.TextureAtlasOffset.X + sprite.Width), (short)sprite.TextureAtlasOffset.Y);
            textureAtlasOffsetBuffer.Update(index + 2, (short)(sprite.TextureAtlasOffset.X + sprite.Width), (short)(sprite.TextureAtlasOffset.Y + sprite.Height));
            textureAtlasOffsetBuffer.Update(index + 3, (short)sprite.TextureAtlasOffset.X, (short)(sprite.TextureAtlasOffset.Y + sprite.Height));
        }

        public void UpdateColor(int index, Color color)
        {
            if (colorBuffer != null)
            {
                colorBuffer.Update(index, color);
                colorBuffer.Update(index + 1, color);
                colorBuffer.Update(index + 2, color);
                colorBuffer.Update(index + 3, color);
            }
        }

        public void UpdateDisplayLayer(int index, byte displayLayer)
        {
            if (layerBuffer != null)
            {
                layerBuffer.Update(index, displayLayer);
                layerBuffer.Update(index + 1, displayLayer);
                layerBuffer.Update(index + 2, displayLayer);
                layerBuffer.Update(index + 3, displayLayer);
            }
        }

        public void FreeDrawIndex(int index)
        {
            /*int newSize = -1;

            if (index == (positionBuffer.Size - 8) / 8)
            {
                int i = (index - 1) * 4;
                newSize = positionBuffer.Size - 8;

                while (i >= 0 && !positionBuffer.IsPositionValid(i))
                {
                    i -= 4;
                    newSize -= 8;
                }
            }*/

            for (int i = 0; i < 4; ++i)
            {
                positionBuffer.Update(index + i, short.MaxValue, short.MaxValue); // ensure it is not visible
                positionBuffer.Remove(index + i);
            }

            if (textureAtlasOffsetBuffer != null)
            {
                textureAtlasOffsetBuffer.Remove(index);
                textureAtlasOffsetBuffer.Remove(index + 1);
                textureAtlasOffsetBuffer.Remove(index + 2);
                textureAtlasOffsetBuffer.Remove(index + 3);
            }

            if (maskTextureAtlasOffsetBuffer != null)
            {
                maskTextureAtlasOffsetBuffer.Remove(index);
                maskTextureAtlasOffsetBuffer.Remove(index + 1);
                maskTextureAtlasOffsetBuffer.Remove(index + 2);
                maskTextureAtlasOffsetBuffer.Remove(index + 3);
            }

            if (baseLineBuffer != null)
            {
                baseLineBuffer.Remove(index);
                baseLineBuffer.Remove(index + 1);
                baseLineBuffer.Remove(index + 2);
                baseLineBuffer.Remove(index + 3);
            }

            if (colorBuffer != null)
            {
                colorBuffer.Remove(index);
                colorBuffer.Remove(index + 1);
                colorBuffer.Remove(index + 2);
                colorBuffer.Remove(index + 3);
            }

            if (layerBuffer != null)
            {
                layerBuffer.Remove(index);
                layerBuffer.Remove(index + 1);
                layerBuffer.Remove(index + 2);
                layerBuffer.Remove(index + 3);
            }

            // TODO: this code causes problems. commented out for now
            /*if (newSize != -1)
            {
                positionBuffer.ReduceSizeTo(newSize);

                if (textureAtlasOffsetBuffer != null)
                    textureAtlasOffsetBuffer.ReduceSizeTo(newSize);

                if (maskTextureAtlasOffsetBuffer != null)
                    maskTextureAtlasOffsetBuffer.ReduceSizeTo(newSize);

                if (baseLineBuffer != null)
                    baseLineBuffer.ReduceSizeTo(newSize / 2);

                if (colorBuffer != null)
                    colorBuffer.ReduceSizeTo(newSize * 2);

                if (layerBuffer != null)
                    layerBuffer.ReduceSizeTo(newSize / 2);
            }*/
        }

        public void Render()
        {
            if (disposed)
                return;

            vertexArrayObject.Bind();

            unsafe
            {
                vertexArrayObject.Lock();

                try
                {
                    State.Gl.DrawElements(PrimitiveType.Triangles, (uint)positionBuffer.Size / 4 * 3, DrawElementsType.UnsignedInt, (void*)0);
                }
                catch
                {
                    // ignore for now
                }
                finally
                {
                    vertexArrayObject.Unlock();
                }
            }
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
                    vertexArrayObject?.Dispose();
                    positionBuffer?.Dispose();
                    textureAtlasOffsetBuffer?.Dispose();
                    maskTextureAtlasOffsetBuffer?.Dispose();
                    baseLineBuffer?.Dispose();
                    colorBuffer?.Dispose();
                    layerBuffer?.Dispose();
                    indexBuffer?.Dispose();

                    disposed = true;
                }
            }
        }
    }
}
