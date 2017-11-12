using System;
using System.ComponentModel;

namespace Vm.Tools.Application
{
    public interface IApplication
    {
        void ForceClose();

        void TryClose();

        event EventHandler<CancelEventArgs> MainWindowClosing;

        event EventHandler<CancelEventArgs> SessionEnding;
    }
}
