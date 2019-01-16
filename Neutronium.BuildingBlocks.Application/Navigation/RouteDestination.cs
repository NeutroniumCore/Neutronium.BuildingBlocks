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
        internal Type Type { get; }

        /// <summary>
        /// Resolution key to be used in common service locator
        /// </summary>
        internal string ResolutionKey { get; }

        internal RouteDestination(Type type, string resolutionKey = null)
        {
            Type = type;
            ResolutionKey = resolutionKey;
        }
    }
}
