using System.Threading.Tasks;

namespace Neutronium.BuildingBlocks.Application.WindowServices
{
    /// <summary>
    /// Message box/ modal window abstraction
    /// </summary>
    public interface IMessageBox 
    {
        /// <summary>
        /// Display a confirmation message
        /// </summary>
        /// <param name="confirmationMessage"></param>
        /// <returns></returns>
        Task<bool> ShowMessage(ConfirmationMessage confirmationMessage);

        /// <summary>
        /// Show information
        /// </summary>
        /// <param name="messageInformation"></param>
        void ShowInformation(MessageInformation messageInformation);
    }
}