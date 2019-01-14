using System;

namespace Neutronium.BuildingBlocks.Application.Navigation
{
    /// <summary>
    /// Navigation context
    /// </summary>
    public class NavigationContext
    {
        /// <summary>
        /// Resolution key used by dependency injection
        /// </summary>
        public string ResolutionKey { get; set; }

        /// <summary>
        /// Route name
        /// </summary>
        public string RouteName { get; set; }
    }

    /// <summary>
    /// Navigation context with before event
    /// </summary>
    /// <typeparam name="T">
    /// viewModel type
    /// </typeparam>
    public class NavigationContext<T> : NavigationContext
    {
        /// <summary>
        /// Action that wil be called before navigation
        /// </summary>
        public Action<T> BeforeNavigate { get; set; }
    }
}