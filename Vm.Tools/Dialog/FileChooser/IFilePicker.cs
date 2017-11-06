namespace Vm.Tools.Dialog 
{
    public interface IFilePicker : IIODialog
    {
        string[] Extensions { get; set; }
        string ExtensionDescription { get; set; }
    }
}
