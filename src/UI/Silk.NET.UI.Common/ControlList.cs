using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Silk.NET.UI
{
    public sealed class ControlList : IList<Control>, INotifyCollectionChanged
    {
        private Control parent;
        private List<Control> controls = new List<Control>();

        internal ControlList(Control parent)
        {
            this.parent = parent;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void InvokeCollectionChange(NotifyCollectionChangedAction action,
            int startIndex, int count, List<Control> controls = null)
        {
            NotifyCollectionChangedEventArgs args;
            List<Control> changedControls = controls ?? this.controls.GetRange(startIndex, count);

            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    args = new NotifyCollectionChangedEventArgs(action, changedControls, startIndex);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid collection changed action: {action}");
            }

            CollectionChanged?.Invoke(parent, args);
        }

        public Control this[int index]
        {
            get => controls[index];
            set
            {
                if (controls[index] != value)
                {
                    controls[index] = value;
                    InvokeCollectionChange(NotifyCollectionChangedAction.Replace, index, 1);
                }
            }
        }

        public int Count => controls.Count;
        public bool IsReadOnly => false;

        public void Add(IEnumerable<Control> controls)
        {
            this.controls.AddRange(controls);
            int count = controls.Count();
            InvokeCollectionChange(NotifyCollectionChangedAction.Add,
                this.controls.Count - count, count);
        }

        public void Add(params Control[] controls)
        {
            if (controls.Length == 0)
                return;

            this.controls.AddRange(controls);
            InvokeCollectionChange(NotifyCollectionChangedAction.Add,
                this.controls.Count - controls.Length, controls.Length);
        }

        public void Add(Control control)
        {
            this.controls.Add(control);
            InvokeCollectionChange(NotifyCollectionChangedAction.Add,
                this.controls.Count - 1, 1);
        }

        public void Insert(int index, IEnumerable<Control> controls)
        {
            this.controls.InsertRange(index, controls);
            InvokeCollectionChange(NotifyCollectionChangedAction.Add,
                index, controls.Count());
        }

        public void Insert(int index, params Control[] controls)
        {
            if (controls.Length == 0)
                return;

            this.controls.InsertRange(index, controls);
            InvokeCollectionChange(NotifyCollectionChangedAction.Add,
                index, controls.Length);
        }

        public void Insert(int index, Control control)
        {
            this.controls.Insert(index, control);
            InvokeCollectionChange(NotifyCollectionChangedAction.Add,
                index, 1);
        }

        public bool Remove(Control control)
        {
            int index = controls.IndexOf(control);
            if (controls.Remove(control))
            {
                CollectionChanged?.Invoke(parent, new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove, control, index
                ));
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            var control = controls[index];
            controls.RemoveAt(index);
            CollectionChanged?.Invoke(parent, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove, control, index
            ));
        }

        public void RemoveRange(int index, int count)
        {
            var removedControls = controls.GetRange(index, count);
            controls.RemoveRange(index, count);
            InvokeCollectionChange(NotifyCollectionChangedAction.Remove, index, count, removedControls);
        }

        public void Clear()
        {
            controls.Clear();
            CollectionChanged?.Invoke(parent, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset
            ));
        }

        public int IndexOf(Control control)
        {
            return controls.IndexOf(control);
        }

        public bool Contains(Control control)
        {
            return controls.Contains(control);
        }

        public void CopyTo(Control[] array, int arrayIndex)
        {
            controls.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Control> GetEnumerator()
        {
            return controls.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (controls as IEnumerable).GetEnumerator();
        }
    }
}
