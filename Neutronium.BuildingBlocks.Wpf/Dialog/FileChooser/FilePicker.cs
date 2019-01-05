using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using Neutronium.BuildingBlocks.ApplicationTools;

namespace Neutronium.BuildingBlocks.Wpf.Dialog.FileChooser
{
    public class FilePicker : IFilePicker
    {
        public string Title { get; set; }
        public string Directory { get; set; }
        public string ExtensionDescription { get; set; }
        public string[] Extensions { get; set; }

        public string Choose()
        {
            var owner = System.Windows.Application.Current.MainWindow;
            return ShowDialog(owner);
        }

        private string ShowDialog(Window owner)
        {
            var fileDialog = new OpenFileDialog
            {
                InitialDirectory = Directory,
                Title = Title
            };

            if (Extensions?.Length > 0)
            {
                fileDialog.DefaultExt = Extensions[0];
                var files = String.Join("; ", Extensions.Select(ext => $"*{ext}"));
                fileDialog.Filter = $"{ExtensionDescription} ({files})|{files}";
            }
            var result = fileDialog.ShowDialog(owner);
            return (result == true) ? fileDialog.FileName : null;
        }
    }
}
