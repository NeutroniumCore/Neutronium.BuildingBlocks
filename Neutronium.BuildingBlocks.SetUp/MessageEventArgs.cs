using System;

namespace Neutronium.BuildingBlocks.SetUp
{
    public class MessageEventArgs : EventArgs
    {
        public string Message { get; }

        public bool Error { get; }

        public MessageEventArgs(string message, bool error = false)
        {
            Message = message;
            Error = error;
        }
    }
}
