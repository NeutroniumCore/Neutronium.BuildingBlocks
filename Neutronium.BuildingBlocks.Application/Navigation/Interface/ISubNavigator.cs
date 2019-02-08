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
        string ChildName { get; }

        /// <summary>
        /// Child
        /// </summary>
        ISubNavigator Child { get; }
    }
}
