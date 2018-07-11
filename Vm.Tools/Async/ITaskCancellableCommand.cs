using System;
using System.ComponentModel;
using System.Windows.Input;
using Neutronium.MVVMComponents;

namespace Vm.Tools.Async
{
    public interface ITaskCancellableCommand<TResult> : INotifyPropertyChanged, INotifyPropertyChanging
    {
        bool Computing { get; }
        bool CanBeExecuted { get; set; }
        ICommand Run { get; }
        ISimpleCommand Cancel { get; }
        IObservable<CommandResult<TResult>> Results { get; }
    }


    public interface ITaskCancellableCommand<TResult, out TProgress> : ITaskCancellableCommand<TResult>
    {
        IObservable<TProgress> Progress { get; }
    }
}
