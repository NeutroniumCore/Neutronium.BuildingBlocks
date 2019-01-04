using System;
using System.Threading.Tasks;

namespace Application.SetUp.NpmHelper
{
    public interface INpmRunner : IDisposable
    {
        Task<int> GetPortAsync();

        event EventHandler<RunnerMessageEventArgs> OnMessageReceived;

        event EventHandler<RunnerMessageEventArgs> OnErrorReceived;
    }
}
