namespace Silk.NET.UI
{
    public abstract class RootComponent : Component
    {        
        private ControlRenderer _controlRenderer;

        internal override ControlRenderer ControlRenderer => _controlRenderer;

        internal void SetControlRenderer(IControlRenderer controlRenderer)
        {
            _controlRenderer = new ControlRenderer(controlRenderer);
        }
    }
}