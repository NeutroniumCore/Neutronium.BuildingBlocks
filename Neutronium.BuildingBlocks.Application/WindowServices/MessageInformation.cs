namespace Neutronium.BuildingBlocks.Application.WindowServices
{
    public class MessageInformation
    {
        public string Message { get; }
        public string OkMessage { get; }
        public string Title { get; }

        public MessageInformation(string title, string message, string okMessage)
        {
            Title = title;
            Message = message;
            OkMessage = okMessage;
        }
    }
}