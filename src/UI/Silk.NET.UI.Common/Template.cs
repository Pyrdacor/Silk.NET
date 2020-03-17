using System.Runtime.CompilerServices;

namespace Silk.NET.UI.Common
{
    public abstract class Template
    {
        public string? Name { get; }

        protected Template([CallerMemberName] string? name = null)
        {
            Name = name ?? nameof(Template);
        }

        internal protected abstract void CreateFor(Component component);
    }
}
