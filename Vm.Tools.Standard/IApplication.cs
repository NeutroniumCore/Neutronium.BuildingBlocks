using System;
using System.ComponentModel;

namespace Vm.Tools.Standard
{
    public interface IApplication
    {
        void ForceClose();

        void TryClose();

        void Restart(string commandLineOptions = "");

        event EventHandler<CancelEventArgs> MainWindowClosing;

        event EventHandler Closed;

        event EventHandler<CancelEventArgs> SessionEnding;
    }
}
