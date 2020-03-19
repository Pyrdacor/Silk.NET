using System;

namespace Silk.NET.UI.Common.Dynamic
{
    public abstract class Observable<T>
    {
        protected Action<T> nextAction = null;
        protected Action<Exception> errorAction = null;
        protected Action completeAction = null;

        public virtual void Subscribe(Action<T> next, Action<Exception> error = null, Action complete = null)
        {
            nextAction = next;
            errorAction = error;
            completeAction = complete;
        }
    }
}