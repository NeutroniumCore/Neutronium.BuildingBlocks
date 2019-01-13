namespace Neutronium.BuildingBlocks.ApplicationTools 
{
    /// <summary>
    /// Common abstraction for both <see cref="IFilePicker"/> and <see cref="IFolderPicker"/>
    /// </summary>
    public interface IIODialog 
    {
        /// <summary>
        /// Window title
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Directory where the picker is opened
        /// </summary>
        string Directory { get; set; }

        /// <summary>
        /// Open the picker
        /// </summary>
        /// <returns>
        /// Entity chosen, null if canceled
        /// </returns>
        string Choose();
    }
}
