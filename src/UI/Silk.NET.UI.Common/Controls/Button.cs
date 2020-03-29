using System.Drawing;
using System;
namespace Silk.NET.UI.Controls
{
    /// <summary>
    /// A clickable button.
    /// </summary>
    public class Button : Panel
    {
        private BoolProperty _pressed = new BoolProperty(nameof(Pressed), false);

        public bool Pressed
        {
            get => Enabled && Visible && _pressed.HasValue && _pressed.Value.Value;
            set => _pressed.Value = value && Enabled && Visible;
        }

        public Button(string id = null)
            : base(id)
        {
            // for now set some base dimensions
            Width = 100;
            Height = 60;
        }

        protected override void OnRender(RenderEventArgs args)
        {
            // A button is just a panel which sets some default styles
            // when different states are active.
            ColorValue backgroundColor;
            AllDirectionStyleValue<ColorValue> borderColor;            

            if (Enabled)
            {
                backgroundColor = (ColorValue)(Hovered ? Color.LightGray :  Color.Gray);
                borderColor = (ColorValue)(Hovered ? Color.LightGray :  Color.Gray);
            }
            else
            {
                backgroundColor = (ColorValue)Color.DarkGray;
                borderColor = (ColorValue)Color.DarkGray;
            }

            // TODO: The following 5 lines have to improve
            OverrideStyle/*IfUndefined*/("border.size", 4);
            OverrideStyle/*IfUndefined*/("border.color", Color.DarkGray);
            OverrideStyle/*IfUndefined*/("border.linestyle", Pressed ? BorderLineStyle.Inset : BorderLineStyle.Outset);
            OverrideStyle/*IfUndefined*/("background.color", backgroundColor);

            ControlRenderer.ForceRedraw = true;

            // render with set styles
            base.OnRender(args);
        }
    }
}