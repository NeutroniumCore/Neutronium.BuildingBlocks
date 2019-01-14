using System;

namespace Neutronium.BuildingBlocks.Application.Navigation
{
    /// <summary>
    /// Routing information, useful for logging purpose
    /// </summary>
    public class RoutingMessageArgs : EventArgs
    {
        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Message type
        /// </summary>
        public MessageType Type { get; }

        public RoutingMessageArgs(string message, MessageType type)
        {
            Message = message;
            Type = type;
        }
    }

    /// <summary>
    /// Routing message type
    /// </summary>
    public enum MessageType
    {
        Error,
        Information
    }
}
