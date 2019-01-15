using Neutronium.BuildingBlocks.ApplicationTools;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neutronium.BuildingBlocks.Wpf.Async
{
    /// <summary>
    /// <see cref="ITaskCancellableCommand{TResult, TProgress}"/> Wpf implementation
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TProgress"></typeparam>
    public sealed class TaskCommand<TResult, TProgress> : TaskCancellableCommand<TResult>, ITaskCancellableCommand<TResult, TProgress>
    {
        public IObservable<TProgress> Progress { get; }

        private event EventHandler<TProgress> _OnProgress;
        private readonly Func<CancellationToken, IProgress<TProgress>, TResult> _Process;
        private readonly TimeSpan? _ThrottleProgress;
        private CancellationToken _CancellationToken;

        /// <summary>
        /// Construct a new TaskCommand
        /// </summary>
        /// <param name="process">
        /// Function to be executed in the command.
        /// Will be dispatcher in a Task.
        /// </param>
        /// <param name="throttleProgress">
        /// If not null TimeSpan used to throttle progress events
        /// </param>
        public TaskCommand(Func<CancellationToken, IProgress<TProgress>, TResult> process, TimeSpan? throttleProgress = null)
        {
            _Process = process;
            _ThrottleProgress = throttleProgress;

            var progress = Observable.FromEventPattern<TProgress>(evt => _OnProgress += evt, evt => _OnProgress -= evt)
                    .Select(evtArg => evtArg.EventArgs);

            Progress = _ThrottleProgress.HasValue ? progress.Sample(_ThrottleProgress.Value).ObserveOn(SynchronizationContext.Current).Where(_ => !_CancellationToken.IsCancellationRequested) : progress;
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

            return _ThrottleProgress.HasValue ? (IDisposableProgress<TProgress>)new CancellableProgressNoDispatch<TProgress>(OnProgress, cancellationToken) : new CancellableProgress<TProgress>(OnProgress, cancellationToken);
        }
    }
}
