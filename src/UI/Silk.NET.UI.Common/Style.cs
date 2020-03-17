using System.Drawing;

namespace Silk.NET.UI.Common
{
    public struct Style
    {
        #region Background

        public BackgroundStyle Background;
        public Color BackgroundColor;

        #endregion


        #region Border

        public BorderStyle Border;
        public BorderSideStyle BorderTop;
        public BorderSideStyle BorderRight;
        public BorderSideStyle BorderBottom;
        public BorderSideStyle BorderLeft;
        public AllDirectionStyleValue<int> BorderSize;
        public AllDirectionStyleValue<BorderLineStyle> BorderLineStyle;
        public AllDirectionStyleValue<Color> BorderColor;

        #endregion
    }
}
