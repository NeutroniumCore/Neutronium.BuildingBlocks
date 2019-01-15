using System.Windows;
using Neutronium.BuildingBlocks.ApplicationTools;

namespace Neutronium.BuildingBlocks.Wpf.Dialog.Message
{
    /// <summary>
    /// Wpf implementation of <see cref="INativeMessageBox"/>
    /// </summary>
    public class MessageBoxWpf: INativeMessageBox
    {
        private readonly Window _Window;

        /// <summary>
        /// Construct MessageBoxWpf
        /// </summary>
        /// <param name="window">
        /// Main application window
        /// </param>
        public MessageBoxWpf(Window window)
        {
            _Window = window;
        }

        public bool ShowConfirmationMessage(string message, string title, WindowType type)
        {
            var messageBoxType = (type == WindowType.OkCancel) ? MessageBoxButton.OKCancel : MessageBoxButton.YesNo;
            return MessageBox.Show(_Window, message, title, messageBoxType) == MessageBoxResult.OK;
        }

        public void ShowMessage(string message, string title)
        {
            MessageBox.Show(_Window, message, title);
        }
    }
}
