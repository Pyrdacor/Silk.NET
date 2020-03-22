using System;

namespace Silk.NET.UI.Renderer.OpenGL.Exceptions
{
    public class ShaderProgramLoadException : Exception
    {
        public ShaderProgramLoadException(string message)
            : base(message)
        {

        }
    }
}