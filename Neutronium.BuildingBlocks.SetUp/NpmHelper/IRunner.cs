using System;

namespace Neutronium.BuildingBlocks.SetUp.NpmHelper
{
    public interface IRunner : IDisposable
    {
        /// <summary>
        /// Sent when npm console displays information
        /// </summary>
        event EventHandler<MessageEventArgs> OnMessageReceived;
    }
}
