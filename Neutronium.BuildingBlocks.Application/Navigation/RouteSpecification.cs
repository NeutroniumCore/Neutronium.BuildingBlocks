namespace Neutronium.BuildingBlocks.Application.Navigation
{
    /// <summary>
    /// Route destination
    /// </summary>
    public struct RouteSpecification
    {
        /// <summary>
        /// Name of the route
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Route context, null means route context
        /// </summary>
        public string Context { get; }

        public RouteSpecification(string name, string context = null)
        {
            Name = name;
            Context = context;
        }
    }
}
