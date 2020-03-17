using System.Collections.Specialized;

namespace Silk.NET.UI.Common
{
    public abstract class ContainerControl : Control
    {
        public ControlList Children => base.InternalChildren;
        
        public event NotifyCollectionChangedEventHandler ChildrenChanged
        {
            add => Children.CollectionChanged += value;
            remove => Children.CollectionChanged -= value;
        }

        public ContainerControl(string? id)
            : base(id)
        {

        }
    }
}
