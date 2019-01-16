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

        public IRouterBuilder Register(RouteDefinition routeDefinition)
        {
            var destination = routeDefinition.Destination;
            if (!_TypeToRoute.ContainsKey(destination.Type))
                _TypeToRoute.Add(destination.Type, routeDefinition.Name);

            if (!routeDefinition.IsDefault && _RouteToType.ContainsKey(routeDefinition.Name))
                return this;

            _RouteToType[routeDefinition.Name] = destination;
            return this;
        }

        public IRouterBuilder Register(Type type, string routerName, bool defaultType = true)
        {
            var routeDefinition = new RouteDefinition(routerName, type,null, defaultType);
            return Register(routeDefinition);
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

        public RouteDestination SolveType(string route)
        {
            return _RouteToType.GetOrDefault(route);
        }
    }
}
