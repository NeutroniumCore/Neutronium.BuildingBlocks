namespace Neutronium.BuildingBlocks.Application.WindowServices
{
    /// <summary>
    /// Message asking for confirmation 
    /// </summary>
    public class ConfirmationMessage : MessageInformation
    {
        /// <summary>
        /// Cancel message
        /// </summary>
        public string CancelMessage { get; }

        public ConfirmationMessage(string title, string message, string okMessage, string cancelMessage) : base(title, message, okMessage)
        {
            CancelMessage = cancelMessage;
        }
    }
}