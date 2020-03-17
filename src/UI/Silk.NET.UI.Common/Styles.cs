using System.Collections.Generic;

namespace Silk.NET.UI.Common
{
    public abstract class Styles
    {
        private readonly Dictionary<Selector, Style> styles = new Dictionary<Selector, Style>();

        public void Add(Selector selector, Style style)
        {
            styles.Add(selector, style);
        }

        internal void Apply(Template template, Component component)
        {
            // TODO
        }
    }
}
