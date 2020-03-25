namespace Silk.NET.UI
{
    public abstract class RootComponent : Component
    {        
        private ControlRenderer controlRenderer;

        internal override ControlRenderer ControlRenderer => controlRenderer;

        internal void SetControlRenderer(IControlRenderer controlRenderer)
        {
            this.controlRenderer = new ControlRenderer(controlRenderer);
        }
    }
}