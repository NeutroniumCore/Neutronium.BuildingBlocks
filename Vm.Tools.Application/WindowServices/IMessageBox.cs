using System.Threading.Tasks;

namespace Vm.Tools.Application.WindowServices
{
    public interface IMessageBox 
    {
        Task<bool> ShowMessage(ConfirmationMessage confirmationMessage);

        void ShowInformation(MessageInformation messageInformation);
    }
}