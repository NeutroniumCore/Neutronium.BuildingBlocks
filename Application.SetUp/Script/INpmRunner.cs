using System;
using System.Threading.Tasks;

namespace Application.SetUp.Script
{
    public interface INpmRunner : IDisposable
    {
        Task<int> GetPortAsync();
    }
}
