using System;
using System.Collections.Generic;
using System.Drawing;

namespace Silk.NET.UI
{
    public class ControlRenderer
    {
        private IControlRenderer _renderer;
        private List<int> _lastRenderObjects;
        private readonly List<int> _currentRenderObjects = new List<int>();
        public bool ForceRedraw { get; set; } = false;

        internal ControlRenderer(IControlRenderer renderer)
        {
            _renderer = renderer;
        }

        internal void Init()
        {
            _lastRenderObjects = new List<int>(_currentRenderObjects);
            _currentRenderObjects.Clear();
            _renderer.StartRenderCycle();
        }

        internal void Render()
        {
            // remove no longer existing render objects
            foreach (var removedRenderObject in _lastRenderObjects)
            {
                _renderer.RemoveRenderObject(removedRenderObject);
            }

            _renderer.EndRenderCycle();
        }

        private int RunDrawCall(int? reference, Func<int> drawActionWrapper)
        {
            int renderObjectIndex;

            if (reference == null || !_lastRenderObjects.Contains(reference.Value))
            {
                renderObjectIndex = drawActionWrapper();
            }
            else
            {
                if (ForceRedraw)
                {
                    _renderer.RemoveRenderObject(reference.Value);
                    renderObjectIndex = drawActionWrapper();
                    _currentRenderObjects.Add(renderObjectIndex);
                    _lastRenderObjects.Remove(reference.Value);
                    return renderObjectIndex;
                }
                else
                    renderObjectIndex = reference.Value;
            }

            _currentRenderObjects.Add(renderObjectIndex);
            _lastRenderObjects.Remove(renderObjectIndex);
            return renderObjectIndex;
        }

        public void RemoveRenderObject(int renderObjectIndex)
        {
            _renderer.RemoveRenderObject(renderObjectIndex);
        }

        public int DrawRectangle(int? reference, int x, int y, int width, int height, Color color, int lineSize)
        {
            return RunDrawCall(reference, () => _renderer.DrawRectangle(x, y, width, height, color, lineSize));
        }

        public int FillRectangle(int? reference, int x, int y, int width, int height, Color color)
        {
            return RunDrawCall(reference, () => _renderer.FillRectangle(x, y, width, height, color));
        }

        public int DrawRectangleLine(int? reference, int x, int y, int width, int height, Color color, LineStyle lineStyle)
        {
            return RunDrawCall(reference, () => _renderer.DrawRectangleLine(x, y, width, height, color, lineStyle));
        }

        public int DrawImage(int? reference, int x, int y, Image image, Color? colorOverlay = null)
        {
            return RunDrawCall(reference, () => _renderer.DrawImage(x, y, image, colorOverlay));
        }

        public int FillTriangle(int? reference, int x1, int y1, int x2, int y2, int x3, int y3, Color color)
        {
            return RunDrawCall(reference, () => _renderer.FillTriangle(x1, y1, x2, y2, x3, y3, color));
        }

        public int FillPolygon(int? reference, Color color, params Point[] points)
        {
            return RunDrawCall(reference, () => _renderer.FillPolygon(color, points));
        }
    }
}