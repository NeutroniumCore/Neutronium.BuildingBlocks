using System;
using System.ComponentModel;

namespace Neutronium.BuildingBlocks.ApplicationTools
{
    /// <summary>
    /// Application abstraction
    /// </summary>
    public interface IApplication
    {
        /// <summary>
        /// Close the application without calling the cancellation hooks.
        /// </summary>
        void ForceClose();

        /// <summary>
        /// Try to close the application
        /// </summary>
        void TryClose();

        /// <summary>
        /// Close the application and restart it.
        /// </summary>
        /// <param name="commandLineOptions">
        /// command line to be applied to the restarting application
        /// </param>
        void Restart(string commandLineOptions = "");

        /// <summary>
        /// Sent when the main window is closing
        /// </summary>
        event EventHandler<CancelEventArgs> MainWindowClosing;

        /// <summary>
        /// Sent when the main window is closing
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Sent when the session is ending
        /// </summary>
        event EventHandler<CancelEventArgs> SessionEnding;
    }
}
