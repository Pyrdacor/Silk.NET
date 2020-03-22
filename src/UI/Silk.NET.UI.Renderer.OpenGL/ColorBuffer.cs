using System;
using System.Drawing;
using Silk.NET.OpenGL;

namespace Silk.NET.UI.Renderer.OpenGL
{
    internal class ColorBuffer : BufferObject<byte>
    {
        uint index = 0;
        bool disposed = false;
        byte[] buffer = null;
        readonly object bufferLock = new object();
        int size; // count of values
        readonly IndexPool indices = new IndexPool();
        bool changedSinceLastCreation = true;
        readonly GLEnum usageHint = GLEnum.DynamicDraw;

        public override int Size => size;

        public override VertexAttribPointerType Type => VertexAttribPointerType.UnsignedByte;

        public override int Dimension => 4;

        public ColorBuffer(bool staticData)
        {
            index = State.Gl.GenBuffer();

            if (staticData)
                usageHint = GLEnum.StaticDraw;
        }

        public int Add(Color color, int index = -1)
        {
            bool reused;

            if (index == -1)
                index = indices.AssignNextFreeIndex(out reused);
            else
                reused = indices.AssignIndex(index);

            if (buffer == null)
            {
                buffer = new byte[128];
                buffer[0] = color.R;
                buffer[1] = color.G;
                buffer[2] = color.B;
                buffer[3] = color.A;
                size = 4;
                changedSinceLastCreation = true;
            }
            else
            {
                buffer = EnsureBufferSize(buffer, index * 4, out bool changed);

                if (!reused)
                    size += 4;

                int bufferIndex = index * 4;

                if (buffer[bufferIndex + 0] != color.R ||
                    buffer[bufferIndex + 1] != color.G ||
                    buffer[bufferIndex + 2] != color.B ||
                    buffer[bufferIndex + 3] != color.A)
                {
                    buffer[bufferIndex + 0] = color.R;
                    buffer[bufferIndex + 1] = color.G;
                    buffer[bufferIndex + 2] = color.B;
                    buffer[bufferIndex + 3] = color.A;

                    changedSinceLastCreation = true;
                }
                else if (changed)
                {
                    changedSinceLastCreation = true;
                }
            }

            return index;
        }

        public void Update(int index, Render.Color color)
        {
            int bufferIndex = index * 4;

            if (buffer[bufferIndex + 0] != color.R ||
                buffer[bufferIndex + 1] != color.G ||
                buffer[bufferIndex + 2] != color.B ||
                buffer[bufferIndex + 3] != color.A)
            {
                buffer[bufferIndex + 0] = color.R;
                buffer[bufferIndex + 1] = color.G;
                buffer[bufferIndex + 2] = color.B;
                buffer[bufferIndex + 3] = color.A;

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
                    fixed (byte* ptr = &buffer[0])
                    {
                        State.Gl.BufferData(GLEnum.ArrayBuffer, (uint)(Size * sizeof(byte)),
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
                    fixed (byte* ptr = &buffer[0])
                    {
                        State.Gl.BufferData(GLEnum.ArrayBuffer, (uint)(Size * sizeof(byte)),
                            ptr, usageHint);
                    }
                }
            }

            changedSinceLastCreation = false;

            return true;
        }
    }
}
