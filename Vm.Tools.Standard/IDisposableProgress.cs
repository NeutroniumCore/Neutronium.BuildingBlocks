using System;

namespace Vm.Tools.Standard
{
    public interface IDisposableProgress<in T>: IProgress<T> , IDisposable
    {
    }
}
