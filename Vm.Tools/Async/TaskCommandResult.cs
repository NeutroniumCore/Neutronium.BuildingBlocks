using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Neutronium.MVVMComponents;

namespace Vm.Tools.Async 
{
    public class TaskCommandResult<TResult> : ViewModel, ICommandWithoutParameter
    {
        private bool _Computing;
        public bool Computing 
        {
            get => _Computing;
            private set
            {
                if (Set(ref _Computing, value))
                {
                    UpdateCommandStatus();
                }
            }
        }

        private bool _CanBeRun = true;
        public bool CanBeExecuted
        {
            get => _CanBeRun;
            set
            {
                if (Set(ref _CanBeRun, value))
                {
                    UpdateCommandStatus();
                }
            }
        }

        public IObservable<CommandResult<TResult>> Results { get; }

        public bool CanExecute { get; private set; }

        private event EventHandler<CommandResult<TResult>> _OnResult;
        public event EventHandler CanExecuteChanged;

        private readonly Func<Task<TResult>> _Compute;

        protected TaskCommandResult(Func<Task<TResult>> compute)
        {
            Results = Observable.FromEventPattern<CommandResult<TResult>>(evt => _OnResult += evt, evt => _OnResult -= evt) 
                                .Select(evtArg => evtArg.EventArgs);

            _Compute = compute;
        }

        private void UpdateCommandStatus()
        {
            var newValue = !_Computing && CanBeExecuted;
            if (CanExecute == newValue)
                return;

            CanExecute = newValue;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public async void Execute()
        {
            if (Computing)
                return;

            try
            {
                Computing = true;
                var result = await _Compute();             
                _OnResult?.Invoke(this, new CommandResult<TResult>(result));
            }
            catch (Exception exception)
            {
                _OnResult?.Invoke(this, new CommandResult<TResult>(exception));
            }
            finally
            {
                Computing = false;
            }
        }
    }
}
