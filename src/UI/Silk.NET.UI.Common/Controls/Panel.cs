namespace Silk.NET.UI.Controls
{
    /// <summary>
    /// Most basic control which is only
    /// a clickable surface with a background.
    /// </summary>
    public class Panel : Control
    {
        private int? _backgroundRef;
        private int? _borderRef;

        public Panel(string id = null)
            : base(id)
        {
            // for now set some base dimensions
            Width = 100;
            Height = 100;
        }

        protected override void OnRender(RenderEventArgs args)
        {
            var borderSize = Style.Get<int>("border.size", 0);
            var borderColor = Style.Get<ColorValue>("border.color", "transparent");
            var backgroundColor = Style.Get<ColorValue>("background.color", "gray");

            _backgroundRef = args.Renderer.FillRectangle(_backgroundRef, X, Y, Width, Height, backgroundColor);
            _borderRef = args.Renderer.DrawRectangle(_borderRef, X, Y, Width, Height, borderColor, borderSize);

            // render child controls
            base.OnRender(args);
        }
    }
}