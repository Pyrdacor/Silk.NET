using System.ComponentModel;

namespace Silk.NET.UI
{
    public struct Style
    {
        #region Background

        public BackgroundStyle? Background;
        [DefaultValue("#ffffff")] // TODO
        public ColorValue? BackgroundColor;

        #endregion


        #region Border

        public BorderStyle? Border;
        public BorderSideStyle? BorderTop;
        public BorderSideStyle? BorderRight;
        public BorderSideStyle? BorderBottom;
        public BorderSideStyle? BorderLeft;
        [DefaultValue(0)]
        public AllDirectionStyleValue<int>? BorderSize;
        [DefaultValue(UI.BorderLineStyle.None)]
        public AllDirectionStyleValue<BorderLineStyle>? BorderLineStyle;
        [DefaultValue("#000000")]
        public AllDirectionStyleValue<ColorValue>? BorderColor;

        #endregion
    }
}
