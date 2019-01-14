using System;

namespace Neutronium.BuildingBlocks.Application.Navigation
{
    public class RoutedEventArgs : EventArgs
    {
        public RouteInfo NewRoute { get; }

        internal RoutedEventArgs(RouteContext routeContext): this(new RouteInfo(routeContext))
        {
        }

        public RoutedEventArgs(object viewModel, string routeName)
        {
            NewRoute = new RouteInfo(viewModel, routeName);
        }

        public RoutedEventArgs(RouteInfo route)
        {
            NewRoute = route;
        }

        public override string ToString()
        {
            return $"NewRoute: {NewRoute}";
        }
    }
}