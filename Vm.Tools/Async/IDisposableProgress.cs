using System;

namespace Vm.Tools.Async
{
    public interface IDisposableProgress<in T>: IProgress<T> , IDisposable
    {
    }
}
