using System;
using System.Collections.Generic;

namespace Silk.NET.UI.Common
{
    internal class TypeSelector : Selector
    {
        internal override int Priority => 100;
        private List<Type> types;

        public TypeSelector(params Type[] types)
        {
            this.types = new List<Type>(types);
        }

        protected override bool Match(Control control, SelectorPathNode path)
        {
            return types.Contains(control.GetType());
        }

        public override bool Equals(Selector other)
        {
            var otherIdSelector = other as TypeSelector;

            if (otherIdSelector == null || types.Count != otherIdSelector.types.Count)
                return false;

            for (int i = 0; i < types.Count; ++i)
            {
                if (types[i] != otherIdSelector.types[i])
                    return false;
            }

            return true;
        }

        protected override int CalculateHashCode()
        {
            int hash = 17;

            hash = hash * 23 + Priority.GetHashCode();

            foreach (var type in types)
                hash = hash * 23 + type.GetHashCode();

            return hash;
        }
    }
}
