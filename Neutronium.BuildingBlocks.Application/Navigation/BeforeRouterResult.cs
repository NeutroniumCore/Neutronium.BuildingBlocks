namespace Neutronium.BuildingBlocks.Application.Navigation
{
    /// <summary>
    /// Before navigation information
    /// </summary>
    public struct BeforeRouterResult
    {
        /// <summary>
        /// Redirect route if not null
        /// </summary>
        public string Redirect { get; }

        /// <summary>
        /// Cancel navigation if false
        /// </summary>
        public bool Continue { get; }

        /// <summary>
        /// Targeted ViewModel
        /// </summary>
        public object To { get; }

        private BeforeRouterResult(string redirect, object viewModel)
        {
            Redirect = redirect;
            Continue = true;
            To = viewModel;
        }

        private BeforeRouterResult(bool continueRoute, object viewModel)
        {
            Redirect = null;
            Continue = continueRoute;
            To = viewModel;
        }

        /// <summary>
        /// Instance used to cancel navigation
        /// </summary>
        /// <returns></returns>
        public static BeforeRouterResult Cancel()
        {
            return new BeforeRouterResult(false, null);
        }

        /// <summary>
        /// Instance used to  navigate to the given ViewModel
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static BeforeRouterResult Ok(object viewModel)
        {
            return new BeforeRouterResult(null, viewModel);
        }

        /// <summary>
        /// Instance used to redirect the navigation
        /// </summary>
        /// <param name="routeName"></param>
        /// <returns></returns>
        public static BeforeRouterResult CreateRedirect(string routeName)
        {
            return new BeforeRouterResult(routeName, null);
        }
    }
}
