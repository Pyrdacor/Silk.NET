using System;
using Silk.NET.OpenGL;

namespace Silk.NET.UI.Renderer.OpenGL
{
    internal class PositionBuffer : BufferObject<short>
    {
        uint index = 0;
        bool disposed = false;
        short[] buffer = null;
        readonly object bufferLock = new object();
        int size;
        readonly IndexPool indices = new IndexPool();
        bool changedSinceLastCreation = true;
        readonly GLEnum usageHint = GLEnum.DynamicDraw;

        public override int Size => size;

        public override VertexAttribPointerType Type => VertexAttribPointerType.Short;

        public override int Dimension => 2;

        public PositionBuffer(bool staticData)
        {
            index = State.Gl.GenBuffer();

            if (staticData)
                usageHint = GLEnum.StaticDraw;
        }

        public bool IsPositionValid(int index)
        {
            index *= Dimension; // 2 coords each

            if (index < 0 || index >= buffer.Length)
                return false;

            return buffer[index] != short.MaxValue;
        }

        public int Add(short x, short y, int index = -1)
        {
            bool reused;

            if (index == -1)
                index = indices.AssignNextFreeIndex(out reused);
            else
                reused = indices.AssignIndex(index);

            if (buffer == null)
            {
                buffer = new short[256];
                buffer[0] = x;
                buffer[1] = y;
                size = 2;
                changedSinceLastCreation = true;
            }
            else
            {
                buffer = EnsureBufferSize(buffer, index * 2, out bool changed);

                if (!reused)
                    size += 2;

                int bufferIndex = index * 2;

                if (buffer[bufferIndex + 0] != x ||
                    buffer[bufferIndex + 1] != y)
                {
                    buffer[bufferIndex + 0] = x;
                    buffer[bufferIndex + 1] = y;

                    changedSinceLastCreation = true;
                }
                else if (changed)
                {
                    changedSinceLastCreation = true;
                }
            }

            return index;
        }

        public void Update(int index, short x, short y)
        {
            int bufferIndex = index * 2;

            if (buffer[bufferIndex + 0] != x ||
                buffer[bufferIndex + 1] != y)
            {
                buffer[bufferIndex + 0] = x;
                buffer[bufferIndex + 1] = y;

                changedSinceLastCreation = true;
            }
        }

        public void Remove(int index)
        {
            indices.UnassignIndex(index);
        }

        public void ReduceSizeTo(int size)
        {
            this.size = size;
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
                    State.Gl.BindBuffer(GLEnum.ArrayBuffer, 0);

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

        public override void Bind()
        {
            if (disposed)
                throw new InvalidOperationException("Tried to bind a disposed buffer.");

            State.Gl.BindBuffer(GLEnum.ArrayBuffer, index);

            Recreate(); // ensure that the data is up to date
        }

        void Recreate() // is only called when the buffer is bound (see Bind())
        {
            if (!changedSinceLastCreation || buffer == null)
                return;

            lock (bufferLock)
            {
                unsafe
                {
                    fixed (short* ptr = &buffer[0])
                    {
                        State.Gl.BufferData(GLEnum.ArrayBuffer, (uint)(Size * sizeof(short)),
                            ptr, usageHint);
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
                    fixed (short* ptr = &buffer[0])
                    {
                        State.Gl.BufferData(GLEnum.ArrayBuffer, (uint)(Size * sizeof(short)),
                            ptr, usageHint);
                    }
                }
            }

            changedSinceLastCreation = false;

            return true;
        }
    }
}
