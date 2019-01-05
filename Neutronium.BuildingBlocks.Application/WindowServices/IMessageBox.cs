using System.Threading.Tasks;

namespace Neutronium.BuildingBlocks.Application.WindowServices
{
    public interface IMessageBox 
    {
        Task<bool> ShowMessage(ConfirmationMessage confirmationMessage);

        void ShowInformation(MessageInformation messageInformation);
    }
}