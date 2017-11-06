namespace Vm.Tools.Dialog 
{
    public interface IFolderPicker : IIODialog
    {
        bool Multiselect { get; set; }
    }
}
