using System;

namespace Silk.NET.UI
{
    internal class NegateSelector : Selector
    {
        internal override int Priority { get; }
        private Selector _selector;

        public NegateSelector(Selector selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            Priority = selector.Priority;
            _selector = selector;
        }

        protected override bool Match(Control control, SelectorPathNode path)
        {
            return !_selector.MatchControl(control, path);
        }

        public override bool Equals(Selector other)
        {
            var otherNegateSelector = other as NegateSelector;

            return otherNegateSelector != null &&
                _selector == otherNegateSelector._selector;
        }

        protected override int CalculateHashCode()
        {
            int hash = 17;

            hash = hash * 23 + _selector.GetHashCode();

            return hash;
        }
    }
}