using System.ComponentModel;
using Neutronium.BuildingBlocks.Application.Navigation;

namespace Neutronium.BuildingBlocks.Application.LifeCycleHook
{
    public interface IApplicationLifeCycle
    {
        void OnNavigating(RoutingEventArgs routingEvent);

        void OnNavigated(RoutedEventArgs routedEvent);

        void OnClosing(CancelEventArgs cancelEvent);

        void OnSessionEnding(CancelEventArgs cancelEvent);

        void OnClosed();
    }
}
