using System;
using System.Collections.Generic;
using System.Drawing;

namespace Silk.NET.UI.Renderer.OpenGL
{
    public class ControlRenderer : IControlRenderer
    {
        private readonly Dictionary<Layer, RenderLayer> renderLayers = new Dictionary<Layer, RenderLayer>();
        private readonly Dictionary<int, RenderNode> renderNodes = new Dictionary<int, RenderNode>();
        private readonly IndexPool renderNodeIndexPool = new IndexPool();
        private readonly Context context;
        private readonly RenderDimensionReference renderDimensionReference;
        private readonly TextureAtlas textureAtlas = new TextureAtlas();
        private int displayLayer = 0;

        public ControlRenderer(RenderDimensionReference renderDimensionReference)
        {
            this.renderDimensionReference = renderDimensionReference;
            context = new Context(renderDimensionReference);

            renderLayers.Add(Layer.Controls, new RenderLayer(Layer.Controls, null));
            renderLayers.Add(Layer.Images, new RenderLayer(Layer.Images, textureAtlas.AtlasTexture));
            renderLayers.Add(Layer.Shapes, new RenderLayer(Layer.Shapes, null));
        }

        public void StartRenderCycle()
        {
            displayLayer = 0;
        }

        public void EndRenderCycle()
        {
            
        }

        public void RemoveRenderObject(int renderObjectIndex)
        {
            if (renderNodes.ContainsKey(renderObjectIndex))
            {
                renderNodes[renderObjectIndex].Delete();
                renderNodeIndexPool.UnassignIndex(renderObjectIndex);
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

            int renderObjectIndex = renderNodeIndexPool.AssignNextFreeIndex(out _);
            var topLine = new Sprite(width, lineSize, renderDimensionReference);
            var leftLine = new Sprite(lineSize, height - 2 * lineSize, renderDimensionReference);
            var rightLine = new Sprite(lineSize, height - 2 * lineSize, renderDimensionReference);
            var bottomLine = new Sprite(width, height - 2 * lineSize, renderDimensionReference);
            var layer = renderLayers[Layer.Controls];

            topLine.X = x;
            topLine.Y = y;
            topLine.Color = color;
            topLine.DisplayLayer = displayLayer;
            topLine.Layer = layer;
            topLine.Visible = true;

            leftLine.X = x;
            leftLine.Y = y + lineSize;
            leftLine.Color = color;
            leftLine.DisplayLayer = displayLayer;
            leftLine.Layer = layer;
            leftLine.Visible = true;

            rightLine.X = x + width - lineSize;
            rightLine.Y = y + lineSize;
            rightLine.Color = color;
            rightLine.DisplayLayer = displayLayer;
            rightLine.Layer = layer;
            rightLine.Visible = true;

            bottomLine.X = x;
            bottomLine.Y = y + height - lineSize;
            bottomLine.Color = color;
            bottomLine.DisplayLayer = displayLayer;
            bottomLine.Layer = layer;
            bottomLine.Visible = true;

            ++displayLayer;

            var container = new RenderNodeContainer();

            container.AddChild(topLine);
            container.AddChild(leftLine);
            container.AddChild(rightLine);
            container.AddChild(bottomLine);

            renderNodes.Add(renderObjectIndex, container);

            return renderObjectIndex;
        }

        public int FillRectangle(int x, int y, int width, int height, Color color)
        {
            if (width == 0 || height == 0)
                return -1;

            int renderObjectIndex = renderNodeIndexPool.AssignNextFreeIndex(out _);
            var sprite = new Sprite(width, height, renderDimensionReference);

            sprite.X = x;
            sprite.Y = y;
            sprite.Color = color;
            sprite.DisplayLayer = displayLayer++; // last draw call -> last rendering (= highest display layer)
            sprite.Layer = renderLayers[Layer.Controls];
            sprite.Visible = true;

            renderNodes.Add(renderObjectIndex, sprite);

            return renderObjectIndex;
        }

        public int DrawLine(int x1, int y1, int x2, int y2, Color color)
        {           
            return FillRectangle(Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(x2 - x1), Math.Abs(y2 - y1), color);
        }

        public int DrawImage(int x, int y, Image image, Color? colorOverlay = null)
        {
            if (image.Width == 0 || image.Height == 0)
                return -1;

            int renderObjectIndex = renderNodeIndexPool.AssignNextFreeIndex(out _);
            var sprite = new Sprite(width, height, renderDimensionReference);

            sprite.X = x;
            sprite.Y = y;
            sprite.Color = color;
            sprite.TextureAtlasOffset = textureAtlas.AddTexture(image);
            sprite.DisplayLayer = displayLayer++; // last draw call -> last rendering (= highest display layer)
            sprite.Layer = renderLayers[Layer.Controls];
            sprite.Visible = true;

            renderNodes.Add(renderObjectIndex, sprite);

            return renderObjectIndex;
        }
    }
}
