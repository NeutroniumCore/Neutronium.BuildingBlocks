using System;

namespace Neutronium.BuildingBlocks.Application.Navigation.Internals
{
    internal class TemplateConvention
    {
        private readonly bool _LowerPath;
        private readonly string _Format;
        private const string ViewModelPostFix = "ViewModel";
        private readonly string _PostFix;

        internal TemplateConvention(string format, bool lowerPath = true, string postFix = null)
        {
            _LowerPath = lowerPath;
            _PostFix = postFix ?? ViewModelPostFix;
            _Format = format.Replace("{vm}", "{0}").Replace("{namespace}", "{1}").Replace("{id}", "{2}");
        }

        internal Tuple<string, RouteDestination> GetRouteInformation(Type type, string id)
        {
            var typeName = type.Name;
            if (typeName.EndsWith(_PostFix))
                typeName = typeName.Substring(0, typeName.Length - _PostFix.Length);

            var route = string.Format(_Format, typeName, type.Namespace, id);
            if (_LowerPath)
                route = route.ToLower();

            var routeDestination = new RouteDestination(type);
            return Tuple.Create(route, routeDestination);
        }
    }
}
