using System;
using System.Collections.Generic;
using System.Linq;

namespace Silk.NET.UI
{
    internal class ClassSelector : Selector
    {
        internal override int Priority => 10000;
        private List<string> classes;

        public ClassSelector(params string[] classes)
        {
            this.classes = new List<string>(classes);
        }

        protected override bool Match(Control control, SelectorPathNode path)
        {
            if (control.Classes.Count == 0)
                return false;
                
            return classes.Any(clazz => control.Classes.Contains(clazz));
        }

        public override bool Equals(Selector other)
        {
            var otherClassSelector = other as ClassSelector;

            if (otherClassSelector == null || classes.Count != otherClassSelector.classes.Count)
                return false;

            for (int i = 0; i < classes.Count; ++i)
            {
                if (classes[i] != otherClassSelector.classes[i])
                    return false;
            }

            return true;
        }

        protected override int CalculateHashCode()
        {
            int hash = 17;

            hash = hash * 23 + Priority.GetHashCode();

            foreach (var clazz in classes)
                hash = hash * 23 + clazz.GetHashCode();

            return hash;
        }
    }
}
