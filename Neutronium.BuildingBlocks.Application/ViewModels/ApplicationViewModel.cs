using System;
using System.Threading.Tasks;
using Neutronium.BuildingBlocks.Application.Navigation;
using Neutronium.BuildingBlocks.Application.ViewModels.Modal;
using Neutronium.BuildingBlocks.Application.WindowServices;
using Neutronium.MVVMComponents;

namespace Neutronium.BuildingBlocks.Application.ViewModels
{
    /// <summary>
    /// ViewModel representing the application
    /// </summary>
    /// <typeparam name="T">
    /// ViewModel type that is designed to be accessible
    /// throughout the application life cycle independent from
    /// which page is currently displayed 
    /// </typeparam>
    public class ApplicationViewModel<T> : ViewModel, IMessageBox, INotificationSender
    {
        private readonly Lazy<T> _ApplicationInformationBuilder;

        /// <summary>
        /// Application information
        /// </summary>
        public T ApplicationInformation => _ApplicationInformationBuilder.Value;

        /// <summary>
        /// Window interface
        /// </summary>
        public IWindowViewModel Window { get; }

        /// <summary>
        /// Router
        /// </summary>
        public NavigationViewModel Router { get; }

        private object _CurrentViewModel;
        /// <summary>
        /// Page ViewModel
        /// </summary>
        public object CurrentViewModel
        {
            get => _CurrentViewModel;
            set => Set(ref _CurrentViewModel, value);
        }

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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="window"></param>
        /// <param name="router"></param>
        /// <param name="applicationInformation"></param>
        public ApplicationViewModel(IWindowViewModel window, NavigationViewModel router, T applicationInformation)
        {
            Window = window;
            Router = router;
            _ApplicationInformationBuilder = new Lazy<T>(() => applicationInformation);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="window"></param>
        /// <param name="router"></param>
        /// <param name="applicationInformationBuilder"></param>
        public ApplicationViewModel(IWindowViewModel window, NavigationViewModel router, Func<T> applicationInformationBuilder)
        {
            Window = window;
            Router = router;
            _ApplicationInformationBuilder = new Lazy<T>(applicationInformationBuilder);
        }

        /// <summary>
        /// Show message with confirmation in modal window
        /// </summary>
        /// <param name="confirmationMessage">
        /// true if the user clicks on OK
        /// </param>
        /// <returns></returns>
        public Task<bool> ShowMessage(ConfirmationMessage confirmationMessage)
        {
            var modal = new MainModalViewModel(confirmationMessage);
            Modal = modal;
            return modal.CompletionTask;
        }

        /// <summary>
        /// Show information in modal window
        /// </summary>
        /// <param name="messageInformation"></param>
        public void ShowInformation(MessageInformation messageInformation)
        {
            var modal = new MessageModalViewModel(messageInformation);
            Modal = modal;
        }

        /// <summary>
        /// Display a notification
        /// </summary>
        /// <param name="notification"></param>
        public void Send(Notification notification)
        {
            Notification = notification;
        }
    }
}

