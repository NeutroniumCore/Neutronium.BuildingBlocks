using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neutronium.BuildingBlocks.Wpf.Async
{
    /// <summary>
    /// <see cref="ITaskCancellableCommand{T}"/> Wpf implementation
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class TaskCommand<TResult> : TaskCancellableCommand<TResult>
    {
        private readonly Func<CancellationToken, TResult> _Process;

        /// <summary>
        /// Construct a new TaskCommand
        /// </summary>
        /// <param name="process">
        /// Function to be executed in the command.
        /// Will be dispatcher in a Task.
        /// </param>
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
