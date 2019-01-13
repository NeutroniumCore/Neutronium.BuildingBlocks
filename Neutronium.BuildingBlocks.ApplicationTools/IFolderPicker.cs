namespace Neutronium.BuildingBlocks.ApplicationTools 
{
    /// <summary>
    /// Folder picker abstraction
    /// </summary>
    public interface IFolderPicker : IIODialog
    {
        /// <summary>
        /// true to allow multi select
        /// </summary>
        bool Multiselect { get; set; }
    }
}
