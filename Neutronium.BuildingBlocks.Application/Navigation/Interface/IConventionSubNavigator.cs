namespace Neutronium.BuildingBlocks.Application.Navigation
{
    /// <summary>
    /// Manage relative navigation within ViewModel
    /// </summary>
    public interface IConventionSubNavigator: ISubNavigator
    { 
        /// <summary>
        /// Set Child and ChildName to the corresponding
        /// values.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns>
        /// The created nested viewModel
        /// </returns>
        void SetChild(string relativePath, object child);
    }
}
