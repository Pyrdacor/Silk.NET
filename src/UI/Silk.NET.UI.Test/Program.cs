using System;
using Silk.NET.UI.Common;

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
        }
    }
}
