using System;
using System.ComponentModel;
using System.Windows.Input;
using Neutronium.MVVMComponents;

namespace Neutronium.BuildingBlocks.ApplicationTools
{
    /// <summary>
    /// Cancellable command
    /// </summary>
    /// <typeparam name="TResult">
    /// Result type
    /// </typeparam>
    public interface ITaskCancellableCommand<TResult> : INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <summary>
        /// True if the command is executing
        /// </summary>
        bool Computing { get; }

        /// <summary>
        /// True if the command can be executed
        /// </summary>
        bool CanBeExecuted { get; set; }

        /// <summary>
        /// Execute the command
        /// </summary>
        ICommand Run { get; }

        /// <summary>
        /// Cancel the current execution
        /// </summary>
        ISimpleCommand Cancel { get; }

        /// <summary>
        /// Results exposed as Observable <see cref="CommandResult{TResult}"/>
        /// </summary>
        IObservable<CommandResult<TResult>> Results { get; }
    }


    /// <summary>
    /// Cancellable command with progress
    /// </summary>
    /// <typeparam name="TResult">
    /// Result type
    /// </typeparam>
    /// <typeparam name="TProgress">
    /// Progress type
    /// </typeparam>
    public interface ITaskCancellableCommand<TResult, out TProgress> : ITaskCancellableCommand<TResult>
    {
        /// <summary>
        /// Command progress exposed as Observable 
        /// </summary>
        IObservable<TProgress> Progress { get; }
    }
}
