using System.Drawing;

namespace Silk.NET.UI
{
    public interface IControlRenderer
    {
        void StartRenderCycle();
        void EndRenderCycle();
        void RemoveRenderObject(int renderObjectIndex);
        int DrawRectangle(int x, int y, int width, int height, Color color, int lineSize);
        int FillRectangle(int x, int y, int width, int height, Color color);
        int DrawLine(int x1, int y1, int x2, int y2, Color color);
        int DrawImage(int x, int y, Image image, Color? colorOverlay = null);
    }
}