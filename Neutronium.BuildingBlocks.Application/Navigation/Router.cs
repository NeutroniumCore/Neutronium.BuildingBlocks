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
        private readonly Dictionary<Type, string> _TypeToRoute = new Dictionary<Type, string>();

        public IRouterBuilder Register( RouteDestination destination, string routeName, bool defaultType = true)
        {
            if (!_TypeToRoute.ContainsKey(destination.Type))
                _TypeToRoute.Add(destination.Type, routeName);

            if (!defaultType && _RouteToType.ContainsKey(routeName))
                return this;

            _RouteToType[routeName] = destination;
            return this;
        }

        public IRouterBuilder Register(Type type, string routeName, bool defaultType = true)
        {
            var routeDestination = new RouteDestination(type, null);
            return Register(routeDestination, routeName,defaultType);
        }

        public IRouterBuilder Register<T>(string routerName, bool defaultType = true)
        {
            return Register(typeof(T), routerName, defaultType);
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
            return _TypeToRoute.GetOrDefault(type);
        }

        public RouteDestination SolveType(string route, string context= null)
        {
            return _RouteToType.GetOrDefault(route);
        }
    }
}
