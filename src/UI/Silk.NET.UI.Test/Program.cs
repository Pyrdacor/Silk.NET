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
                    Color = "#ff00ff"
                },
            });
        }
    }

    [Template(typeof(MyTemplate))]
    [Styles(typeof(MyStyles))]
    class MyComponent : RootComponent
    {
        
    }

    class Program
    {
        static void Main(string[] args)
        {
            ComponentManager.Run(typeof(MyComponent));
        }
    }
}
