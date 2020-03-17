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

    class MyComponent : Component
    {
        
    }

    class Program
    {
        static void Main(string[] args)
        {
            
        }
    }
}
