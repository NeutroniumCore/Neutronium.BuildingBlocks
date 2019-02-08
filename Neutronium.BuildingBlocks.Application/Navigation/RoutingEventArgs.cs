using System.ComponentModel;
using Neutronium.BuildingBlocks.Application.Navigation.Internals;

namespace Neutronium.BuildingBlocks.Application.Navigation
{
    /// <summary>
    /// Routing event
    /// </summary>
    public class RoutingEventArgs : CancelEventArgs
    {
        internal RoutingEventArgs(RouteContext toContext, string fromRoute, object fromVm)
        {
            To = new RouteInfo(toContext);
            From = new RouteInfo(fromVm, fromRoute);
        }

        /// <summary>
        /// Destination route
        /// </summary>
        public RouteInfo To { get; }

        /// <summary>
        /// Current route
        /// </summary>
        public RouteInfo From { get; }

        /// <summary>
        /// Redirected route
        /// </summary>
        public string RedirectedTo { get; private set; }

        /// <summary>
        /// Redirect to the given route
        /// </summary>
        /// <param name="newRouteName"></param>
        public void RedirectToRoute(string newRouteName)
        {
            if (To.RouteName == newRouteName)
                return;
            Cancel = From.RouteName == newRouteName;
            RedirectedTo = newRouteName;
        }

        public override string ToString()
        {
            return $"From: {From} To: {To} RedirectedTo: {RedirectedTo}";
        }
    }
}
