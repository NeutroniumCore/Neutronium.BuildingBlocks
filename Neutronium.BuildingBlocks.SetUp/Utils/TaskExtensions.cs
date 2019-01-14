using System.Threading;
using System.Threading.Tasks;

namespace Neutronium.BuildingBlocks.SetUp.Utils
{
    /// <summary>
    /// Task extensions
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Transform a CancellationToken in a Task
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<T> AsTask<T>(this CancellationToken token)
        {
            var tcs = new TaskCompletionSource<T>();
            token.Register(() => tcs.TrySetCanceled(token));
            return tcs.Task;
        }

        /// <summary>
        /// Execute the task with a cancellation Token,
        /// If the token is cancelled the returned task will immediattely
        /// return as cancelled.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken token)
        {
            return await await Task.WhenAny(task, token.AsTask<T>());
        }
    }
}