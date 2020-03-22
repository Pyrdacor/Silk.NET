using System;
using System.Collections.Generic;
using System.Drawing;

namespace Silk.NET.UI
{
    public class ControlRenderer
    {
        private Control control;
        private IControlRenderer renderer;
        private List<int> lastRenderObjects;
        private readonly List<int> currentRenderObjects = new List<int>();

        internal ControlRenderer(Control control, IControlRenderer renderer)
        {
            this.control = control; // TODO: needed
            this.renderer = renderer;
        }

        internal void Init()
        {
            lastRenderObjects = new List<int>(currentRenderObjects);
            currentRenderObjects.Clear();
            renderer.StartRenderCycle();
        }

        internal void Render()
        {
            // remove no longer existing render objects
            foreach (var removedRenderObject in lastRenderObjects)
            {
                renderer.RemoveRenderObject(removedRenderObject);
            }

            renderer.EndRenderCycle();
        }

        private int RunDrawCall(int? reference, Func<int> drawActionWrapper)
        {
            int renderObjectIndex;

            if (reference == null || !lastRenderObjects.Contains(reference.Value))
            {
                renderObjectIndex = drawActionWrapper();
            }
            else
            {
                renderObjectIndex = reference.Value;
            }

            currentRenderObjects.Add(renderObjectIndex);
            return renderObjectIndex;
        }

        public int DrawRectangle(int? reference, int x, int y, int width, int height, Color color, int lineSize)
        {
            return RunDrawCall(reference, () => renderer.DrawRectangle(x, y, width, height, color, lineSize));
        }

        public int FillRectangle(int? reference, int x, int y, int width, int height, Color color)
        {
            return RunDrawCall(reference, () => renderer.FillRectangle(x, y, width, height, color));
        }

        public int DrawLine(int? reference, int x1, int y1, int x2, int y2, Color color)
        {
            return RunDrawCall(reference, () => renderer.FillRectangle(x1, y1, x2, y2, color));
        }

        public int DrawImage(int? reference, int x, int y, Image image, Color? colorOverlay = null)
        {
            return RunDrawCall(reference, () => renderer.DrawImage(x, y, image, colorOverlay));
        }
    }
}