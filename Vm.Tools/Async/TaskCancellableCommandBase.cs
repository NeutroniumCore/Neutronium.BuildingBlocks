using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;
using Vm.Tools.Standard;

namespace Vm.Tools.Async
{
    public abstract class TaskCancellableCommand<TResult> : ViewModel, ITaskCancellableCommand<TResult>
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

        private bool _CanBeExecuted = true;
        public bool CanBeExecuted
        {
            get => _CanBeExecuted;
            set
            {
                if (Set(ref _CanBeExecuted, value))
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

        protected TaskCancellableCommand()
        {
            Results = Observable.FromEventPattern<CommandResult<TResult>>(evt => _OnResult += evt, evt => _OnResult -= evt) 
                                .Select(evtArg => evtArg.EventArgs);

            Cancel = new RelaySimpleCommand(DoCancel);
            _Run = new RelayToogleCommand(DoCompute);
        }

        private void UpdateCommandStatus()
        {
            _Run.ShouldExecute = !_Computing && CanBeExecuted;
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
                Publish(cancellationToken.IsCancellationRequested ? new CommandResult<TResult>() : new CommandResult<TResult>(result) );
            }
            catch (TaskCanceledException)
            {
                Publish(new CommandResult<TResult>());
            }
            catch (Exception exception)
            {
                Publish(new CommandResult<TResult>(exception));
            }
            finally
            {
                _CancellationTokenSource.Cancel();
                Computing = false;
            }
        }

        private void Publish(CommandResult<TResult> result)
        {
            _OnResult?.Invoke(this, result);
        }
    }
}
