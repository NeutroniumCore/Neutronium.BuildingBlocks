namespace Neutronium.BuildingBlocks.Application.Navigation
{

    /// <summary>
    /// Manage relative navigation within ViewModel
    /// </summary>
    public interface ISubNavigator
    {
        /// <summary>
        /// Relative Name
        /// </summary>
        string RelativeName { get; }

        /// <summary>
        /// Child
        /// </summary>
        ISubNavigator Child { get; }

        /// <summary>
        /// Navigate to a sub path
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns>
        /// The created nested viewModel
        /// </returns>
        ISubNavigator NavigateTo(string relativePath);
    }
}
