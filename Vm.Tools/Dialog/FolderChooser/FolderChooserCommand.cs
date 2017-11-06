namespace Vm.Tools.Dialog.FolderChooser 
{
    public class FolderChooserCommand : ChooserCommand<IFolderPicker> 
    {
        public FolderChooserCommand() : base(new FolderPicker())
        {
        }
    }
}
