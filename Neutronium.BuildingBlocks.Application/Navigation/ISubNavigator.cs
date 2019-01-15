namespace Neutronium.BuildingBlocks.Application.Navigation
{
    /// <summary>
    /// Manage relative navigation within ViewModel
    /// </summary>
    public interface ISubNavigator
    {
        /// <summary>
        /// Relative Name of Child
        /// </summary>
        string RelativeName { get; }

        /// <summary>
        /// Child
        /// </summary>
        ISubNavigator Child { get; }

        /// <summary>
        /// Navigate to a sub path and set RelativeName to the corresponding
        /// relativePath.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns>
        /// The created nested viewModel
        /// </returns>
        ISubNavigator NavigateTo(string relativePath);
    }
}
