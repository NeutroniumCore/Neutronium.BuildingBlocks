using System.Threading.Tasks;
using Neutronium.BuildingBlocks.Application.Navigation;
using Neutronium.BuildingBlocks.Application.ViewModel.Modal;
using Neutronium.BuildingBlocks.Application.WindowServices;
using Neutronium.MVVMComponents;

namespace Neutronium.BuildingBlocks.Application.ViewModel
{
    public class ApplicationViewModel<T> : Neutronium.BuildingBlocks.ApplicationTools.ViewModel, IMessageBox, INotificationSender
    {
        public T ApplicationInformation { get; }
        public IWindowViewModel Window { get; }
        public NavigationViewModel Router { get; }

        public object CurrentViewModel { get; set; }

        private MessageModalViewModel _Modal;
        public MessageModalViewModel Modal
        {
            get => _Modal;
            private set => Set(ref _Modal, value);
        }

        private Notification _Notification = null;
        public Notification Notification
        {
            get => _Notification;
            set => Set(ref _Notification, value);
        }

        public ApplicationViewModel(IWindowViewModel window, NavigationViewModel router, T applicationInformation)
        {
            Window = window;
            Router = router;
            ApplicationInformation = applicationInformation;
        }

        public Task<bool> ShowMessage(ConfirmationMessage confirmationMessage)
        {
            var modal = new MainModalViewModel(confirmationMessage);
            Modal = modal;
            return modal.CompletionTask;
        }

        public void ShowInformation(MessageInformation messageInformation)
        {
            var modal = new MessageModalViewModel(messageInformation);
            Modal = modal;
        }

        public void Send(Notification notification)
        {
            Notification = notification;
        }
    }
}

