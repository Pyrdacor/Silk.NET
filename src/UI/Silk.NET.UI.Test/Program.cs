using System.Drawing;
using System;
using Silk.NET.Windowing.Common;

namespace Silk.NET.UI.Test
{
    class Button : Control
    {
        public Button(string? id) : base(id) { }
    }

    class MyTemplate : Template
    {
        protected override void CreateFor(Component component)
        {
            new Button("foo").WithClasses("bar").AddTo(component);
        }
    }

    class MyStyles : Styles
    {
        public MyStyles()
        {
            Add(Selector.ForId("foo"), new Style()
            {
                Background = new BackgroundStyle {
                    Color = "yellow"
                },
            });
        }
    }

    [Template(typeof(MyTemplate))]
    [Styles(typeof(MyStyles))]
    class MyComponent : RootComponent
    {
        protected override void OnAfterViewInit()
        {
            Console.WriteLine("Id: " + Children[0].Id);
            // Console.WriteLine("background Color: " + Children[0].Style.Get<ColorValue?>("Background.Color").Value.Value.ToString());
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ComponentManager.Run(typeof(MyComponent));

            var window = Silk.NET.Windowing.Window.Create(WindowOptions.Default);
            var dimensions = new Silk.NET.UI.Renderer.OpenGL.RenderDimensionReference();
            dimensions.SetDimensions(window.Size.Width, window.Size.Height);
            var controlRenderer = new Silk.NET.UI.Renderer.OpenGL.ControlRenderer(dimensions);

            window.Render += (double foo) =>
            {
                window.MakeCurrent();
                controlRenderer.StartRenderCycle();
                controlRenderer.FillRectangle(100, 100, 200, 200, Color.Red);
                controlRenderer.EndRenderCycle();
                window.SwapBuffers();
            };

            window.Run();
        }
    }
}
