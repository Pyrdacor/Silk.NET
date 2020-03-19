using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Silk.NET.UI.Common
{
    public static class ComponentManager
    {
        private static readonly Dictionary<string, List<string>> componentTypesByName = new Dictionary<string, List<string>>();
        private static readonly Dictionary<string, Type> componentTypesByFullName = new Dictionary<string, Type>();
        private static readonly Assembly entryAssembly = Assembly.GetEntryAssembly();

        public static void Run(Component rootComponent)
        {
            // find and register all component types
            foreach (var type in FindTypes((type) => type.IsSubclassOf(typeof(Component))))
            {
                componentTypesByFullName.Add(type.FullName, type);

                if (!componentTypesByName.ContainsKey(type.Name))
                    componentTypesByName.Add(type.Name, new List<string>());

                componentTypesByName[type.Name].Add(type.FullName);
            }

            // enter UI loop
            Loop(rootComponent);
        }

        internal static Component InitializeComponent(string name, string id)
        {
            if (componentTypesByFullName.ContainsKey(name))
                return Component.Create(componentTypesByFullName[name], id);

            if (!componentTypesByName.ContainsKey(name))
                throw new ArgumentException($"Unknown component {name}.");

            var possibleTypes = componentTypesByName[name];

            if (possibleTypes.Count != 1) // TODO: add the fully qualified names into the message
                throw new ArgumentException($"Component with type name {name} exists more than once. Specify it with fully qualified name.");

            return Component.Create(componentTypesByFullName[possibleTypes[0]], id);
        }

        private static void Loop(Component rootComponent)
        {
            // TODO: process UI events
            // TODO: draw / update UI
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