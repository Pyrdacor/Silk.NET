using System.Reflection;
using System.Collections.Generic;
using System.Drawing;

namespace Silk.NET.UI.Renderer.OpenGL
{
    internal class TextureAtlas
    {
        public MutableTexture AtlasTexture { get; } = new MutableTexture(0, 0);
        private readonly Dictionary<Image, Point> images = new Dictionary<Image, Point>();

        public Point AddTexture(Image image)
        {
            if (!images.ContainsKey(image))
            {
                // TODO: place the image so the atlas size uses minimal space
                // AtlasTexture.AddSprite(x, y, imageData, image.Width, image.Height);
                // return position
                return Point.Empty;
            }
            else
            {
                return images[image];
            }
        }
    }
}
