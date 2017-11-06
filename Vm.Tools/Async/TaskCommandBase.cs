using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;

namespace Vm.Tools.Async
{
    public abstract class TaskCommandBase<TResult> : ViewModel
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
        public bool CanBeRun
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

        public ICommand Run => _Run;
        public ISimpleCommand Cancel { get; }
        public IObservable<CommandResult<TResult>> Results { get; }

        private readonly RelayToogleCommand _Run;
        private event EventHandler<CommandResult<TResult>> _OnResult;
        private CancellationTokenSource _CancellationTokenSource;

        protected TaskCommandBase()
        {
            Results = Observable.FromEventPattern<CommandResult<TResult>>(evt => _OnResult += evt, evt => _OnResult -= evt) 
                                .Select(evtArg => evtArg.EventArgs);

            Cancel = new RelaySimpleCommand(DoCancel);
            _Run = new RelayToogleCommand(DoCompute);
        }

        private void UpdateCommandStatus()
        {
            _Run.ShouldExecute = !_Computing && CanBeRun;
        }

        private void DoCancel()
        {
            _CancellationTokenSource?.Cancel();
        }

        protected abstract Task<TResult> Process(CancellationToken cancellationToken);

        private async void DoCompute()
        {
            _CancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _CancellationTokenSource.Token;
            try
            {
                Computing = true;
                var result = await Process(cancellationToken);
                Computing = false;
                _OnResult(this, cancellationToken.IsCancellationRequested ?
                                new CommandResult<TResult>() : new CommandResult<TResult>(result) );
            }
            catch (TaskCanceledException)
            {
                _OnResult(this, new CommandResult<TResult>());
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
