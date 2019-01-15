using Neutronium.BuildingBlocks.ApplicationTools;

namespace Neutronium.BuildingBlocks.Wpf.Dialog.FileChooser 
{
    /// <summary>
    /// ChooserCommand for FilePicker
    /// </summary>
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
