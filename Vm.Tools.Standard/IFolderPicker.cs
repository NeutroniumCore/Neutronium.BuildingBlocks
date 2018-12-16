namespace Vm.Tools.Standard 
{
    public interface IFolderPicker : IIODialog
    {
        bool Multiselect { get; set; }
    }
}
