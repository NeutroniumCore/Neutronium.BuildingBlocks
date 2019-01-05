using Neutronium.BuildingBlocks.ApplicationTools;

namespace Neutronium.BuildingBlocks.Wpf.Dialog.FileChooser 
{
    public class FileChooserCommand : ChooserCommand<IFilePicker> 
    {
        public FileChooserCommand() : this(new FilePicker())
        {
        }

        public FileChooserCommand(IFilePicker filePicker) : base(filePicker) 
        {
        }
    }
}
