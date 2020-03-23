using System;

namespace Silk.NET.UI
{
    public interface IObserver<T>
    {
        void Next(T value);
        void Error(Exception exception);
        void Complete();
    }
}