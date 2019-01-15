using System;
using System.Threading;
using Neutronium.BuildingBlocks.ApplicationTools;

namespace Neutronium.BuildingBlocks.Wpf.Async
{
    internal class CancellableProgressNoDispatch<T> : IDisposableProgress<T>
    {
        private readonly Action<T> _Action;
        private readonly CancellationTokenSource _CancellationTokenSource;

        public bool Cancelled => _CancellationTokenSource.IsCancellationRequested;

        internal CancellableProgressNoDispatch(Action<T> action, CancellationToken cancellationToken)
        {
            _CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _Action = action;
        }

        public void Stop()
        {
            _CancellationTokenSource.Cancel();
        }

        public void Report(T value)
        {
            if (Cancelled)
                return;

            _Action(value);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}