using System;
using System.Collections.Generic;
using MoreCollection.Extensions;
using Neutronium.Core.Navigation.Routing;

namespace Neutronium.BuildingBlocks.Application.Navigation.Internals
{
    /// <summary>
    /// Object that build navigation based on convention
    /// </summary>
    internal class ConventionRouter : IExtendedConventionRouter
    {
        private readonly IRouterBuilder _RouterBuilder;
        private readonly Func<Type, string, Tuple<RouteSpecification, RouteDestination>>  _RouteInformationGetter;

        /// <summary>
        /// Construct a template based convention router
        /// </summary>
        /// <param name="routerBuilder"></param>
        /// <param name="format">Route name using template string where {vm} is the class name without postfix,
        /// {namespace} is the namespace, and {id} is the id provided in the register method
        /// </param>
        /// <param name="lowerPath">true to use class name in lower case</param>
        /// <param name="postFix">Class name post fix to be discarded- default to "ViewModel"</param>
        internal ConventionRouter(IRouterBuilder routerBuilder, string format, bool lowerPath = true, string postFix = null)
        {
            _RouterBuilder = routerBuilder;
            var templateConvention = new TemplateConvention(format, lowerPath, postFix);
            _RouteInformationGetter = templateConvention.GetRouteInformation;
        }

        /// <summary>
        /// Construct convention router using a factory method
        /// </summary>
        /// <param name="routerBuilder"></param>
        /// <param name="routeInformationGetter"></param>
        internal ConventionRouter(IRouterBuilder routerBuilder, Func<Type, string, Tuple<RouteSpecification, RouteDestination>> routeInformationGetter)
        {
            _RouterBuilder = routerBuilder;
            _RouteInformationGetter = routeInformationGetter;
        }

        /// <summary>
        /// Add the corresponding types to the convention
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public IConventionRouter Register(IEnumerable<Type> types)
        {
            types.ForEach(t => Register(t));
            return this;
        }

        /// <summary>
        /// Add the corresponding type to the convention, using option id
        /// </summary>
        /// <typeparam name="T">type to register</typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public IConventionRouter Register<T>(string id = null)
        {
            return Register(typeof(T), id);
        }

        /// <summary>
        /// Add the corresponding type to the convention, using option id
        /// </summary>
        /// <param name="type">type to register</param>
        /// <param name="id"></param>
        /// <returns></returns>
        public IConventionRouter Register(Type type, string id = null)
        {
            var (route, routeDestination) = _RouteInformationGetter(type, id);
            _RouterBuilder.Register(route, routeDestination);
            return this;
        }
    }
}
