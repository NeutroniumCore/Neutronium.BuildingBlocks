using System;

namespace Neutronium.BuildingBlocks.SetUp
{
    /// <summary>
    /// Message events
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// Message value
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// True if associated with an error
        /// </summary>
        public bool Error { get; }

        public MessageEventArgs(string message, bool error = false)
        {
            Message = message;
            Error = error;
        }
    }
}
