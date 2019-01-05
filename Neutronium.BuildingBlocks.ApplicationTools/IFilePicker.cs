namespace Neutronium.BuildingBlocks.ApplicationTools 
{
    public interface IFilePicker : IIODialog
    {
        string[] Extensions { get; set; }
        string ExtensionDescription { get; set; }
    }
}
