using System.ComponentModel;

namespace Vm.Tools.Application.Navigation
{
    public class RoutingEventArgs : CancelEventArgs
    {
        public RoutingEventArgs(RouteContext toContext, string fromRoute, object fromVm)
        {
            To = new RouteInfo(toContext);
            From = new RouteInfo(fromVm, fromRoute);
        }

        public RouteInfo To { get; }
        public RouteInfo From { get; }
        public string RedirectedTo { get; private set; }

        public void RedirectToroute(string newRouteName)
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
