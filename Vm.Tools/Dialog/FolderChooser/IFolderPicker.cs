namespace Vm.Tools.Dialog.FolderChooser 
{
    public interface IFolderPicker : IIODialog
    {
        bool Multiselect { get; set; }
    }
}
