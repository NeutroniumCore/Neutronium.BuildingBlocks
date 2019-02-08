using System;

namespace Neutronium.BuildingBlocks.Application.Navigation
{
    /// <summary>
    /// Route destination
    /// </summary>
    public class RouteDestination
    {
        /// <summary>
        /// Destination type
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Resolution key to be used in common service locator
        /// </summary>
        public string ResolutionKey { get; }

        public RouteDestination(Type type, string resolutionKey = null)
        {
            Type = type;
            ResolutionKey = resolutionKey;
        }
    }
}
