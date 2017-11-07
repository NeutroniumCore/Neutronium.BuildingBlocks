namespace Vm.Tools.Dialog.FileChooser 
{
    public class FolderChooserCommand : ChooserCommand<IFolderPicker> 
    {
        public FolderChooserCommand() : this(new FolderPicker())
        {
        }

        public FolderChooserCommand(IFolderPicker folderPicker) : base(folderPicker) 
        {
        }
    }
}
