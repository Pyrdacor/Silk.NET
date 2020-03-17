using System.Drawing;

namespace Silk.NET.UI.Common
{
    public enum BorderLineStyle
    {
        /// <summary>
        /// Defines a solid border.
        /// </summary>
        Solid,
        /// <summary>
        /// Defines a dotted border.
        /// </summary>
        Dotted,
        /// <summary>
        /// Defines a dashed border.
        /// </summary>
        Dashed,
        /// <summary>
        /// Defines a double border.
        /// </summary>
        Double,
        /// <summary>
        /// Defines a 3D grooved border. The effect depends on the BorderColor value.
        /// </summary>
        Groove,
        /// <summary>
        /// Defines a 3D ridged border. The effect depends on the BorderColor value.
        /// </summary>
        Ridge,
        /// <summary>
        /// Defines a 3D inset border. The effect depends on the BorderColor value.
        /// </summary>
        Inset,
        /// <summary>
        /// Defines a 3D outset border. The effect depends on the BorderColor value.
        /// </summary>
        Outset,
        /// <summary>
        /// Defines no border.
        /// </summary>
        None,
    }

    public struct BorderStyle
    {
        public AllDirectionStyleValue<int> Size;
        public AllDirectionStyleValue<BorderLineStyle> LineStyle;
        public AllDirectionStyleValue<Color> Color;
    }

    public struct BorderSideStyle
    {
        public int Size;
        public BorderLineStyle LineStyle;
        public Color Color;
    }
}