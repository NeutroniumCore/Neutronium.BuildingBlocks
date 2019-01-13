namespace Neutronium.BuildingBlocks.ApplicationTools
{
    /// <summary>
    /// Message box window type
    /// </summary>
    public enum WindowType
    {
        OkCancel,
        YesNo
    }

    /// <summary>
    /// Native message box abstraction
    /// </summary>
    public interface INativeMessageBox
    {
        /// <summary>
        /// Show a message to end user and wait for an answer
        /// </summary>
        /// <param name="message">Message to be displayed.</param>
        /// <param name="title">The title of the window.</param>
        /// <param name="type"></param>
        /// <returns> 
        /// true if end users agrees to the question
        /// </returns>
        bool ShowConfirmationMessage(string message, string title, WindowType type);

        /// <summary>
        /// Show a message to end user
        /// </summary>   
        /// <param name="message">Message to be displayed.</param>
        /// <param name="title">The title of the window.</param>
        void ShowMessage(string message, string title);
    }
}
