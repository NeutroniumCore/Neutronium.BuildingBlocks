using System.Threading.Tasks;

namespace Neutronium.BuildingBlocks.Application.Navigation.Internals
{
    internal class RouteContext
    {
        internal object ViewModel { get; private set; }
        internal string Route { get; private set; }
        internal Task Task => _TaskCompletionSource.Task;

        private readonly TaskCompletionSource<int> _TaskCompletionSource = new TaskCompletionSource<int>();

        internal RouteContext(object viewModel, string route)
        {
            ViewModel = viewModel;
            Route = route;
        }

        internal void Complete()
        {
            _TaskCompletionSource.TrySetResult(0);
        }

        internal void Redirect(string redirect, object viewModel)
        {
            Route = redirect;
            ViewModel = viewModel;
        }
    }
}
