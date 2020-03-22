using System;
using System.Collections.Generic;
using System.Linq;

namespace Silk.NET.UI.Dynamic
{
    internal class MultipleComponentBinder<T> : ComponentBinder
    {
        private Observable<IEnumerable<T>> values;
        private readonly List<Component> boundComponents = new List<Component>();
        private string componentTypeName;
        private Func<T, string> componentIdProvider;

        public MultipleComponentBinder(Observable<IEnumerable<T>> values, string componentTypeName, Func<T, string> componentIdProvider)
        {
            this.values = values;
            this.componentTypeName = componentTypeName;
            this.componentIdProvider = componentIdProvider;
        }

        public override void Bind(Component parentComponent)
        {
            values.Subscribe(result =>
            {
                foreach (var boundComponent in boundComponents)
                {
                    boundComponent.DestroyControl();
                }

                boundComponents.Clear();

                foreach (var value in result)
                {
                    var boundComponent = ComponentManager.InitializeComponent(componentTypeName,
                        componentIdProvider == null ? null : componentIdProvider(value));
                    boundComponent.AddTo(parentComponent);
                    boundComponents.Add(boundComponent);
                }
            });
        }
    }

    internal class MultipleComponentBinder : MultipleComponentBinder<int>
    {
        public MultipleComponentBinder(Observable<int> count, string componentTypeName, Func<int, string> componentIdProvider)
            : base(count.Map(c => Enumerable.Range(0, c)), componentTypeName, componentIdProvider)
        {

        }
    }
}
