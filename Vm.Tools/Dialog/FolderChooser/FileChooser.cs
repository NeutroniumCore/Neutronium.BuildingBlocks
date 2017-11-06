namespace Vm.Tools.Dialog.FileChooser 
{
    public class FolderChooserCommand : ChooserCommand<IFolderPicker> 
    {
        public FolderChooserCommand() : base(new FolderPicker())
        {
        }
    }
}
