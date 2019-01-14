namespace Neutronium.BuildingBlocks.Application.WindowServices
{
    /// <summary>
    /// Notifier abstraction
    /// </summary>
    public interface INotificationSender
    {
        /// <summary>
        /// Show the corresponding notification
        /// </summary>
        /// <param name="notification"></param>
        void Send(Notification notification);
    }
}
