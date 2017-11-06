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
            get { return _Computing; }
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
            get { return _CanBeRun; }
            set
            {
                if (Set(ref _CanBeRun, value))
                {
                    UpdateCommandStatus();
                }
            }
        }

        public IObservable<CommandResult<TResult>> Results { get; }

        private bool _CanExecute;
        public bool CanExecute => _CanExecute;

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
            if (_CanExecute == newValue)
                return;

            _CanExecute = newValue;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public async void Execute()
        {
            try
            {
                Computing = true;
                var result = await _Compute();
                Computing = false;
                _OnResult(this, new CommandResult<TResult>(result));
            }
            catch (Exception exception)
            {
                _OnResult(this, new CommandResult<TResult>(exception));
            }
            finally
            {
                Computing = false;
            }
        }
    }
}
