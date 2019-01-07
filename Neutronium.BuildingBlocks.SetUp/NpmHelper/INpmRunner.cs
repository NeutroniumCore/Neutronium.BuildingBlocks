using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neutronium.BuildingBlocks.SetUp.NpmHelper
{
    public interface INpmRunner : IDisposable
    {
        Task<int> GetPortAsync(CancellationToken cancellationToken);

        event EventHandler<RunnerMessageEventArgs> OnMessageReceived;

        event EventHandler<RunnerMessageEventArgs> OnErrorReceived;
    }
}
