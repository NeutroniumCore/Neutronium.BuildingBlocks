using Neutronium.BuildingBlocks.Application.WindowServices;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;

namespace Neutronium.BuildingBlocks.Application.ViewModels.Modal 
{
    /// <summary>
    /// Modal ViewModel. Internally used by <see cref="IMessageBox"/> implementation
    /// </summary>
    public class MessageModalViewModel
    {
        public string Title { get; }
        public string Message { get; }
        public string OkMessage { get; }

        public ISimpleCommand OkCommand { get; }

        internal MessageModalViewModel(MessageInformation messageInformation)
        {
            Title = messageInformation.Title;
            Message = messageInformation.Message;
            OkMessage = messageInformation.OkMessage;

            OkCommand = new RelaySimpleCommand(Ok);
        }

        protected virtual void Ok()
        {           
        }
    }
}