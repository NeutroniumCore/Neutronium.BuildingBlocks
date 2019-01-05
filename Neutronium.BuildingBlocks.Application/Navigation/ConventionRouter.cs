using System;
using System.Collections.Generic;
using MoreCollection.Extensions;
using Neutronium.Core.Navigation.Routing;

namespace Neutronium.BuildingBlocks.Application.Navigation
{
    public class ConventionRouter : IConventionRouter
    {
        private readonly IRouterBuilder _RouterBuilder;
        private readonly bool _LowerPath;
        private readonly string _Format;
        private const string ViewModelPostFix = "ViewModel";
        private readonly string _PostFix;

        public ConventionRouter(IRouterBuilder routerBuilder, string format, bool lowerPath = true, string postFix = null)
        {
            _RouterBuilder = routerBuilder;
            _LowerPath = lowerPath;
            _PostFix = postFix ?? ViewModelPostFix;
            _Format = format.Replace("{vm}", "{0}").Replace("{namespace}", "{1}").Replace("{id}", "{2}");
        }

        public IConventionRouter Register(IEnumerable<Type> types)
        {
            types.ForEach(t => Register(t));
            return this;
        }

        public IConventionRouter Register<T>(string id = null)
        {
            return Register(typeof(T), id);
        }

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
