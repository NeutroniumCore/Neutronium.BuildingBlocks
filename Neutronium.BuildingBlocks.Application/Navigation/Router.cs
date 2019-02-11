using MoreCollection.Extensions;
using System;
using System.Collections.Generic;

namespace Neutronium.BuildingBlocks.Application.Navigation
{
    /// <summary>
    /// <see cref="IRouterBuilder"/> and <see cref="IRouterSolver"/> implementation
    /// </summary>
    public class Router : IRouterBuilder, IRouterSolver
    {
        private readonly Dictionary<string, RouteDestination> _RouteToType = new Dictionary<string, RouteDestination>();
        private readonly Dictionary<string, Dictionary<string, RouteDestination>> _ContextualRouteToType = new Dictionary<string, Dictionary<string, RouteDestination>>();
        private readonly Dictionary<Type, string> _TypeToRoute = new Dictionary<Type, string>();

        public IRouterBuilder Register<T>(string routerName, bool defaultType = true)
        {
            return Register(typeof(T), routerName, defaultType);
        }

        public IRouterBuilder Register(Type type, string routeName, bool defaultType = true)
        {
            var routeSpecification = new RouteSpecification(routeName);
            var routeDestination = new RouteDestination(type, null);
            return Register(routeSpecification, routeDestination, defaultType);
        }

        public IRouterBuilder Register(RouteSpecification route, RouteDestination destination, bool defaultType = true)
        {
            if (route.Context == null)
            {
                if (!_TypeToRoute.ContainsKey(destination.Type))
                    _TypeToRoute.Add(destination.Type, route.Name);
            }

            var routeToType = GetRoutes(route);

            if (!defaultType && routeToType.ContainsKey(route.Name))
                return this;

            routeToType[route.Name] = destination;
            return this;
        }

        private Dictionary<string, RouteDestination> GetRoutes(RouteSpecification route)
        {
            return route.Context == null
                ? _RouteToType : _ContextualRouteToType.GetOrAddEntity(route.Context, _ => new Dictionary<string, RouteDestination>());
        }

        public string SolveRoute(object viewModel)
        {
            return SolveRoute(viewModel.GetType());
        }

        public string SolveRoute<T>()
        {
            return SolveRoute(typeof(T));
        }

        private string SolveRoute(Type type)
        {
            var route = default(string);
            do
            {
                route = _TypeToRoute.GetOrDefault(type);
            } while ((route == null) && ((type = type.BaseType) != null));
            return route;
        }

        public RouteDestination SolveType(string route, string context = null)
        {
            return SolveType(new RouteSpecification(route, context));
        }

        public RouteDestination SolveType(RouteSpecification route)
        {
            var routeToType = GetRoutes(route);
            return routeToType.GetOrDefault(route.Name);
        }
    }
}
