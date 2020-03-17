using System;
using System.Collections.Generic;

namespace Silk.NET.UI.Common
{
    internal class IdSelector : Selector
    {
        private List<string> ids;

        public IdSelector(params string[] ids)
        {
            this.ids = new List<string>(ids);
        }

        protected override bool Match(Control control)
        {
            return control.Id == null ? false : ids.Contains(control.Id);
        }
    }
}
