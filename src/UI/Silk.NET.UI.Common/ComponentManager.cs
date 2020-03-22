using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Silk.NET.UI
{
    public static class ComponentManager
    {
        private static readonly Dictionary<string, List<string>> componentTypesByName = new Dictionary<string, List<string>>();
        private static readonly Dictionary<string, Type> componentTypesByFullName = new Dictionary<string, Type>();

        public static void Run(Type rootComponentType)
        {
            if (!rootComponentType.IsSubclassOf(typeof(RootComponent)))
                throw new ArgumentException($"The given type is not a subclass of `{nameof(RootComponent)}`.");

            IControlRenderer controlRenderer = null; // TODO: get it from somewhere
            var rootComponent = Component.Create(rootComponentType, null, true) as RootComponent;
            rootComponent.SetControlRenderer(controlRenderer);

            // find and register all component types
            foreach (var type in FindTypes((type) => type.IsSubclassOf(typeof(Component))))
            {
                componentTypesByFullName.Add(type.FullName, type);

                if (!componentTypesByName.ContainsKey(type.Name))
                    componentTypesByName.Add(type.Name, new List<string>());

                componentTypesByName[type.Name].Add(type.FullName);
            }

            // init root component and its view
            rootComponent.InitControl();

            // enter UI loop
            Loop(rootComponent);

            // destroy root component view
            rootComponent.DestroyView();
        }

        internal static Component InitializeComponent(string name, string id)
        {
            if (componentTypesByFullName.ContainsKey(name))
                return Component.Create(componentTypesByFullName[name], id, false);

            if (!componentTypesByName.ContainsKey(name))
                throw new ArgumentException($"Unknown component {name}.");

            var possibleTypes = componentTypesByName[name];

            if (possibleTypes.Count != 1) // TODO: add the fully qualified names into the message
                throw new ArgumentException($"Component with type name {name} exists more than once. Specify it with fully qualified name.");

            return Component.Create(componentTypesByFullName[possibleTypes[0]], id, false);
        }

        private static void Loop(Component rootComponent)
        {
            // TODO: process UI events
            // TODO: draw / update UI
            rootComponent.RenderControl();
        }

        private static IEnumerable<Type> FindTypes(Func<Type, bool> condition)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic)
                    .SelectMany(a => a.GetTypes())
                    .Where(t => condition(t));
        }
    }
}