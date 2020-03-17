using System;
using System.Collections.Generic;

namespace Silk.NET.UI.Common
{
    internal class TypeSelector : Selector
    {
        private List<Type> types;

        public TypeSelector(params Type[] types)
        {
            this.types = new List<Type>(types);
        }

        protected override bool Match(Control control)
        {
            return types.Contains(control.GetType());
        }
    }
}
