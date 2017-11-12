using System;
using System.ComponentModel;
using System.Windows;

namespace Vm.Tools.Application
{
    public class WpfApplication : IApplication
    {
        private readonly Window _Window;

        public WpfApplication(Window window)
        {
            _Window = window;
            _Window.Closing += _Window_Closing;
            System.Windows.Application.Current.SessionEnding += Current_SessionEnding;
        }

        private void Current_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            SessionEnding?.Invoke(this, e);
        }

        private void _Window_Closing(object sender, CancelEventArgs e)
        {
            MainWindowClosing?.Invoke(this, e);
        }

        public void ForceClose()
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void TryClose()
        {
            _Window.Close();
        }

        public event EventHandler<CancelEventArgs> MainWindowClosing;
        public event EventHandler<CancelEventArgs> SessionEnding;
    }
}
