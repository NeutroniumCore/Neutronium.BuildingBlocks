using System;
using System.Reactive.Linq;
using Vm.Tools.Async;

namespace Vm.Tools.Dialog 
{
    public class ChooserCommand<T> : TaskCommandResult<string>, IChooserCommand<T> where T:IIODialog
    {
        public T Picker  { get;}

        IObservable<string> IChooserCommand<T>.Results => Results.Select(cr => cr.Result);

        public ChooserCommand(T picker):base(() => picker.Choose())
        {
            Picker = picker;
        }
    }
}
