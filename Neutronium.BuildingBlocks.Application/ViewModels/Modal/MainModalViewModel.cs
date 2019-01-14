using System.ComponentModel;
using System.Threading.Tasks;
using Neutronium.BuildingBlocks.Application.WindowServices;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;

namespace Neutronium.BuildingBlocks.Application.ViewModels.Modal 
{
    /// <summary>
    /// Modal ViewModel. Internally used by <see cref="IMessageBox"/> implementation
    /// </summary>
    public class MainModalViewModel : MessageModalViewModel
    {
        public string CancelMessage { get; }

        public ISimpleCommand CancelCommand { get; }

        [Bindable(false)]
        public Task<bool> CompletionTask => _TaskCompletionSource.Task;

        private readonly TaskCompletionSource<bool> _TaskCompletionSource = new TaskCompletionSource<bool>();

        internal MainModalViewModel(ConfirmationMessage confirmationMessage) :base(confirmationMessage)
        {
            CancelMessage = confirmationMessage.CancelMessage;
            CancelCommand = new RelaySimpleCommand(Cancel);
        }

        protected override void Ok() => SetResult(true);

        private void Cancel() => SetResult(false);

        private void SetResult(bool value)
        {
            _TaskCompletionSource.TrySetResult(value);
        }
    }
}