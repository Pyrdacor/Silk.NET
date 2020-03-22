using System;
using System.Net.Http.Headers;
namespace Silk.NET.UI.Renderer.OpenGL
{
    /// <summary>
    /// A shape is a colored polygon.
    /// </summary>
    public class Shape : RenderNode
    {
        public enum Type
        {
            /// <summary>
            /// Is specified by 3 points.
            /// </summary>
            Triangle,
            /// <summary>
            /// Is specified by a width and height (p1 = upper left, p2 = lower right).
            /// Upper left is always (0, 0) so lower right point is enough.
            /// </summary>
            Ellipse,
            /// <summary>
            /// Is specified by a width and height (p1 = upper left, p2 = lower right).
            /// Upper left is always (0, 0) so lower right point is enough.
            /// </summary>
            RoundRect,
        }

        protected int? drawIndex = null;
        int numVertices = 0;
        byte displayLayer = 0;
        Type type = Type.Triangle;
        Point[] points;

        private Shape(Type type, Rect virtualScreen, params Point[] points)
            : base(LayerShape.Polygon, CalculateWidth(type, points), CalculateHeight(type, points), virtualScreen)
        {
            this.points = points;
            numVertices = type switch
            {
                Type.Triangle => 3,
                Type.Ellipse => 64, // TODO
                Type.RoundRect => 20, // TODO
                _ => throw new ArgumentException("Invalid shape type.")
            };
        }

        public Shape CreateTriangle(Rect virtualScreen, Point p1, Point p2, Point p3)
        {
            return new Shape(Type.Triangle, virtualScreen, p1, p2, p3 );
        }

        public Shape CreateEllipse(Rect virtualScreen, int width, int height)
        {
            return new Shape(Type.Ellipse, virtualScreen, new Point(width, height));
        }

        public Shape CreateRoundRect(Rect virtualScreen, int width, int height)
        {
            return new Shape(Type.RoundRect, virtualScreen, new Point(width, height));
        }

        private static int CalculateWidth(Type type, Point[] points)
        {
            return type switch
            {
                Type.Triangle => Util.Max(points[0].X, points[1].X, points[2].X) - Util.Min(points[0].X, points[1].X, points[2].X),
                Type.Ellipse => points[0].X,
                Type.RoundRect => points[0].X,
                _ => 0
            };
        }

        private static int CalculateHeight(Type type, Point[] points)
        {
            return type switch
            {
                Type.Triangle => Util.Max(points[0].Y, points[1].Y, points[2].Y) - Util.Min(points[0].Y, points[1].Y, points[2].Y),
                Type.Circle => points[0].Y,
                Type.RoundRect => points[0].Y,
                _ => 0
            };
        }

        public byte DisplayLayer
        {
            get => displayLayer;
            set
            {
                if (displayLayer == value)
                    return;

                displayLayer = value;

                UpdateDisplayLayer();
            }
        }

        protected override void AddToLayer()
        {
            drawIndex = Layer.GetDrawIndex(this);
        }

        protected override void RemoveFromLayer()
        {
            if (drawIndex.HasValue)
            {
                Layer.FreeDrawIndex(drawIndex.Value);
                drawIndex = null;
            }
        }

        protected override void UpdatePosition()
        {
            if (drawIndex.HasValue)
                Layer.UpdatePosition(drawIndex.Value, this);
        }

        public virtual void Resize(int width, int height)
        {
            if (Width == width && Height == height)
                return;

            switch (type)
            {
                case Type.Triangle:
                    {
                        for (int i = 0; i < 3; ++i)
                        {
                            int newX = points[i].X;
                            int newY = points[i].Y;

                            if (points[i].X != 0)
                            {
                                float ratio = (float)points[i].X / Width;
                                newX = Util.Round(ratio * width);
                            }
                            if (points[i].Y != 0)
                            {
                                float ratio = (float)points[i].Y / Height;
                                newY = Util.Round(ratio * height);
                            }

                            points[i] = new Point(newX, newY);
                        }
                    }
                    break;
                case Type.Circle:
                case Type.RoundRect:
                    points[0] = new Point(width, height);
                    break;
            }            

            base.Resize(width, height);

            UpdatePosition();
        }

        protected override void UpdateDisplayLayer()
        {
            if (drawIndex.HasValue)
                Layer.UpdateDisplayLayer(drawIndex.Value, displayLayer);
        }

        private Point[] GetEllipsePoints()
        {
            // TODO
            return new Point[] { };
        }

        private Point[] GetRoundRectPoints()
        {
            // TODO
            return new Point[] { };
        }

        public override Point[] ProvideVertexPositions()
        {
            return type switch
            {
                Type.Triangle => points,
                Type.Ellipse => GetEllipsePoints(),
                Type.RoundRect => GetRoundRectPoints(),
                _ => null
            };
        }

    }
}
