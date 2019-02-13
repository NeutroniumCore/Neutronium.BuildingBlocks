using Neutronium.BuildingBlocks.Application.Navigation;
using Neutronium.Core.Navigation.Routing;

namespace Neutronium.Vue.SPA
{
    public class RoutingConfiguration
    {
        public static IRouterSolver Register()
        {
            var router = new Router();
            BuildRoutes(router);
            return router;
        }

        private static void BuildRoutes(IRouterBuilder routeBuilder)
        {
            var convention = routeBuilder.GetTemplateConvention("{vm}");
            typeof(RoutingConfiguration).GetTypesFromSameAssembly()
                .InNamespace("Neutronium.Vue.SPA1.ViewModel")
                .Register(convention);
        }
    }
}
