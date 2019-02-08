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

        /// <summary>
        /// Resolution context as a path.
        /// Null for route context
        /// </summary>
        public string Context { get; }

        public RouteDestination(Type type, string context= null, string resolutionKey = null)
        {
            Type = type;
            Context = context;
            ResolutionKey = resolutionKey;
        }
    }
}
