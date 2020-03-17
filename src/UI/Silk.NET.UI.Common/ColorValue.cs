using System;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Silk.NET.UI.Common
{
    /// <summary>
    /// Color value for styling.
    /// 
    /// Colors can be specified as:
    /// - Named color: like red, green, etc
    /// - Color struct: System.Drawing.Color
    /// - Integer: 0xaarrggbb or its decimal representation
    /// - RGB: "r g b"
    /// - RGBA: "r g b a"
    /// - Hex string: "#aarrggbb" or "#rrggbb" or "#argb" or "#rgb"
    /// 
    /// Examples:
    ///  "green"
    ///  System.Drawing.Color.Black
    ///  0xff00ffff
    ///  "255 0 255"
    ///  "64 0 0 255"
    ///  "#ff002040"
    ///  "#aa00ee"
    ///  "#fa0b"
    ///  "#f0f"
    /// </summary>
    public struct ColorValue
    {
        private static readonly Regex RgbPattern = new Regex(@"^([0-9]{1,3}) ([0-9]{1,3}) ([0-9]{1,3})$", RegexOptions.Compiled);
        private static readonly Regex RgbaPattern = new Regex(@"^([0-9]{1,3}) ([0-9]{1,3}) ([0-9]{1,3}) ([0-9]{1,3})$", RegexOptions.Compiled);
        private static readonly Regex HexArgbPattern = new Regex(@"^#([0-9a-f]{2})([0-9a-f]{2})([0-9a-f]{2})([0-9a-f]{2})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex HexRgbPattern = new Regex(@"^#([0-9a-f]{2})([0-9a-f]{2})([0-9a-f]{2})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex HexArgbShortPattern = new Regex(@"^#([0-9a-f])([0-9a-f])([0-9a-f])([0-9a-f])$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex HexRgbShortPattern = new Regex(@"^#([0-9a-f])([0-9a-f])([0-9a-f])$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private Color color;

        public ColorValue(Color color)
        {
            this.color = color;
        }
        
        public ColorValue(byte r, byte g, byte b, byte a = 255)
        {
            color = Color.FromArgb(r, g, b, a);
        }

        public ColorValue(int color)
        {
            this.color = Color.FromArgb(color);
        }

        public static implicit operator ColorValue(Color value)
        {
            return new ColorValue(value);
        }

        public static implicit operator ColorValue(int value)
        {
            return new ColorValue(value);
        }

        public static implicit operator Color(ColorValue value)
        {
            return value.color;
        }

        public static implicit operator ColorValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new FormatException($"Invalid color value: {value}");

            value = value.Trim();

            if (value.ToLower() == "transparent")
                return new ColorValue(Color.Transparent);

            var match = HexArgbPattern.Match(value);

            if (match.Success)
            {
                return new ColorValue
                (
                    byte.Parse(match.Groups[2].Value, NumberStyles.AllowHexSpecifier),
                    byte.Parse(match.Groups[3].Value, NumberStyles.AllowHexSpecifier),
                    byte.Parse(match.Groups[4].Value, NumberStyles.AllowHexSpecifier),
                    byte.Parse(match.Groups[1].Value, NumberStyles.AllowHexSpecifier)
                );
            }

            match = HexRgbPattern.Match(value);

            if (match.Success)
            {
                return new ColorValue
                (
                    byte.Parse(match.Groups[1].Value, NumberStyles.AllowHexSpecifier),
                    byte.Parse(match.Groups[2].Value, NumberStyles.AllowHexSpecifier),
                    byte.Parse(match.Groups[3].Value, NumberStyles.AllowHexSpecifier)
                );
            }

            match = HexArgbShortPattern.Match(value);

            if (match.Success)
            {
                return new ColorValue
                (
                    byte.Parse(match.Groups[2].Value, NumberStyles.AllowHexSpecifier),
                    byte.Parse(match.Groups[3].Value, NumberStyles.AllowHexSpecifier),
                    byte.Parse(match.Groups[4].Value, NumberStyles.AllowHexSpecifier),
                    byte.Parse(match.Groups[1].Value, NumberStyles.AllowHexSpecifier)
                );
            }

            match = HexRgbShortPattern.Match(value);

            if (match.Success)
            {
                return new ColorValue
                (
                    byte.Parse(match.Groups[1].Value, NumberStyles.AllowHexSpecifier),
                    byte.Parse(match.Groups[2].Value, NumberStyles.AllowHexSpecifier),
                    byte.Parse(match.Groups[3].Value, NumberStyles.AllowHexSpecifier)
                );
            }

            match = RgbaPattern.Match(value);

            if (match.Success)
            {
                int r = int.Parse(match.Groups[1].Value);
                int g = int.Parse(match.Groups[2].Value);
                int b = int.Parse(match.Groups[3].Value);
                int a = int.Parse(match.Groups[4].Value);

                if (r > 255 || g > 255 || b > 255 || a > 255)
                    throw new FormatException($"Invalid color value format: {value}");

                return new ColorValue
                (
                    (byte)r, (byte)g, (byte)b, (byte)a
                );
            }

            match = RgbPattern.Match(value);

            if (match.Success)
            {
                int r = int.Parse(match.Groups[1].Value);
                int g = int.Parse(match.Groups[2].Value);
                int b = int.Parse(match.Groups[3].Value);

                if (r > 255 || g > 255 || b > 255)
                    throw new FormatException($"Invalid color value format: {value}");

                return new ColorValue
                (
                    (byte)r, (byte)g, (byte)b
                );
            }

            var color = Color.FromName(value);

            if (color == Color.Transparent)
                throw new FormatException($"Invalid color value format: {value}");

            return new ColorValue(color);
        }
    }
}