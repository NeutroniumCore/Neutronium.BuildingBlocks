namespace Vm.Tools.Dialog.FileChooser 
{
    public class FileChooserCommand : ChooserCommand<IFilePicker> 
    {
        public FileChooserCommand() : base(new FilePicker())
        {
        }
    }
}
