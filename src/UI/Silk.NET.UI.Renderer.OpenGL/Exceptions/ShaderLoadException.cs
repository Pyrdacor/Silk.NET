using System;

namespace Silk.NET.UI.Renderer.OpenGL.Exceptions
{
    public class ShaderLoadException : Exception
    {
        public ShaderLoadException(string message)
            : base(message)
        {

        }
    }
}