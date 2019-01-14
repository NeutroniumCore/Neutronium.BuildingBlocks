using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neutronium.BuildingBlocks.SetUp.NpmHelper
{
    /// <summary>
    /// Npm script runner
    /// </summary>
    public interface INpmRunner : IDisposable
    {
        /// <summary>
        /// Run live script and return the corresponding port
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// The port where the page is served
        /// </returns>
        Task<int> GetPortAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Sent when npm console displays information
        /// </summary>
        event EventHandler<MessageEventArgs> OnMessageReceived;
    }
}
