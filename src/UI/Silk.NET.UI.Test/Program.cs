using System.Drawing;
using System;
using Silk.NET.Windowing.Common;

namespace Silk.NET.UI.Test
{
    class Button : Control
    {
        private int? drawReference = null;

        public Button(string? id) : base(id) { }

        protected override void OnRender(RenderEventArgs args)
        {
            drawReference = args.Renderer.FillRectangle(drawReference, 20, 20, 100, 60, Color.LightGray);

            base.OnRender(args);
        }
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
            // Test output
            Console.WriteLine("Id: " + Children[0].Id);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var window = Silk.NET.Windowing.Window.Create(WindowOptions.Default);

            ComponentManager.Run(typeof(MyComponent), window, new Silk.NET.UI.Renderer.OpenGL.ControlRendererFactory());
        }
    }
}
