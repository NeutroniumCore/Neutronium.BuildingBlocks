using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using Vm.Tools.Standard;

namespace Vm.Tools.Dialog.FolderChooser 
{
    public class FolderPicker : IFolderPicker
    {
        public string Title { get; set; }
        public string Directory { get; set; }
        public bool Multiselect { get; set; }

        public string Choose()
        {
            var owner = System.Windows.Application.Current.MainWindow;
            return ShowDialog(owner);
        }

        private string ShowDialog(Window owner)
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
           
            var res = directoryDialog.ShowDialog(owner);
            return (res == CommonFileDialogResult.Ok) ? directoryDialog.FileName : null;
        }
    }
}
