using System;
using System.Threading.Tasks;

namespace Neutronium.BuildingBlocks.Application.Navigation
{
    /// <summary>
    /// Navigation interface
    /// </summary>
    public interface INavigator
    {
        /// <summary>
        /// Navigate to the given viewModel using the optional route
        /// If no route, is provided the RouterSolver will be used to find
        /// the corresponding route
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="routeName"></param>
        /// <returns></returns>
        Task Navigate(object viewModel, string routeName = null);

        /// <summary>
        /// Navigate to the given route
        /// </summary>
        /// <param name="routeName"></param>
        /// <returns></returns>
        Task Navigate(string routeName);

        /// <summary>
        /// Navigate to the given NavigationContext
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        Task Navigate<T>(NavigationContext<T> context = null);

        /// <summary>
        /// Navigate to the type, using CommonServiceLocator to instance the corresponding ViewModel
        /// </summary>
        /// <param name="type"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task Navigate(Type type, NavigationContext context = null);

        /// <summary>
        /// Sent during navigation 
        /// </summary>
        event EventHandler<RoutingEventArgs> OnNavigating;

        /// <summary>
        /// Sent after navigation
        /// </summary>
        event EventHandler<RoutedEventArgs> OnNavigated;

        /// <summary>
        /// Routing events sent for routing purpose 
        /// </summary>
        event EventHandler<RoutingMessageArgs> OnRoutingMessage;
    }
}