namespace Silk.NET.UI.Controls
{
    /// <summary>
    /// Most basic control which is only
    /// a clickable surface with a background.
    /// </summary>
    public class Panel : Control
    {
        private int? _backgroundRef;
        private int?[] _borderRefs = new int?[4];
        private int?[] _borderCornerRefs = new int?[2];

        public Panel(string id = null)
            : base(id)
        {
            // for now set some base dimensions
            Width = 100;
            Height = 100;
        }

        protected override void OnRender(RenderEventArgs args)
        {
            var borderSize = Style.Get<AllDirectionStyleValue<int>>("border.size", 0);
            var borderColor = Style.Get<AllDirectionStyleValue<ColorValue>>("border.color", "transparent");
            var borderStyle = Style.Get<AllDirectionStyleValue<BorderLineStyle>>("border.linestyle", BorderLineStyle.None);
            var backgroundColor = Style.Get<ColorValue>("background.color", "gray");

            _backgroundRef = args.Renderer.FillRectangle(_backgroundRef, X, Y, Width, Height, backgroundColor);

            DrawBorder(args.Renderer, 0, borderStyle.Top, borderColor.Top, borderSize.Top);
            DrawBorder(args.Renderer, 1, borderStyle.Right, borderColor.Right, borderSize.Right);
            DrawBorder(args.Renderer, 2, borderStyle.Bottom, borderColor.Bottom, borderSize.Bottom);
            DrawBorder(args.Renderer, 3, borderStyle.Left, borderColor.Left, borderSize.Left);

            // render child controls
            base.OnRender(args);
        }

        private void DrawBorder(ControlRenderer renderer, int index, BorderLineStyle lineStyle, ColorValue color, int size)
        {
            // index: 0 -> top, 1 -> right, 2 -> bottom, 3 -> left
            int x = index switch
            {
                1 => X + Width - size,
                _ => X
            };
            int y = index switch
            {
                0 => Y,
                2 => Y + Height - size,
                _ => size
            };
            int width = index % 2 == 0 ? Width : size;
            int height = index % 2 == 0 ? size : Height - 2 * size;

            switch (lineStyle)
            {
                case BorderLineStyle.Solid:
                    _borderRefs[index] = renderer.DrawRectangleLine(_borderRefs[index], x, y, width, height, color, LineStyle.Solid);
                    break;
                case BorderLineStyle.Dotted:
                    _borderRefs[index] = renderer.DrawRectangleLine(_borderRefs[index], x, y, width, height, color, LineStyle.Dotted);
                    break;
                case BorderLineStyle.Dashed:
                    _borderRefs[index] = renderer.DrawRectangleLine(_borderRefs[index], x, y, width, height, color, LineStyle.Dashed);
                    break;
                case BorderLineStyle.Double:
                case BorderLineStyle.Groove:
                case BorderLineStyle.Ridge:
                    // TODO
                    break;
                case BorderLineStyle.Inset:
                {
                    ColorValue drawColor = index % 3 == 0 ? color.Darken(0.5f) : color.Lighten(0.25f);
                    // TODO: the line end must be a triagle
                    _borderRefs[index] = renderer.DrawRectangleLine(_borderRefs[index], x, y, width, height, drawColor, LineStyle.Solid);
                    if (index == 1) // right
                    {
                        y -= size;
                        _borderCornerRefs[0] = renderer.DrawTriangle(
                            _borderCornerRefs[0], x + width, y, x + width, y + size, x, y + size, drawColor
                        );
                    }
                    break;
                }
                case BorderLineStyle.Outset:
                {
                    ColorValue drawColor = index % 3 == 0 ? color.Lighten(0.25f) : color.Darken(0.5f);
                    // TODO: the line end must be a triagle
                    _borderRefs[index] = renderer.DrawRectangleLine(_borderRefs[index], x, y, width, height, drawColor, LineStyle.Solid);
                    if (index == 1) // right
                    {
                        y -= size;
                        _borderCornerRefs[0] = renderer.DrawTriangle(
                            _borderCornerRefs[0], x + width, y, x + width, y + size, x, y + size, drawColor
                        );
                    }
                    break;
                }
                case BorderLineStyle.None:
                default:
                    break;
            }
        }
    }
}