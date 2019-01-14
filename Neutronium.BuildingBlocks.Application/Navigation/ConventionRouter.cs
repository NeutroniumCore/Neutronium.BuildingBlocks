using System;
using System.Collections.Generic;
using MoreCollection.Extensions;
using Neutronium.Core.Navigation.Routing;

namespace Neutronium.BuildingBlocks.Application.Navigation
{
    /// <summary>
    /// Object that build navigation based on convention
    /// </summary>
    public class ConventionRouter : IConventionRouter
    {
        private readonly IRouterBuilder _RouterBuilder;
        private readonly bool _LowerPath;
        private readonly string _Format;
        private const string ViewModelPostFix = "ViewModel";
        private readonly string _PostFix;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routerBuilder"></param>
        /// <param name="format">Route name using template string where {vm} is the class name without postfix,
        /// {namespace} is the namespace, and {id} is the id provided in the register method
        /// </param>
        /// <param name="lowerPath">true to use class name in lower case</param>
        /// <param name="postFix">Class name post fix to be discarded- default to "ViewModel"</param>
        public ConventionRouter(IRouterBuilder routerBuilder, string format, bool lowerPath = true, string postFix = null)
        {
            _RouterBuilder = routerBuilder;
            _LowerPath = lowerPath;
            _PostFix = postFix ?? ViewModelPostFix;
            _Format = format.Replace("{vm}", "{0}").Replace("{namespace}", "{1}").Replace("{id}", "{2}");
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
            var typeName = type.Name;
            if (typeName.EndsWith(_PostFix))
                typeName = typeName.Substring(0, typeName.Length - _PostFix.Length);

            var route = string.Format(_Format, typeName, type.Namespace, id);
            if (_LowerPath)
                route = route.ToLower();
            _RouterBuilder.Register(type, route);
            return this;
        }
    }
}
