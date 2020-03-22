using System;
using System.Collections.Generic;

namespace Silk.NET.UI
{
    internal class IdSelector : Selector
    {
        internal override int Priority => 1000000;
        private List<string> ids;

        public IdSelector(params string[] ids)
        {
            this.ids = new List<string>(ids);
        }

        protected override bool Match(Control control, SelectorPathNode path)
        {
            return control.Id == null ? false : ids.Contains(control.Id);
        }

        public override bool Equals(Selector other)
        {
            var otherIdSelector = other as IdSelector;

            if (otherIdSelector == null || ids.Count != otherIdSelector.ids.Count)
                return false;

            for (int i = 0; i < ids.Count; ++i)
            {
                if (ids[i] != otherIdSelector.ids[i])
                    return false;
            }

            return true;
        }

        protected override int CalculateHashCode()
        {
            int hash = 17;

            hash = hash * 23 + Priority.GetHashCode();

            foreach (var id in ids)
                hash = hash * 23 + id.GetHashCode();

            return hash;
        }
    }
}
