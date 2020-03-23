using System;
using Silk.NET.OpenGL;

namespace Silk.NET.UI.Renderer.OpenGL
{
    using IndexType = UInt32;

    internal class IndexBuffer : BufferObject<IndexType>
    {
        uint index = 0;
        bool disposed = false;
        readonly object bufferLock = new object();
        private IndexType[] buffer = null;
        bool changedSinceLastCreation = true;
        int size = 0;

        public override int Size => size;

        public override VertexAttribPointerType Type => VertexAttribPointerType.UnsignedInt;

        public override int Dimension => 6;

        public IndexBuffer()
        {
            index = State.Gl.GenBuffer();
        }

        public override void Bind()
        {
            if (disposed)
                throw new Exception("Tried to bind a disposed buffer.");

            State.Gl.BindBuffer(GLEnum.ElementArrayBuffer, index);

            Recreate(); // ensure that the data is up to date
        }

        public void Unbind()
        {
            if (disposed)
                return;

            State.Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);
        }

        void Recreate() // is only called when the buffer is bound (see Bind())
        {
            if (!changedSinceLastCreation || buffer == null)
                return;

            lock (bufferLock)
            {
                unsafe
                {
                    fixed (IndexType* ptr = &buffer[0])
                    {
                        State.Gl.BufferData(GLEnum.ElementArrayBuffer, (uint)(Size * sizeof(IndexType)),
                            ptr, GLEnum.StaticDraw);
                    }
                }
            }

            changedSinceLastCreation = false;
        }

        internal override bool RecreateUnbound()
        {
            if (!changedSinceLastCreation || buffer == null)
                return false;

            if (disposed)
                throw new InvalidOperationException("Tried to recreate a disposed buffer.");

            State.Gl.BindBuffer(GLEnum.ArrayBuffer, index);

            lock (bufferLock)
            {
                unsafe
                {
                    fixed (IndexType* ptr = &buffer[0])
                    {
                        State.Gl.BufferData(GLEnum.ArrayBuffer, (uint)(Size * sizeof(IndexType)),
                            ptr, GLEnum.StaticDraw);
                    }
                }
            }

            changedSinceLastCreation = false;

            return true;
        }

        public void InsertQuad(int quadIndex)
        {
            if (quadIndex >= int.MaxValue / 6)
                throw new Exceptions.InsufficientResourcesException("Too many polygons to render.");

            int arrayIndex = quadIndex * 6; // 2 triangles with 3 vertices each
            var vertexIndex = (IndexType)(quadIndex * 4); // 4 different vertices form a quad

            if (size <= arrayIndex + 6)
            {
                buffer = EnsureBufferSize(buffer, arrayIndex + 6, out _);

                buffer[arrayIndex++] = vertexIndex + 0;
                buffer[arrayIndex++] = vertexIndex + 1;
                buffer[arrayIndex++] = vertexIndex + 2;
                buffer[arrayIndex++] = vertexIndex + 3;
                buffer[arrayIndex++] = vertexIndex + 0;
                buffer[arrayIndex++] = vertexIndex + 2;

                size = arrayIndex;
                changedSinceLastCreation = true;
            }
        }

        public void InsertVertices(int offset, int numVertices)
        {
            if (offset > int.MaxValue - numVertices)
                throw new Exceptions.InsufficientResourcesException("Too many polygons to render.");

            if (size < offset + numVertices)
            {
                buffer = EnsureBufferSize(buffer, (int)offset + numVertices, out _);

                for (int i = 0; i < numVertices; ++i)
                    buffer[offset] = (IndexType)offset++;

                size = buffer.Length;
                changedSinceLastCreation = true;
            }
        }

        public override void Dispose()
        {
            Dispose(true);
        }

        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    State.Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);

                    if (index != 0)
                    {
                        State.Gl.DeleteBuffer(index);

                        if (buffer != null)
                        {
                            lock (bufferLock)
                            {
                                buffer = null;
                            }
                        }

                        size = 0;
                        index = 0;
                    }

                    disposed = true;
                }
            }
        }
    }
}
