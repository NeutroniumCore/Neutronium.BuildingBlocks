namespace Neutronium.BuildingBlocks.Application.Navigation
{
    public struct RouteInfo
    {
        public object ViewModel { get; }
        public string RouteName { get; }

        public RouteInfo(RouteContext routeContext) : this(routeContext.ViewModel, routeContext.Route)
        {
        }

        public RouteInfo(object viewModel, string routeName)
        {
            ViewModel = viewModel;
            RouteName = routeName;
        }

        public override string ToString()
        {
            return $"RouteName: {RouteName} ViewModel: {ViewModel}";
        }
    }
}
