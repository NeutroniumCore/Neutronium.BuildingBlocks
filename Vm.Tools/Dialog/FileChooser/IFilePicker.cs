namespace Vm.Tools.Dialog.FileChooser 
{
    public interface IFilePicker : IIODialog
    {
        string[] Extensions { get; set; }
        string ExtensionDescription { get; set; }
    }
}
