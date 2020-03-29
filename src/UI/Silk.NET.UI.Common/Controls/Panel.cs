namespace Silk.NET.UI.Controls
{
    // The panel is the base class for all
    // basic non-component controls like
    // buttons, inputs, labels and so on.

    /// <summary>
    /// Most basic control which is only
    /// a clickable surface with a background.
    /// </summary>
    public class Panel : Control
    {
        private int? _backgroundRef;
        private int?[] _borderRefs = new int?[4];

        public Panel(string id = null)
            : base(id)
        {
            // for now set some base dimensions
            Width = 100;
            Height = 100;
        }

        internal override void DestroyView()
        {
            if (_backgroundRef.HasValue)
            {
                ControlRenderer.RemoveRenderObject(_backgroundRef.Value);
                _backgroundRef = null;
            }

            for (int i = 0; i < _borderRefs.Length; ++i)
            {
                if (_borderRefs[i].HasValue)
                {
                    ControlRenderer.RemoveRenderObject(_borderRefs[i].Value);
                    _borderRefs[i] = null;
                }
            }
        }

        protected override void OnRender(RenderEventArgs args)
        {
            ControlRenderer.ForceRedraw = true; // TODO

            var borderSize = Style.Get<AllDirectionStyleValue<int>>("border.size", 0);
            var borderColor = Style.Get<AllDirectionStyleValue<ColorValue>>("border.color", "transparent");
            var borderStyle = Style.Get<AllDirectionStyleValue<BorderLineStyle>>("border.linestyle", BorderLineStyle.None);
            var backgroundColor = Style.Get<ColorValue>("background.color", "gray");

            _backgroundRef = args.Renderer.FillRectangle(_backgroundRef, X, Y, Width, Height, backgroundColor);

            var renderer = args.Renderer;
            var rectangle = ClientRectangle;

            DrawBorder(ref _borderRefs[0], renderer, StlyeDirection.Top, borderStyle.Top,
                borderColor.Top, borderSize.Top, rectangle);
            DrawBorder(ref _borderRefs[1], renderer, StlyeDirection.Right, borderStyle.Right,
                borderColor.Right, borderSize.Right, rectangle);
            DrawBorder(ref _borderRefs[2], renderer, StlyeDirection.Bottom, borderStyle.Bottom,
                borderColor.Bottom, borderSize.Bottom, rectangle);
            DrawBorder(ref _borderRefs[3], renderer, StlyeDirection.Left, borderStyle.Left,
                borderColor.Left, borderSize.Left, rectangle);

            // render child controls
            base.OnRender(args);
        }

        internal override void CheckStyleChanges()
        {
            Parent?.CheckStyleChanges();
        }
    }
}