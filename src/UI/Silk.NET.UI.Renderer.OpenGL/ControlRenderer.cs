using System.Collections.Specialized;
using System.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using Silk.NET.OpenGL;

namespace Silk.NET.UI.Renderer.OpenGL
{
    internal class ControlRenderer : IControlRenderer
    {
        private readonly Dictionary<Layer, RenderLayer> _renderLayers = new Dictionary<Layer, RenderLayer>();
        private readonly Dictionary<int, IRenderNode> _renderNodes = new Dictionary<int, IRenderNode>();
        private readonly IndexPool _renderNodeIndexPool = new IndexPool();
        private readonly Context _context;
        private readonly RenderDimensionReference _renderDimensionReference;
        private readonly TextureAtlas _textureAtlas = new TextureAtlas();
        private uint _displayLayer = 0;

        public ControlRenderer(RenderDimensionReference renderDimensionReference)
        {
            _renderDimensionReference = renderDimensionReference;
            _context = new Context(renderDimensionReference);

            _renderLayers.Add(Layer.Controls, new RenderLayer(Layer.Controls, null));
            _renderLayers.Add(Layer.Images, new RenderLayer(Layer.Images, _textureAtlas.AtlasTexture));
            _renderLayers.Add(Layer.Triangles, new RenderLayer(Layer.Triangles, null));
            _renderLayers.Add(Layer.Ellipsis, new RenderLayer(Layer.Ellipsis, null));
            _renderLayers.Add(Layer.RoundRects, new RenderLayer(Layer.RoundRects, null));
        }

        public void StartRenderCycle()
        {
            _context.SetRotation(Rotation.None); // TODO: can be used later for different devices

            _displayLayer = 0;
            State.Gl.Clear((uint)ClearBufferMask.ColorBufferBit | (uint)ClearBufferMask.DepthBufferBit);
        }

        public void EndRenderCycle()
        {
            foreach (var renderLayer in _renderLayers)
                renderLayer.Value.Render();
        }

        public void RemoveRenderObject(int renderObjectIndex)
        {
            if (_renderNodes.ContainsKey(renderObjectIndex))
            {
                _renderNodes[renderObjectIndex].Delete();
                _renderNodeIndexPool.UnassignIndex(renderObjectIndex);
            }
        }

        public int DrawRectangle(int x, int y, int width, int height, Color color, int lineSize)
        {
            if (width == 0 || height == 0 || lineSize == 0)
                return -1;

            if (width <= 2 * lineSize || height <= 2 * lineSize)
            {
                // it's just a filled rect with the border color
                return FillRectangle(x, y, width, height, color);
            }

            int renderObjectIndex = _renderNodeIndexPool.AssignNextFreeIndex(out _);
            var topLine = new Sprite(width, lineSize, _renderDimensionReference);
            var leftLine = new Sprite(lineSize, height - 2 * lineSize, _renderDimensionReference);
            var rightLine = new Sprite(lineSize, height - 2 * lineSize, _renderDimensionReference);
            var bottomLine = new Sprite(width, lineSize, _renderDimensionReference);
            var layer = _renderLayers[Layer.Controls];

            topLine.X = x;
            topLine.Y = y;
            topLine.Color = color;
            topLine.DisplayLayer = _displayLayer;
            topLine.Layer = layer;
            topLine.Visible = true;

            leftLine.X = x;
            leftLine.Y = y + lineSize;
            leftLine.Color = color;
            leftLine.DisplayLayer = _displayLayer;
            leftLine.Layer = layer;
            leftLine.Visible = true;

            rightLine.X = x + width - lineSize;
            rightLine.Y = y + lineSize;
            rightLine.Color = color;
            rightLine.DisplayLayer = _displayLayer;
            rightLine.Layer = layer;
            rightLine.Visible = true;

            bottomLine.X = x;
            bottomLine.Y = y + height - lineSize;
            bottomLine.Color = color;
            bottomLine.DisplayLayer = _displayLayer;
            bottomLine.Layer = layer;
            bottomLine.Visible = true;

            ++_displayLayer;

            var container = new RenderNodeContainer();

            container.AddChild(topLine);
            container.AddChild(leftLine);
            container.AddChild(rightLine);
            container.AddChild(bottomLine);

            _renderNodes.Add(renderObjectIndex, container);

            return renderObjectIndex;
        }

        public int FillRectangle(int x, int y, int width, int height, Color color)
        {
            if (width == 0 || height == 0)
                return -1;

            int renderObjectIndex = _renderNodeIndexPool.AssignNextFreeIndex(out _);
            var sprite = new Sprite(width, height, _renderDimensionReference);

            sprite.X = x;
            sprite.Y = y;
            sprite.Color = color;
            sprite.DisplayLayer = _displayLayer++; // last draw call -> last rendering (= highest display layer)
            sprite.Layer = _renderLayers[Layer.Controls];
            sprite.Visible = true;

            _renderNodes.Add(renderObjectIndex, sprite);

            return renderObjectIndex;
        }

        public int DrawRectangleLine(int x, int y, int width, int height, Color color, LineStyle lineStyle)
        {
            switch (lineStyle)
            {
                case LineStyle.Solid:
                    return FillRectangle(x, y, width, height, color);
                case LineStyle.Dotted:
                    // TODO
                    return -1;
                case LineStyle.Dashed:
                    // TODO
                    return -1;
                default:
                    return -1;
            }
        }

        public int DrawImage(int x, int y, Image image, Color? colorOverlay = null)
        {
            if (image.Width == 0 || image.Height == 0)
                return -1;

            int renderObjectIndex = _renderNodeIndexPool.AssignNextFreeIndex(out _);
            var sprite = new Sprite(image.Width, image.Height, _renderDimensionReference);

            sprite.X = x;
            sprite.Y = y;
            sprite.Color = colorOverlay ?? Color.White;
            sprite.TextureAtlasOffset = _textureAtlas.AddTexture(image);
            sprite.DisplayLayer = _displayLayer++; // last draw call -> last rendering (= highest display layer)
            sprite.Layer = _renderLayers[Layer.Controls];
            sprite.Visible = true;

            _renderNodes.Add(renderObjectIndex, sprite);

            return renderObjectIndex;
        }

        public int DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
        {
            var p1 = new Point(x1, y1);
            var p2 = new Point(x2, y2);
            var p3 = new Point(x3, y3);

            if (p1 == p2 || p1 == p3 || p2 == p3)
                return -1;

            int renderObjectIndex = _renderNodeIndexPool.AssignNextFreeIndex(out _);
            var shape = Shape.CreateTriangle(_renderDimensionReference, p1, p2, p3);

            shape.X = Util.Min(x1, x2, x3);
            shape.Y = Util.Min(y1, y2, y3);
            shape.Color = color;
            shape.DisplayLayer = _displayLayer++; // last draw call -> last rendering (= highest display layer)
            shape.Layer = _renderLayers[Layer.Triangles];
            shape.Visible = true;

            _renderNodes.Add(renderObjectIndex, shape);

            return renderObjectIndex;
        }
    }

    public class ControlRendererFactory : IControlRendererFactory
    {
        public IControlRenderer CreateControlRenderer(Windowing.Common.IView view)
        {
            var dimensions = new Silk.NET.UI.Renderer.OpenGL.RenderDimensionReference();
            dimensions.SetDimensions(view.Size.Width, view.Size.Height);
            return  new Silk.NET.UI.Renderer.OpenGL.ControlRenderer(dimensions);
        }
    }
}