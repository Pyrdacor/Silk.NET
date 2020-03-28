using System;
using System.Drawing;
using Silk.NET.OpenGL;

namespace Silk.NET.UI.Renderer.OpenGL
{
    internal class RenderBuffer : IDisposable
    {
        private bool _disposed = false;
        private bool _supportTextures = false;
        private int _verticesPerNode = 0;
        private PrimitiveType _primitiveType = PrimitiveType.Triangles;
        private readonly VertexArrayObject _vertexArrayObject = null;
        private readonly PositionBuffer _positionBuffer = null;
        private readonly PositionBuffer _textureAtlasOffsetBuffer = null;
        private readonly ColorBuffer _colorBuffer = null;
        private readonly LayerBuffer _layerBuffer = null;
        private readonly IndexBuffer _indexBuffer = null;

        public RenderBuffer(bool supportTextures, int verticesPerNode, PrimitiveType primitiveType)
        {
            _supportTextures = supportTextures;
            _verticesPerNode = verticesPerNode;
            _primitiveType = primitiveType;

            if (supportTextures)
            {
                _vertexArrayObject = new VertexArrayObject(TextureShader.Instance.ShaderProgram);

                _textureAtlasOffsetBuffer = new PositionBuffer(false);
                _vertexArrayObject.AddBuffer(TextureShader.DefaultTexCoordName, _textureAtlasOffsetBuffer);                
            }
            else
            {
                _vertexArrayObject = new VertexArrayObject(ColorShader.Instance.ShaderProgram);

                _colorBuffer = new ColorBuffer(true);
                _vertexArrayObject.AddBuffer(ColorShader.DefaultColorName, _colorBuffer);                
            }

            _indexBuffer = new IndexBuffer();
            _positionBuffer = new PositionBuffer(false);
            _layerBuffer = new LayerBuffer(true);

            _vertexArrayObject.AddBuffer("index", _indexBuffer);
            _vertexArrayObject.AddBuffer(ColorShader.DefaultPositionName, _positionBuffer);
            
            _vertexArrayObject.AddBuffer(ColorShader.DefaultLayerName, _layerBuffer);
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

            int index = _positionBuffer.Add((short)position.X, (short)position.Y);
            _positionBuffer.Add((short)(position.X + size.Width), (short)position.Y, index + 1);
            _positionBuffer.Add((short)(position.X + size.Width), (short)(position.Y + size.Height), index + 2);
            _positionBuffer.Add((short)position.X, (short)(position.Y + size.Height), index + 3);

            _indexBuffer.InsertQuad(index / 4);

            if (_supportTextures)
            {
                var textureAtlasOffset = sprite.TextureAtlasOffset.Value; // should never be null
                int textureAtlasOffsetBufferIndex = _textureAtlasOffsetBuffer.Add((short)textureAtlasOffset.X, (short)textureAtlasOffset.Y);

                if (textureAtlasOffsetBufferIndex != index)
                    throw new IndexOutOfRangeException("Invalid texture atlas offset buffer index");

                _textureAtlasOffsetBuffer.Add((short)(textureAtlasOffset.X + sprite.Width), (short)textureAtlasOffset.Y, textureAtlasOffsetBufferIndex + 1);
                _textureAtlasOffsetBuffer.Add((short)(textureAtlasOffset.X + sprite.Width), (short)(textureAtlasOffset.Y + sprite.Height), textureAtlasOffsetBufferIndex + 2);
                _textureAtlasOffsetBuffer.Add((short)textureAtlasOffset.X, (short)(textureAtlasOffset.Y + sprite.Height), textureAtlasOffsetBufferIndex + 3);
            }
            else
            {
                int colorBufferIndex = _colorBuffer.Add(sprite.Color);

                if (colorBufferIndex != index)
                    throw new IndexOutOfRangeException("Invalid color buffer index");

                _colorBuffer.Add(sprite.Color, colorBufferIndex + 1);
                _colorBuffer.Add(sprite.Color, colorBufferIndex + 2);
                _colorBuffer.Add(sprite.Color, colorBufferIndex + 3);
            }

            var layer = sprite.DisplayLayer;
            int layerBufferIndex = _layerBuffer.Add(layer);

            if (layerBufferIndex != index)
                throw new IndexOutOfRangeException("Invalid layer buffer index");

            _layerBuffer.Add(layer, layerBufferIndex + 1);
            _layerBuffer.Add(layer, layerBufferIndex + 2);
            _layerBuffer.Add(layer, layerBufferIndex + 3);

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

            int index = _positionBuffer.Add((short)vertexPositions[0].X, (short)vertexPositions[0].Y);

            for (int i = 1; i < vertexPositions.Length; ++i)
                _positionBuffer.Add((short)vertexPositions[i].X, (short)vertexPositions[0].Y, index + i);

            _indexBuffer.InsertVertices(index / 2, vertexPositions.Length);

            int colorBufferIndex = _colorBuffer.Add(shape.Color);

            if (colorBufferIndex != index)
                throw new IndexOutOfRangeException("Invalid color buffer index");

            for (int i = 1; i < vertexPositions.Length; ++i)
                _colorBuffer.Add(shape.Color, colorBufferIndex + i);

            var layer = shape.DisplayLayer;
            int layerBufferIndex = _layerBuffer.Add(layer);

            if (layerBufferIndex != index)
                throw new IndexOutOfRangeException("Invalid layer buffer index");

            for (int i = 1; i < vertexPositions.Length; ++i)
                _layerBuffer.Add(layer, layerBufferIndex + i);

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

            _positionBuffer.Update(index, x, y);
            _positionBuffer.Update(index + 1, (short)(x + width), y);
            _positionBuffer.Update(index + 2, (short)(x + width), (short)(y + height));
            _positionBuffer.Update(index + 3, x, (short)(y + height));
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
                _positionBuffer.Update(index + i,
                    Util.LimitToShort(vertexPositions[i].X),
                    Util.LimitToShort(vertexPositions[i].Y)
                );
            }
        }

