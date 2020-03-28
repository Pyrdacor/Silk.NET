using System;
using System.Drawing;
using Silk.NET.OpenGL;

namespace Silk.NET.UI.Renderer.OpenGL
{
    /// <summary>
    /// A shape is a colored polygon.
    /// </summary>
    internal class Shape : RenderNode
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

        public const int TriangleVertices = 3;
        public const int EllipseVertices = 65; // 1 center, 64 outline (drawn as triangle fan)
        public const int RoundRectVertices = 29; // 1 center, 7 for each corner (drawn as triangle fan)

        private Type _type = Type.Triangle;
        private Point[] _points;

        public override int VerticesPerNode { get; } = 0;
        public override PrimitiveType PrimitiveType => _type switch
        {
            Type.Triangle => PrimitiveType.Triangles,
            Type.Ellipse => PrimitiveType.TriangleFan,
            Type.RoundRect => PrimitiveType.TriangleFan,
            _ => PrimitiveType.Triangles,
        };

        private Shape(Type type, RenderDimensionReference renderDimensionReference, params Point[] points)
            : base(CalculateWidth(type, points), CalculateHeight(type, points), renderDimensionReference)
        {
            _points = points;
            VerticesPerNode = type switch
            {
                Type.Triangle => TriangleVertices,
                Type.Ellipse => EllipseVertices,
                Type.RoundRect => RoundRectVertices,
                _ => throw new ArgumentException("Invalid shape type.")
            };
        }

        public static Shape CreateTriangle(RenderDimensionReference renderDimensionReference, Point p1, Point p2, Point p3)
        {
            return new Shape(Type.Triangle, renderDimensionReference, p1, p2, p3 );
        }

        public static Shape CreateEllipse(RenderDimensionReference renderDimensionReference, int width, int height)
        {
            return new Shape(Type.Ellipse, renderDimensionReference, new Point(width, height));
        }

        public static Shape CreateRoundRect(RenderDimensionReference renderDimensionReference, int width, int height)
        {
            return new Shape(Type.RoundRect, renderDimensionReference, new Point(width, height));
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
                Type.Ellipse => points[0].Y,
                Type.RoundRect => points[0].Y,
                _ => 0
            };
        }

        protected override void AddToLayer()
        {
            _drawIndex = Layer.GetDrawIndex(this);
        }

        protected override void RemoveFromLayer()
        {
            if (_drawIndex.HasValue)
            {
                Layer.FreeDrawIndex(_drawIndex.Value);
                _drawIndex = null;
            }
        }

        protected override void UpdatePosition()
        {
            if (_drawIndex.HasValue)
                Layer.UpdatePosition(_drawIndex.Value, this);
        }

        public override void Resize(int width, int height)
        {
            if (Width == width && Height == height)
                return;

            switch (_type)
            {
                case Type.Triangle:
                    {
                        for (int i = 0; i < 3; ++i)
                        {
                            int newX = _points[i].X;
                            int newY = _points[i].Y;

                            if (_points[i].X != 0)
                            {
                                float ratio = (float)_points[i].X / Width;
                                newX = Util.Round(ratio * width);
                            }
                            if (_points[i].Y != 0)
                            {
                                float ratio = (float)_points[i].Y / Height;
                                newY = Util.Round(ratio * height);
                            }

                            _points[i] = new Point(newX, newY);
                        }
                    }
                    break;
                case Type.Ellipse:
                case Type.RoundRect:
                    _points[0] = new Point(width, height);
                    break;
            }            

            base.Resize(width, height);

            UpdatePosition();
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
            return _type switch
            {
                Type.Triangle => _points,
                Type.Ellipse => GetEllipsePoints(),
                Type.RoundRect => GetRoundRectPoints(),
                _ => null
            };
        }

    }
}
