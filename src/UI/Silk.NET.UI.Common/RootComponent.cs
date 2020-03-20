namespace Silk.NET.UI.Common
{
    public abstract class RootComponent : Component
    {        
        private IControlRenderer controlRenderer;

        internal override IControlRenderer ControlRenderer => controlRenderer;

        internal void SetControlRenderer(IControlRenderer controlRenderer)
        {
            this.controlRenderer = controlRenderer;
        }
    }
}