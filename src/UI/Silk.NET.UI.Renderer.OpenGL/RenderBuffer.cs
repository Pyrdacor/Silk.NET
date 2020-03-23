using System;
using System.Drawing;
using Silk.NET.OpenGL;

namespace Silk.NET.UI.Renderer.OpenGL
{
    internal class RenderBuffer : IDisposable
    {
        bool disposed = false;
        private bool supportTextures = false;
        private int verticesPerNode = 0;
        private PrimitiveType primitiveType = PrimitiveType.Triangles;
        readonly VertexArrayObject vertexArrayObject = null;
        readonly PositionBuffer positionBuffer = null;
        readonly PositionBuffer textureAtlasOffsetBuffer = null;
        readonly ColorBuffer colorBuffer = null;
        readonly LayerBuffer layerBuffer = null;
        readonly IndexBuffer indexBuffer = null;

        public RenderBuffer(bool supportTextures, int verticesPerNode, PrimitiveType primitiveType)
        {
            this.supportTextures = supportTextures;
            this.verticesPerNode = verticesPerNode;
            this.primitiveType = primitiveType;

            if (supportTextures)
            {
                vertexArrayObject = new VertexArrayObject(TextureShader.Instance.ShaderProgram);

                textureAtlasOffsetBuffer = new PositionBuffer(false);
                vertexArrayObject.AddBuffer(TextureShader.DefaultTexCoordName, textureAtlasOffsetBuffer);                
            }
            else
            {
                vertexArrayObject = new VertexArrayObject(ColorShader.Instance.ShaderProgram);

                colorBuffer = new ColorBuffer(true);
                vertexArrayObject.AddBuffer(ColorShader.DefaultColorName, colorBuffer);                
            }

            indexBuffer = new IndexBuffer();
            positionBuffer = new PositionBuffer(false);
            layerBuffer = new LayerBuffer(true);

            vertexArrayObject.AddBuffer("index", indexBuffer);
            vertexArrayObject.AddBuffer(ColorShader.DefaultPositionName, positionBuffer);
            
            vertexArrayObject.AddBuffer(ColorShader.DefaultLayerName, layerBuffer);
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

            if (supportTextures)
            {
                var textureAtlasOffset = sprite.TextureAtlasOffset.Value; // should never be null
                int textureAtlasOffsetBufferIndex = textureAtlasOffsetBuffer.Add((short)textureAtlasOffset.X, (short)textureAtlasOffset.Y);

                if (textureAtlasOffsetBufferIndex != index)
                    throw new IndexOutOfRangeException("Invalid texture atlas offset buffer index");

                textureAtlasOffsetBuffer.Add((short)(textureAtlasOffset.X + sprite.Width), (short)textureAtlasOffset.Y, textureAtlasOffsetBufferIndex + 1);
                textureAtlasOffsetBuffer.Add((short)(textureAtlasOffset.X + sprite.Width), (short)(textureAtlasOffset.Y + sprite.Height), textureAtlasOffsetBufferIndex + 2);
                textureAtlasOffsetBuffer.Add((short)textureAtlasOffset.X, (short)(textureAtlasOffset.Y + sprite.Height), textureAtlasOffsetBufferIndex + 3);
            }
            else
            {
                int colorBufferIndex = colorBuffer.Add(sprite.Color);

                if (colorBufferIndex != index)
                    throw new IndexOutOfRangeException("Invalid color buffer index");

                colorBuffer.Add(sprite.Color, colorBufferIndex + 1);
                colorBuffer.Add(sprite.Color, colorBufferIndex + 2);
                colorBuffer.Add(sprite.Color, colorBufferIndex + 3);
            }

            var layer = sprite.DisplayLayer;
            int layerBufferIndex = layerBuffer.Add(layer);

            if (layerBufferIndex != index)
                throw new IndexOutOfRangeException("Invalid layer buffer index");

            layerBuffer.Add(layer, layerBufferIndex + 1);
            layerBuffer.Add(layer, layerBufferIndex + 2);
            layerBuffer.Add(layer, layerBufferIndex + 3);

            return index;
        }

