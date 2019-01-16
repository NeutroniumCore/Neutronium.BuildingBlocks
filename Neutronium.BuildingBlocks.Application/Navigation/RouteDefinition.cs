using System;

namespace Neutronium.BuildingBlocks.Application.Navigation
{
    /// <summary>
    /// Route definition
    /// </summary>
    public struct RouteDefinition
    {
        /// <summary>
        /// Route name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Route destination
        /// </summary>
        public RouteDestination Destination  { get; }

        /// <summary>
        /// Default type
        /// </summary>
        public bool IsDefault { get; }

        public RouteDefinition(string name, RouteDestination destination, bool isDefault = true)
        {
            Name = name;
            Destination = destination;
            IsDefault = isDefault;
        }

        public RouteDefinition(string name, Type type, string resolutionKey = null, bool isDefault = true)
        {
            Name = name;
            Destination = new RouteDestination(type, resolutionKey);
            IsDefault = isDefault;
        }
    }
}
