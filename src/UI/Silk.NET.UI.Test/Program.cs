using System.Drawing;
using System;
using Silk.NET.Windowing.Common;
using Silk.NET.UI.Controls;

namespace Silk.NET.UI.Test
{
    class Button : Control
    {
        private int? _drawReference = null;

        public Button(string? id) : base(id) { }

        protected override void OnRender(RenderEventArgs args)
        {
            // This is only test drawing code
            _drawReference = args.Renderer.FillRectangle(_drawReference, 200, 200, 100, 60, Color.LightGray);

            base.OnRender(args);
        }
    }

    class MyTemplate : Template
    {
        protected override void CreateFor(Component component)
        {
            new Button("foo").WithClasses("bar").AddTo(component);
            new Panel("panel").AddTo(component);
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
                BorderColor = "black"
            });
            Add(Selector.ForType(typeof(Panel)), new Style()
            {
                BackgroundColor = Color.Beige,
                BorderSize = 1,
                BorderColor = "black"
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

            var panel = this.Children["panel"];

            panel.X = 10;
            panel.Y = 10;
            panel.Width = 150;
            panel.Height = 120;
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
