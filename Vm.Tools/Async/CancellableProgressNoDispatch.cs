using System;
using System.Threading;
using Vm.Tools.Standard;

namespace Vm.Tools.Async
{
    public class CancellableProgressNoDispatch<T> : IDisposableProgress<T>
    {
        private readonly Action<T> _Action;
        private readonly CancellationTokenSource _CancellationTokenSource;

        public bool Cancelled => _CancellationTokenSource.IsCancellationRequested;

        public CancellableProgressNoDispatch(Action<T> action, CancellationToken cancellationToken)
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