using Neutronium.BuildingBlocks.ApplicationTools;

namespace Neutronium.BuildingBlocks.Wpf.Dialog.FolderChooser 
{
    /// <summary>
    /// ChooserCommand for FolderPicker
    /// </summary>
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
