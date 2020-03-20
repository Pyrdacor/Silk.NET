using System.Runtime.CompilerServices;
using System;

namespace Silk.NET.UI.Common
{
    public abstract class Selector
    {
        internal abstract int Priority { get; }
        public string Name { get; }

        protected Selector([CallerMemberName] string name = null)
        {
            Name = name ?? nameof(Selector);
        }

        internal bool MatchControl(Control control) => Match(control);

        protected abstract bool Match(Control control);

        public static Selector ForType(params Type[] type)
        {
            return new TypeSelector(type);
        }

        public static Selector ForId(params string[] id)
        {
            return new IdSelector(id);
        }

        public static Selector ForClass(params string[] clazz)
        {
            return new ClassSelector(clazz);
        }
    }
}
