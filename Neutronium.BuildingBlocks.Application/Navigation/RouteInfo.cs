namespace Neutronium.BuildingBlocks.Application.Navigation
{
    /// <summary>
    /// Route description
    /// </summary>
    public struct RouteInfo
    {
        /// <summary>
        /// Destination viewModel 
        /// </summary>
        public object ViewModel { get; }

        /// <summary>
        /// Destination route
        /// </summary>
        public string RouteName { get; }

        internal RouteInfo(RouteContext routeContext) : this(routeContext.ViewModel, routeContext.Route)
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
