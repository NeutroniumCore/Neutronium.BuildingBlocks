using System.ComponentModel;
using Neutronium.BuildingBlocks.Application.Navigation;

namespace Neutronium.BuildingBlocks.Application.LifeCycleHook
{
    /// <summary>
    /// Abstraction exposing application hook to listen and control
    /// navigation and closing events.
    /// </summary>
    public interface IApplicationLifeCycle
    {
        /// <summary>
        /// Sent before navigation, allows cancellation and redirect
        /// </summary>
        /// <param name="routingEvent"></param>
        void OnNavigating(RoutingEventArgs routingEvent);

        /// <summary>
        /// Sent after navigation
        /// </summary>
        /// <param name="routedEvent"></param>
        void OnNavigated(RoutedEventArgs routedEvent);

        /// <summary>
        /// Sent on application closing, allows cancellation
        /// </summary>
        /// <param name="cancelEvent"></param>
        void OnClosing(CancelEventArgs cancelEvent);

        /// <summary>
        /// Sent on session closing, allows cancellation
        /// </summary>
        /// <param name="cancelEvent"></param>
        void OnSessionEnding(CancelEventArgs cancelEvent);

        /// <summary>
        /// Sent on application closed
        /// </summary>
        void OnClosed();
    }
}
