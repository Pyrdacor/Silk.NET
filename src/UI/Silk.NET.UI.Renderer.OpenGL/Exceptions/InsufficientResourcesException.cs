using System;

namespace Silk.NET.UI.Renderer.OpenGL.Exceptions
{
    public class InsufficientResourcesException : Exception
    {
        public InsufficientResourcesException(string message)
            : base(message)
        {

        }
    }
}