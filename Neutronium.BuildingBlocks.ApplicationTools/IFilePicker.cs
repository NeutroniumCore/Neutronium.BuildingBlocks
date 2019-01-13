namespace Neutronium.BuildingBlocks.ApplicationTools 
{
    /// <summary>
    /// File picker abstraction
    /// </summary>
    public interface IFilePicker : IIODialog
    {
        /// <summary>
        /// Accepted extensions
        /// </summary>
        string[] Extensions { get; set; }

        /// <summary>
        /// Extension description
        /// </summary>
        string ExtensionDescription { get; set; }
    }
}
