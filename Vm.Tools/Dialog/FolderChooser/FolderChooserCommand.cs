using Vm.Tools.Standard;

namespace Vm.Tools.Dialog.FolderChooser 
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
