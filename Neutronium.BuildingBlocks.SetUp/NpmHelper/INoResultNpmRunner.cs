using System.Threading;
using System.Threading.Tasks;

namespace Neutronium.BuildingBlocks.SetUp.NpmHelper
{
    /// <summary>
    /// Npm script runner
    /// </summary>
    public interface INoResultNpmRunner : IRunner
    {
        /// <summary>
        /// Run script until completion
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// </returns>
        Task Run(CancellationToken cancellationToken);
    }
}
