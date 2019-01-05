using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neutronium.BuildingBlocks.Wpf.Async
{
    public sealed class TaskCommand<TResult> : TaskCancellableCommand<TResult>
    {
        private readonly Func<CancellationToken, TResult> _Process;

        public TaskCommand(Func<CancellationToken, TResult> process)
        {
            _Process = process;
        }

        protected override Task<TResult> Process(CancellationToken cancellationToken)
        {
            return Task.Run(() => _Process(cancellationToken), cancellationToken);
        }
    }
}
