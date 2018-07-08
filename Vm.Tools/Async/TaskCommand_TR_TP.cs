using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Vm.Tools.Async
{
    public sealed class TaskCommand<TResult, TProgress> : TaskCancellableCommandBase<TResult>
    {
        public IObservable<TProgress> Progress { get; }

        private event EventHandler<TProgress> _OnProgress;
        private readonly Func<CancellationToken, IProgress<TProgress>, TResult> _Process;

        public TaskCommand(Func<CancellationToken, IProgress<TProgress>, TResult> process, TimeSpan? throttleProgess=null)
        {
            _Process = process;

            var progess = Observable.FromEventPattern<TProgress>(evt => _OnProgress += evt, evt => _OnProgress -= evt)
                    .Select(evtArg => evtArg.EventArgs);

            Progress = throttleProgess.HasValue ? progess.Sample(throttleProgess.Value) : progess;
        }

        protected override async Task<TResult> Process(CancellationToken cancellationToken)
        {
            using (var progress = new CancellableProgress<TProgress>(OnProgress, cancellationToken))
            {
                return await Task.Run(() => _Process(cancellationToken, progress), cancellationToken);
            }
        }

        private void OnProgress(TProgress progress)
        {
            _OnProgress?.Invoke(this, progress);
        }
    }
}
