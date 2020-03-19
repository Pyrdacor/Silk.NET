using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Silk.NET.UI.Common
{
    public abstract class Template
    {
        public string Name { get; }

        protected Template([CallerMemberName] string name = null)
        {
            Name = name ?? nameof(Template);
        }

        internal protected abstract void CreateFor(Component component);

        protected void Add(string componentTypeName, string id = null)
        {

        }

        protected void AddMultiple(string componentTypeName, int count, Func<int, string> idProvider = null)
        {
            
        }

        protected void AddForEach<T>(string componentTypeName, IEnumerable<T> collection, Func<T, string> idProvider = null)
        {
            
        }

        protected void AddIf(string componentTypeName, string id = null)
        {

        }
    }
}
