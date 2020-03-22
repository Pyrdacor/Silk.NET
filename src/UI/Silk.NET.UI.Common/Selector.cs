using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Silk.NET.UI.Common
{
    public class SelectorPathNode
    {
        public SelectorPathNode Prev = null;
        public Control Control = null;
    }

    public abstract class Selector : IEquatable<Selector>
    {
        internal abstract int Priority { get; }
        public string Name { get; }

        protected Selector([CallerMemberName] string name = null)
        {
            Name = name ?? nameof(Selector);
        }

        internal bool MatchControl(Control control, SelectorPathNode path) => Match(control, path);

        protected abstract bool Match(Control control, SelectorPathNode path);

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

        public abstract bool Equals(Selector other);

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return this.Equals((Selector)obj);
        }

        protected abstract int CalculateHashCode();

        public override int GetHashCode()
        {
            return CalculateHashCode();
        }

        public static bool operator ==(Selector lhs, Selector rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Selector lhs, Selector rhs)
        {
            return !(lhs == rhs);
        }
    }
}
