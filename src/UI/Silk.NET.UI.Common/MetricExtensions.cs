using System.Drawing;

namespace Silk.NET.UI
{
    public static class MetricExtensions
    {
        public static Point Add(this Point point, Point other)
        {
            return new Point(point.X + other.X, point.Y + other.Y);
        }
    }
}