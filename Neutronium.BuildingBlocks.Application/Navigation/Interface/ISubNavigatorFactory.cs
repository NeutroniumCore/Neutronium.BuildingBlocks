namespace Neutronium.BuildingBlocks.Application.Navigation
{
    /// <summary>
    /// Manage relative navigation within ViewModel
    /// </summary>
    public interface ISubNavigatorFactory : ISubNavigator
    {
        /// <summary>
        /// Create a ISubNavigator corresponding to a sub path and set ChildName to the corresponding
        /// relativePath.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns>
        /// The created nested viewModel
        /// </returns>
        ISubNavigator Create(string relativePath);
    }
}
