using System;
using Neutronium.MVVMComponents;

namespace Vm.Tools.Dialog
{
    public interface IChooserCommand<out T>: ICommandWithoutParameter where T : IIODialog
    {
        IObservable<string> Results { get; }

        bool CanBeExecuted { get; set; }

        T Picker { get; }
    }
}
