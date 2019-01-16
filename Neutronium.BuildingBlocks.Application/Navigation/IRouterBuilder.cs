using System;

namespace Neutronium.BuildingBlocks.Application.Navigation
{
    /// <summary>
    /// Route builder
    /// </summary>
    public interface IRouterBuilder
    {
        /// <summary>
        /// Associate a viewmodel type to a given route
        /// </summary>
        /// <param name="type">
        /// Type of view model to register
        /// </param>
        /// <param name="routerName">
        /// router name
        /// </param>
        /// <param name="defaultType">
        /// true if the type should be considered as default 
        /// for the corresponding route name
        /// </param>
        /// <returns>
        /// the router builder instance
        /// </returns>
        IRouterBuilder Register(Type type, string routerName, bool defaultType = true);

        /// <summary>
        /// Associate a viewmodel type to a given route
        /// </summary>
        /// <typeparam name="T">
        /// Type of view model to register
        /// </typeparam>
        /// <param name="routerName">
        /// Route name
        /// </param>
        /// <param name="defaultType">
        /// True if the type should be considered as default 
        /// for the corresponding route name
        /// </param>
        /// <returns>
        /// The navigation builder instance
        /// </returns>
        IRouterBuilder Register<T>(string routerName, bool defaultType = true);

        /// <summary>
        /// Associate a viewmodel type to a given route
        /// </summary>
        /// <param name="routeDefinition">
        /// </param>
        /// <returns>
        /// The navigation builder instance
        /// </returns>
        IRouterBuilder Register(RouteDefinition routeDefinition); 
    }
}
