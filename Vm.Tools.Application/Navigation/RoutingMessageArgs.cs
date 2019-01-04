using System;

namespace Vm.Tools.Application.Navigation
{
    public class RoutingMessageArgs : EventArgs
    {
        public string Message { get; }
        public MessageType Type { get; }

        public RoutingMessageArgs(string message, MessageType type)
        {
            Message = message;
            Type = type;
        }
    }

    public enum MessageType
    {
        Error,
        Information
    }
}
