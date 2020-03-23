using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Silk.NET.UI.Renderer.OpenGL
{
    internal class MutableTexture : Texture
    {
        int width = 0;
        int height = 0;
        byte[] data = null;

        public MutableTexture(int width, int height)
            : base(width, height)
        {
            this.width = width;
            this.height = height;
            data = new byte[width * height * 4]; // initialized with zeros so non-occupied areas will be transparent
        }

        public override int Width => width;
        public override int Height => height;

        public void AddSprite(Point position, byte[] data, int width, int height)
        {
            for (int y = 0; y < height; ++y)
            {
                Buffer.BlockCopy(data, y * width * 4, this.data, (position.X + (position.Y + y) * Width) * 4, width * 4);
            }
        }

        public void SetPixel(int x, int y, byte r, byte g, byte b, byte a = 255)
        {
            int index = y * Width + x;

            data[index * 4 + 0] = r;
            data[index * 4 + 1] = g;
            data[index * 4 + 2] = b;
            data[index * 4 + 3] = a;
        }

        public void SetPixels(byte[] pixelData)
        {
            if (pixelData == null)
                throw new ArgumentNullException("Pixel data was null.");

            if (pixelData.Length != data.Length)
                throw new ArgumentOutOfRangeException("Pixel data size does not match texture data size.");

            Buffer.BlockCopy(pixelData, 0, data, 0, pixelData.Length);
        }

        public void Finish(int numMipMapLevels)
        {
            Create(PixelFormat.BGRA8, data, numMipMapLevels);

            data = null;
        }

        public void Resize(int width, int height)
        {
            if (data != null && this.width == width && this.height == height)
                return;

            this.width = width;
            this.height = height;
            data = new byte[width * height * 4]; // initialized with zeros so non-occupied areas will be transparent
        }
    }
}
