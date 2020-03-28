using System.Drawing;

namespace Silk.NET.UI
{
    public enum LineStyle
    {
        Solid,
        Dotted,
        Dashed
    }

    public interface IControlRenderer
    {
        void StartRenderCycle();
        void EndRenderCycle();
        void RemoveRenderObject(int renderObjectIndex);
        int DrawRectangle(int x, int y, int width, int height, Color color, int lineSize);
        int FillRectangle(int x, int y, int width, int height, Color color);
        int DrawRectangleLine(int x, int y, int width, int height, Color color, LineStyle lineStyle);
        int DrawImage(int x, int y, Image image, Color? colorOverlay = null);
        int DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color);
    }

    public interface IControlRendererFactory
    {
        IControlRenderer CreateControlRenderer(Windowing.Common.IView view);
    }
}