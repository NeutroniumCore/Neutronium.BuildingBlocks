namespace Vm.Tools.Application.WindowServices
{
    public class ConfirmationMessage : MessageInformation
    {
        public string CancelMessage { get; }

        public ConfirmationMessage(string title, string message, string okMessage, string cancelMessage) : base(title, message, okMessage)
        {
            CancelMessage = cancelMessage;
        }
    }
}