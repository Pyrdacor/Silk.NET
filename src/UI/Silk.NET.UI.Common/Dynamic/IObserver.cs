using System;

namespace Silk.NET.UI.Common.Dynamic
{
    public interface IObserver<T>
    {
        void Next(T value);
        void Error(Exception exception);
        void Complete();
    }
}