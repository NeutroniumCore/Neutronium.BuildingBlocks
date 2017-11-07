using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Vm.Tools.Dialog.FolderChooser 
{
    public class FolderPicker : IFolderPicker
    {
        public string Title { get; set; }
        public string Directory { get; set; }
        public bool Multiselect { get; set; }

        public Task<string> Choose()
        {
            var tcs = new TaskCompletionSource<string>();
            Task.Run(() => ShowDialog(tcs));
            return tcs.Task;
        }

        private void ShowDialog(TaskCompletionSource<string> tcs)
        {
            var directoryDialog = new CommonOpenFileDialog 
            {
                Title = Title,
                IsFolderPicker = true,
                InitialDirectory = Directory,
                Multiselect = Multiselect,
                DefaultDirectory = Directory,
                EnsureFileExists = true,
                EnsurePathExists = true
            };

            CancelEventHandler eventHandler = (o, e) => FileOk(o as OpenFileDialog, e, tcs);
            directoryDialog.FileOk += eventHandler;
            directoryDialog.ShowDialog();
            directoryDialog.FileOk -= eventHandler;
            tcs.TrySetResult(null);
        }

        private void FileOk(OpenFileDialog dia, CancelEventArgs e, TaskCompletionSource<string> tcs)
        {
            tcs.TrySetResult(dia.FileName);
        }
    }
}
