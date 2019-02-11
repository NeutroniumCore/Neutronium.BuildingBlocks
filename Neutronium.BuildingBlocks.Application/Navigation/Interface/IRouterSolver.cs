namespace Neutronium.BuildingBlocks.Application.Navigation
{
    /// <summary>
    /// Routing configuration associating viewModels and route
    /// </summary>
    public interface IRouterSolver
    {
        /// <summary>
        /// Find route associated with the viewModel
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns>
        /// The corresponding route
        /// </returns>
        string SolveRoute(object viewModel);

        /// <summary>
        /// Find route associated with the provided type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        /// The corresponding route
        /// </returns>
        string SolveRoute<T>();

        /// <summary>
        /// Find the viewModel type associated with the corresponding route
        /// in the given context
        /// </summary>
        /// <param name="context">
        /// null if root context
        /// </param>
        /// <param name="route"></param>
        /// <returns>
        /// The corresponding route destination
        /// </returns>
        RouteDestination SolveType(string route, string context = null);


        /// <summary>
        /// Find the viewModel type associated with the corresponding route
        /// </summary>
        /// <param name="route">
        /// Route specification
        /// </param>
        /// <returns>
        /// The corresponding route destination
        /// </returns>
        RouteDestination SolveType(RouteSpecification route);
    }
}