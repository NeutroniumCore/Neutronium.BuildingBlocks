using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Neutronium.BuildingBlocks.ApplicationTools;

namespace Neutronium.BuildingBlocks.Wpf.Async
{
    public sealed class TaskCommand<TResult, TProgress> : TaskCancellableCommand<TResult>, ITaskCancellableCommand<TResult, TProgress>
    {
        public IObservable<TProgress> Progress { get; }

        private event EventHandler<TProgress> _OnProgress;
        private readonly Func<CancellationToken, IProgress<TProgress>, TResult> _Process;
        private readonly TimeSpan? _ThrottleProgess;
        private CancellationToken _CancellationToken;

        public TaskCommand(Func<CancellationToken, IProgress<TProgress>, TResult> process, TimeSpan? throttleProgess = null)
        {
            _Process = process;
            _ThrottleProgess = throttleProgess;

            var progess = Observable.FromEventPattern<TProgress>(evt => _OnProgress += evt, evt => _OnProgress -= evt)
                    .Select(evtArg => evtArg.EventArgs);

            Progress = _ThrottleProgess.HasValue ? progess.Sample(_ThrottleProgess.Value).ObserveOn(SynchronizationContext.Current).Where(_ => !_CancellationToken.IsCancellationRequested) : progess;
        }

        protected override async Task<TResult> Process(CancellationToken cancellationToken)
        {
            _CancellationToken = cancellationToken;
            using (var progress = GetProgress(cancellationToken))
            {
                return await Task.Run(() => _Process(cancellationToken, progress), cancellationToken);
            }
        }

        private IDisposableProgress<TProgress> GetProgress(CancellationToken cancellationToken)
        {
            void OnProgress(TProgress progress)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                _OnProgress?.Invoke(this, progress);
            }

            return _ThrottleProgess.HasValue ? (IDisposableProgress<TProgress>)new CancellableProgressNoDispatch<TProgress>(OnProgress, cancellationToken) : new CancellableProgress<TProgress>(OnProgress, cancellationToken);
        }
    }
}
