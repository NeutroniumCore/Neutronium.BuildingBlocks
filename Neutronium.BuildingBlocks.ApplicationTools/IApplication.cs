using System;
using System.ComponentModel;

namespace Neutronium.BuildingBlocks.ApplicationTools
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
