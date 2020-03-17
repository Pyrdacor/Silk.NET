using System;
using System.Collections.Generic;
using System.Linq;

namespace Silk.NET.UI.Common
{
    internal class ClassSelector : Selector
    {
        private List<string> classes;

        public ClassSelector(params string[] classes)
        {
            this.classes = new List<string>(classes);
        }

        protected override bool Match(Control control)
        {
            if (control.Classes.Count == 0)
                return false;
                
            return classes.Any(clazz => control.Classes.Contains(clazz));
        }
    }
}
