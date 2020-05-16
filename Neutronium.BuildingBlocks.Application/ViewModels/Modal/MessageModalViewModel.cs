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
        /// <summary>
        /// title
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Modal message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Modal Ok message
        /// </summary>
        public string OkMessage { get; }

        public ISimpleCommand OkCommand { get; }

        /// <summary>
        /// MessageModalViewModel constructor
        /// </summary>
        /// <param name="messageInformation"></param>
        public MessageModalViewModel(MessageInformation messageInformation)
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