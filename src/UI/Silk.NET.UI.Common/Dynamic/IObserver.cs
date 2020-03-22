using System;

namespace Silk.NET.UI.Dynamic
{
    public interface IObserver<T>
    {
        void Next(T value);
        void Error(Exception exception);
        void Complete();
    }
}