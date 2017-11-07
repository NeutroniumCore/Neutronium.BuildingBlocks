namespace Vm.Tools.Dialog.FileChooser 
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
