using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Application.SetUp.NpmHelper
{
    public interface INpmRunner : IDisposable
    {
        Task<int> GetPortAsync();

        event DataReceivedEventHandler OutputDataReceived;
    }
}
