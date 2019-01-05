namespace Neutronium.BuildingBlocks.Application.WindowServices
{
    public interface INotificationSender
    {
        void Send(Notification notification);
    }
}
