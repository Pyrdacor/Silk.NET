using System;
using System.IO;
using Silk.NET.OpenGL;

namespace Silk.NET.UI.Renderer.OpenGL
{
    internal class Texture : IDisposable
    {
        public static Texture ActiveTexture { get; private set; } = null;

        private bool disposed = false;

        public virtual uint Index { get; private set; } = 0u;
        public virtual int Width { get; } = 0;
        public virtual int Height { get; } = 0;

        protected Texture(int width, int height)
        {
            Index = State.Gl.GenTexture();
            Width = width;
            Height = height;
        }

        public Texture(int width, int height, PixelFormat format, Stream pixelDataStream, int numMipMapLevels = 0)
        {
            int size = width * height * (int)BytesPerPixel[(int)format];

            if ((pixelDataStream.Length - pixelDataStream.Position) < size)
                throw new IOException("Pixel data stream does not contain enough data.");

            if (!pixelDataStream.CanRead)
                throw new IOException("Pixel data stream does not support reading.");

            byte[] pixelData = new byte[size];

            pixelDataStream.Read(pixelData, 0, size);

            Index = State.Gl.GenTexture();
            Width = width;
            Height = height;

            Create(format, pixelData, numMipMapLevels);

            pixelData = null;
        }

        public Texture(int width, int height, PixelFormat format, byte[] pixelData, int numMipMapLevels = 0)
        {
            if (width * height * BytesPerPixel[(int)format] != pixelData.Length)
                throw new ArgumentOutOfRangeException("Invalid texture data size.");

            Index = State.Gl.GenTexture();
            Width = width;
            Height = height;

            Create(format, pixelData, numMipMapLevels);
        }

        static GLEnum ToOpenGLPixelFormat(PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.RGBA8:
                    return GLEnum.Rgba;
                case PixelFormat.BGRA8:
                    return GLEnum.Bgra;
                case PixelFormat.RGB8:
                    return GLEnum.Rgb;
                case PixelFormat.BGR8:
                    return GLEnum.Bgr;
                case PixelFormat.Alpha:
                    // Note: for the supported image format GL_RED means one channel data, GL_ALPHA is only used for texture storage on the gpu, so we don't use it
                    // We always use RGBA8 as texture storage on the gpu
                    return GLEnum.Red;
                default:
                    throw new FormatException("Invalid pixel format.");
            }
        }

        protected void Create(PixelFormat format, byte[] pixelData, int numMipMapLevels)
        {
            if (format >= PixelFormat.RGB5A1)
            {
                pixelData = ConvertPixelData(pixelData, ref format);
            }

            Bind();

            var minMode = (numMipMapLevels > 0) ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest;

            State.Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minMode);
            State.Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            State.Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            State.Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            State.Gl.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            unsafe
            {
                fixed (byte* ptr = &pixelData[0])
                {
                    State.Gl.TexImage2D(GLEnum.Texture2D, 0, (int)InternalFormat.Rgba8, (uint)Width, (uint)Height, 0, ToOpenGLPixelFormat(format), GLEnum.UnsignedByte, ptr);
                }
            }

            if (numMipMapLevels > 0)
                State.Gl.GenerateMipmap(GLEnum.Texture2D);
        }

        public virtual void Bind()
        {
            if (disposed)
                throw new InvalidOperationException("Tried to bind a disposed texture.");

            if (ActiveTexture == this)
                return;

            State.Gl.BindTexture(TextureTarget.Texture2D, Index);
            ActiveTexture = this;
        }

        public static void Unbind()
        {
            State.Gl.BindTexture(TextureTarget.Texture2D, 0);
            ActiveTexture = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (ActiveTexture == this)
                        Unbind();

                    if (Index != 0)
                    {
                        State.Gl.DeleteTexture(Index);
                        Index = 0;
                    }
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);

        }
    }
}
