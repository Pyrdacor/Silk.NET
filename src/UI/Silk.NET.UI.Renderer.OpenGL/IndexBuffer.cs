using System;
using Silk.NET.OpenGL;

namespace Silk.NET.UI.Renderer.OpenGL
{
    using IndexType = UInt32;

    internal class IndexBuffer : BufferObject<IndexType>
    {
        private uint _index = 0;
        private bool _disposed = false;
        private readonly object _bufferLock = new object();
        private IndexType[] _buffer = null;
        private bool _changedSinceLastCreation = true;
        private int _size = 0;

        public override int Size => _size;

        public override VertexAttribPointerType Type => VertexAttribPointerType.UnsignedInt;

        public override int Dimension => 6;

        public IndexBuffer()
        {
            _index = State.Gl.GenBuffer();
        }

        public override void Bind()
        {
            if (_disposed)
                throw new Exception("Tried to bind a disposed buffer.");

            State.Gl.BindBuffer(GLEnum.ElementArrayBuffer, _index);

            Recreate(); // ensure that the data is up to date
        }

        public void Unbind()
        {
            if (_disposed)
                return;

            State.Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);
        }

        void Recreate() // is only called when the buffer is bound (see Bind())
        {
            if (!_changedSinceLastCreation || _buffer == null)
                return;

            lock (_bufferLock)
            {
                unsafe
                {
                    fixed (IndexType* ptr = &_buffer[0])
                    {
                        State.Gl.BufferData(GLEnum.ElementArrayBuffer, (uint)(Size * sizeof(IndexType)),
                            ptr, GLEnum.StaticDraw);
                    }
                }
            }

            _changedSinceLastCreation = false;
        }

        internal override bool RecreateUnbound()
        {
            if (!_changedSinceLastCreation || _buffer == null)
                return false;

            if (_disposed)
                throw new InvalidOperationException("Tried to recreate a disposed buffer.");

            State.Gl.BindBuffer(GLEnum.ArrayBuffer, _index);

            lock (_bufferLock)
            {
                unsafe
                {
                    fixed (IndexType* ptr = &_buffer[0])
                    {
                        State.Gl.BufferData(GLEnum.ArrayBuffer, (uint)(Size * sizeof(IndexType)),
                            ptr, GLEnum.StaticDraw);
                    }
                }
            }

            _changedSinceLastCreation = false;

            return true;
        }

        public void InsertQuad(int quadIndex)
        {
            if (quadIndex >= int.MaxValue / 6)
                throw new Exceptions.InsufficientResourcesException("Too many polygons to render.");

            int arrayIndex = quadIndex * 6; // 2 triangles with 3 vertices each
            var vertexIndex = (IndexType)(quadIndex * 4); // 4 different vertices form a quad

            if (_size <= arrayIndex + 6)
            {
                _buffer = EnsureBufferSize(_buffer, arrayIndex + 6, out _);

                _buffer[arrayIndex++] = vertexIndex + 0;
                _buffer[arrayIndex++] = vertexIndex + 1;
                _buffer[arrayIndex++] = vertexIndex + 2;
                _buffer[arrayIndex++] = vertexIndex + 3;
                _buffer[arrayIndex++] = vertexIndex + 0;
                _buffer[arrayIndex++] = vertexIndex + 2;

                _size = arrayIndex;
                _changedSinceLastCreation = true;
            }
        }

        public void InsertVertices(int offset, int numVertices)
        {
            if (offset > int.MaxValue - numVertices)
                throw new Exceptions.InsufficientResourcesException("Too many polygons to render.");

            if (_size < offset + numVertices)
            {
                _buffer = EnsureBufferSize(_buffer, (int)offset + numVertices, out _);

                for (int i = 0; i < numVertices; ++i)
                    _buffer[offset] = (IndexType)offset++;

                _size = _buffer.Length;
                _changedSinceLastCreation = true;
            }
        }

        public override void Dispose()
        {
            Dispose(true);
        }

        void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    State.Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);

                    if (_index != 0)
                    {
                        State.Gl.DeleteBuffer(_index);

                        if (_buffer != null)
                        {
                            lock (_bufferLock)
                            {
                                _buffer = null;
                            }
                        }

                        _size = 0;
                        _index = 0;
                    }

                    _disposed = true;
                }
            }
        }
    }
}
