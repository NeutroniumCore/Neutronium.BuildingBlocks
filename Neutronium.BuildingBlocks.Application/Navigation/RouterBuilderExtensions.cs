using Neutronium.Core.Navigation.Routing;

namespace Neutronium.BuildingBlocks.Application.Navigation
{
    public static class RouterBuilderExtensions
    {
        public static IConventionRouter GetTemplateConvention(this IRouterBuilder routerBuilder, string template, bool lowerPath=true)
        {
            return new ConventionRouter(routerBuilder, template, lowerPath);
        }

        public static IConventionRouter GetTemplateConvention(this IRouterBuilder routerBuilder, string template, string postFix)
        {
            return new ConventionRouter(routerBuilder, template, postFix: postFix);
        }
    }
}
