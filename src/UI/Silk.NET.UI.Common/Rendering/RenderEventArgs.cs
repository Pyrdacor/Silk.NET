using System;

namespace Silk.NET.UI.Common
{
    public class RenderEventArgs : EventArgs
    {
        public ControlRenderer Renderer { get; }

        internal RenderEventArgs(ControlRenderer renderer)
        {
            Renderer = renderer;
        }
    }

    public delegate void RenderEventHandler(object sender, RenderEventArgs args);
}