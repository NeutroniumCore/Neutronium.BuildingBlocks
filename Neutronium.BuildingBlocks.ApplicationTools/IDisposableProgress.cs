using System;

namespace Neutronium.BuildingBlocks.ApplicationTools
{

    /// <summary>
    /// Disposable progress 
    /// </summary>
    /// <typeparam name="T">
    /// Progress type.
    /// </typeparam>
    public interface IDisposableProgress<in T>: IProgress<T> , IDisposable
    {
    }
}
