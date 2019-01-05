namespace Neutronium.BuildingBlocks.ApplicationTools 
{
    public interface IFolderPicker : IIODialog
    {
        bool Multiselect { get; set; }
    }
}
