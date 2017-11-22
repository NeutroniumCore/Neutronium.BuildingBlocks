using System;
using System.Reactive.Linq;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;

namespace Vm.Tools.Dialog 
{
    public class ChooserCommand<T> : IChooserCommand<T> where T:IIODialog
    {
        public bool CanBeExecuted
        {
            get { return Choose.CanExecute; }
            set { _ChooseCommand.ShouldExecute = value; }
        }

        public T Picker { get; }

        public IObservable<string> Results { get; }

        private readonly RelayToogleCommand _ChooseCommand;
        public ICommandWithoutParameter Choose => _ChooseCommand;

        private event EventHandler<string> _OnResult;

        public ChooserCommand(T picker)
        {
            Picker = picker;
            _ChooseCommand = new RelayToogleCommand(DoChoose);
            Results = Observable.FromEventPattern<string>(evt => _OnResult += evt, evt => _OnResult -= evt)
                .Select(evtArg => evtArg.EventArgs);
        }

        private void DoChoose()
        {
            var file = Picker.Choose();
            _OnResult?.Invoke(this, file);
        }
    }
}
