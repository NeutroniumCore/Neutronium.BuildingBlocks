namespace Neutronium.BuildingBlocks.Application.Navigation
{
    /// <summary>
    /// Provides extensions for <see cref="IRouterBuilder"/>
    /// </summary>
    public static class RouterBuilderExtensions
    {
        /// <summary>
        /// Create a convention router for the given router builder
        /// </summary>
        /// <param name="routerBuilder"></param>
        /// <param name="template"></param>
        /// <param name="lowerPath"></param>
        /// <returns></returns>
        public static IExtendedConventionRouter GetTemplateConvention(this IRouterBuilder routerBuilder, string template, bool lowerPath = true)
        {
            return new ConventionRouter(routerBuilder, template, lowerPath);
        }

        /// <summary>
        /// Create a convention router for the given router builder
        /// </summary>
        /// <param name="routerBuilder"></param>
        /// <param name="template"></param>
        /// <param name="postFix"></param>
        /// <returns></returns>
        public static IExtendedConventionRouter GetTemplateConvention(this IRouterBuilder routerBuilder, string template, string postFix)
        {
            return new ConventionRouter(routerBuilder, template, postFix: postFix);
        }
    }
}