        public void UpdateTextureAtlasOffset(int index, Sprite sprite)
        {
            if (_textureAtlasOffsetBuffer == null)
                return;

            short x = Util.LimitToShort(sprite.TextureAtlasOffset.Value.X);
            short y = Util.LimitToShort(sprite.TextureAtlasOffset.Value.Y);
            short width = Util.LimitToShort(sprite.Width);
            short height = Util.LimitToShort(sprite.Height);

            _textureAtlasOffsetBuffer.Update(index, x, y);
            _textureAtlasOffsetBuffer.Update(index + 1, (short)(x + width), y);
            _textureAtlasOffsetBuffer.Update(index + 2, (short)(x + width), (short)(y + height));
            _textureAtlasOffsetBuffer.Update(index + 3, x, (short)(y + height));
        }

        public void UpdateColor(int index, Color color, int numVertices)
        {
            if (_colorBuffer != null)
            {
                for (int i = 0; i < numVertices; ++i)
                    _colorBuffer.Update(index + i, color);
            }
        }

        public void UpdateDisplayLayer(int index, uint displayLayer, int numVertices)
        {
            if (_layerBuffer != null)
            {
                for (int i = 0; i < numVertices; ++i)
                    _layerBuffer.Update(index + i, displayLayer);
            }
        }

        public void FreeDrawIndex(int index, int numVertices)
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

            for (int i = 0; i < numVertices; ++i)
            {
                _positionBuffer.Update(index + i, short.MaxValue, short.MaxValue); // ensure it is not visible
                _positionBuffer.Remove(index + i);
            }

            if (_textureAtlasOffsetBuffer != null)
            {
                for (int i = 0; i < numVertices; ++i)
                    _textureAtlasOffsetBuffer.Remove(index + i);
            }

            if (_colorBuffer != null)
            {
                for (int i = 0; i < numVertices; ++i)
                    _colorBuffer.Remove(index + i);
            }

            if (_layerBuffer != null)
            {
                for (int i = 0; i < numVertices; ++i)
                    _layerBuffer.Remove(index + i);
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
            if (_disposed)
                return;

            _vertexArrayObject.Bind();

            unsafe
            {
                _vertexArrayObject.Lock();

                try
                {
                    State.Gl.DrawElements(_primitiveType, (uint)(_positionBuffer.Size / 2 * _verticesPerNode), DrawElementsType.UnsignedInt, (void*)0);
                }
                catch
                {
                    // ignore for now
                }
                finally
                {
                    _vertexArrayObject.Unlock();
                }
            }
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
                    _vertexArrayObject?.Dispose();
                    _positionBuffer?.Dispose();
                    _textureAtlasOffsetBuffer?.Dispose();
                    _colorBuffer?.Dispose();
                    _layerBuffer?.Dispose();
                    _indexBuffer?.Dispose();

                    _disposed = true;
                }
            }
        }
    }
}
