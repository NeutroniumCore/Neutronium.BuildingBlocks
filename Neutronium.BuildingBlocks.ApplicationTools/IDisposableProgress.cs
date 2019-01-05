using System;

namespace Neutronium.BuildingBlocks.ApplicationTools
{
    public interface IDisposableProgress<in T>: IProgress<T> , IDisposable
    {
    }
}