        public int GetDrawIndex(Shape shape,
            PositionTransformation positionTransformation,
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

            int colorBufferIndex = colorBuffer.Add(shape.Color);

            if (colorBufferIndex != index)
                throw new IndexOutOfRangeException("Invalid color buffer index");

            colorBuffer.Add(shape.Color, colorBufferIndex + 1);
            colorBuffer.Add(shape.Color, colorBufferIndex + 2);
            colorBuffer.Add(shape.Color, colorBufferIndex + 3);

            byte layer = shape.DisplayLayer;
            int layerBufferIndex = layerBuffer.Add(layer);

            if (layerBufferIndex != index)
                throw new IndexOutOfRangeException("Invalid layer buffer index");

            layerBuffer.Add(layer, layerBufferIndex + 1);
            layerBuffer.Add(layer, layerBufferIndex + 2);
            layerBuffer.Add(layer, layerBufferIndex + 3);

            return index;
        }

        public void UpdatePosition(int index, Sprite sprite,
            PositionTransformation positionTransformation,
            SizeTransformation sizeTransformation)
        {
            var position = new Point(sprite.X, sprite.Y);
            var size = new Size(sprite.Width, sprite.Height);

            if (positionTransformation != null)
                position = positionTransformation(position);

            if (sizeTransformation != null)
                size = sizeTransformation(size);

            short x = Util.LimitToShort(position.X);
            short y = Util.LimitToShort(position.Y);
            short width = Util.LimitToShort(size.Width);
            short height = Util.LimitToShort(size.Height);

            positionBuffer.Update(index, x, y);
            positionBuffer.Update(index + 1, (short)(x + width), y);
            positionBuffer.Update(index + 2, (short)(x + width), (short)(y + height));
            positionBuffer.Update(index + 3, x, (short)(y + height));
        }

        public void UpdatePosition(int index, Shape shape,
            PositionTransformation positionTransformation, SizeTransformation sizeTransformation)
        {
            var vertexPositions = shape.ProvideVertexPositions();

            // TODO: are the transformations possible with shapes?
            /*if (positionTransformation != null)
                position = positionTransformation(position);

            if (sizeTransformation != null)
                size = sizeTransformation(size);*/


            for (int i = 0; i < vertexPositions.Length; ++i)
            {
                positionBuffer.Update(index + i,
                    Util.LimitToShort(vertexPositions[i].X),
                    Util.LimitToShort(vertexPositions[i].Y)
                );
            }
        }

        public void UpdateTextureAtlasOffset(int index, Sprite sprite)
        {
            if (textureAtlasOffsetBuffer == null)
                return;

            short x = Util.LimitToShort(sprite.TextureAtlasOffset.Value.X);
            short y = Util.LimitToShort(sprite.TextureAtlasOffset.Value.Y);
            short width = Util.LimitToShort(sprite.Width);
            short height = Util.LimitToShort(sprite.Height);

            textureAtlasOffsetBuffer.Update(index, x, y);
            textureAtlasOffsetBuffer.Update(index + 1, (short)(x + width), y);
            textureAtlasOffsetBuffer.Update(index + 2, (short)(x + width), (short)(y + height));
            textureAtlasOffsetBuffer.Update(index + 3, x, (short)(y + height));
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

        public void UpdateDisplayLayer(int index, uint displayLayer)
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
                    State.Gl.DrawElements(primitiveType, (uint)(positionBuffer.Size / 2 * verticesPerNode), DrawElementsType.UnsignedInt, (void*)0);
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
                    colorBuffer?.Dispose();
                    layerBuffer?.Dispose();
                    indexBuffer?.Dispose();

                    disposed = true;
                }
            }
        }
    }
}
