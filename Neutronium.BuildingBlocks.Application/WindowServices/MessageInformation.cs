namespace Neutronium.BuildingBlocks.Application.WindowServices
{
    /// <summary>
    /// Message to be displayed in modal window
    /// </summary>
    public class MessageInformation
    {
        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Ok button message
        /// </summary>
        public string OkMessage { get; }

        /// <summary>
        /// Window title
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="okMessage"></param>
        public MessageInformation(string title, string message, string okMessage)
        {
            Title = title;
            Message = message;
            OkMessage = okMessage;
        }
    }
}