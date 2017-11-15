using System;
using System.ComponentModel;

namespace Vm.Tools.Application
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
