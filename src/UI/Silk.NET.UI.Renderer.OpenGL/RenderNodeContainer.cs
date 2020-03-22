using System.Collections.Generic;

namespace Silk.NET.UI.Renderer.OpenGL
{
    internal class RenderNodeContainer : IRenderNode
    {
        private readonly List<RenderNode> children = new List<RenderNode>();

        public void AddChild(RenderNode child)
        {
            if (!children.Contains(child))
                children.Add(child);
        }

        public void Delete()
        {
            foreach (var child in children)
                child.Delete();

            children.Clear();
        }
    }
}