using System;
using System.Threading;
using System.Windows.Threading;
using Neutronium.BuildingBlocks.ApplicationTools;

namespace Neutronium.BuildingBlocks.Wpf.Async
{
    internal class CancellableProgress<T> : IDisposableProgress<T>
    {
        private readonly Action<T> _Action;
        private readonly Dispatcher _Dispatcher;
        private readonly CancellationTokenSource _CancellationTokenSource;
        private readonly DispatcherPriority _DispatcherPriority;

        public bool Cancelled => _CancellationTokenSource.IsCancellationRequested;

        internal CancellableProgress(Action<T> action, CancellationToken cancellationToken, DispatcherPriority priority = DispatcherPriority.Normal,
            Dispatcher dispatcher = null)
        {
            _CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _DispatcherPriority = priority;
            var localCancellationToken = _CancellationTokenSource.Token;

            _Action = (message) =>
            {
                if (!localCancellationToken.IsCancellationRequested)
                {
                    action(message);
                }
            };
            _Dispatcher = dispatcher ?? Dispatcher.CurrentDispatcher;
        }

        public void Stop()
        {
            _CancellationTokenSource.Cancel();
        }

        public void Report(T value)
        {
            if (!Cancelled)
                _Dispatcher.BeginInvoke(_Action, _DispatcherPriority, value);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}

