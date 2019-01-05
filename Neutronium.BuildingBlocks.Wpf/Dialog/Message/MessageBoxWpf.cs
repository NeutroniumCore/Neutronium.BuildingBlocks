﻿using System.Windows;
using Neutronium.BuildingBlocks.ApplicationTools;

namespace Neutronium.BuildingBlocks.Wpf.Dialog.Message
{
    public class MessageBoxWpf: INativeMessageBox
    {
        private readonly Window _Window;

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