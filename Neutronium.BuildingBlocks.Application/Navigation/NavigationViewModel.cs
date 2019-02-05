using CommonServiceLocator;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neutronium.BuildingBlocks.Application.Navigation
{
    /// <summary>
    /// ViewModel providing an implementing of <see cref="INavigator"/> and binding with
    /// javascript routing API.
    /// Originally designed to work with vue-router.
    /// </summary>
    public class NavigationViewModel : ViewModel, INavigator
    {
        public IResultCommand<string, BeforeRouterResult> BeforeResolveCommand { get; }
        public ISimpleCommand<string> AfterResolveCommand { get; }

        private string _Route;
        public string Route
        {
            get => _Route;
            private set => Set(ref _Route, value);
        }

        public event EventHandler<RoutingEventArgs> OnNavigating;
        public event EventHandler<RoutedEventArgs> OnNavigated;
        public event EventHandler<RoutingMessageArgs> OnRoutingMessage;

        private readonly Lazy<IServiceLocator> _ServiceLocator;
        private readonly IRouterSolver _RouterSolver;
        private readonly Queue<RouteContext> _CurrentNavigations = new Queue<RouteContext>();

        private object _ViewModel;

        public NavigationViewModel(Lazy<IServiceLocator> serviceLocator, IRouterSolver routerSolver, string initialRoute = null)
        {
            _ServiceLocator = serviceLocator;
            _RouterSolver = routerSolver;
            AfterResolveCommand = new RelaySimpleCommand<string>(AfterResolve);
            BeforeResolveCommand = RelayResultCommand.Create<string, BeforeRouterResult>(BeforeResolve);
            Route = initialRoute;
        }

        private BeforeRouterResult BeforeResolve(string routeName)
        {
            OnInformation($"Navigating to: {routeName}");
            var context = GetRouteContext(routeName);
            return (context == null) ? BeforeRouterResult.Cancel() : Navigate(context);
        }

        private BeforeRouterResult Navigate(RouteContext to)
        {
            var routingEventArgs = new RoutingEventArgs(to, Route, _ViewModel);
            OnNavigating?.Invoke(this, routingEventArgs);

            if (routingEventArgs.Cancel)
            {
                _CurrentNavigations.Dequeue();
                to.Complete();
                return BeforeRouterResult.Cancel();
            }

            var redirect = routingEventArgs.RedirectedTo;
            if (string.IsNullOrEmpty(redirect))
            {
                _ViewModel = to.ViewModel;
                return BeforeRouterResult.Ok(_ViewModel);
            }

            var nextViewModel = GetViewModelFromRoute(redirect);
            if (nextViewModel == null)
            {
                OnError($"Redirect route not found: {redirect}, navigation will be cancelled.");
                return BeforeRouterResult.Cancel();
            }

            to.Redirect(redirect, nextViewModel);
            return BeforeRouterResult.CreateRedirect(redirect);
        }

        private RouteContext GetRouteContext(string routeName)
        {
            if (_CurrentNavigations.Count == 0)
            {
                var newContext = CreateRouteContext(routeName);
                if (newContext == null)
                    OnError($"Route not found {routeName}. Cancelling navigation.");
                return newContext;
            }

            var context = _CurrentNavigations.Peek();
            if (context.Route != routeName)
            {
                OnError($"Navigation inconsistency: from browser {routeName}, from context: {context.Route}");
                return null;
            }

            return context;
        }

        private RouteContext CreateRouteContext(string routeName)
        {
            var viewModel = GetViewModelFromRoute(routeName);
            return viewModel == null ? null : CreateRouteContext(viewModel, routeName);
        }

        private object GetViewModelFromRoute(string routeName)
        {
            var routeDestination = _RouterSolver.SolveType(routeName);
            if (routeDestination != null)
            {
                return GetInstance(routeDestination);
            }

            var path = routeName;
            var paths = new Stack<string>();

            var index = path.LastIndexOf('/');
            while ((index != -1) && (routeDestination == null))
            {
                paths.Push(path.Substring(index + 1, path.Length - index - 1));
                path = path.Substring(0, index);
                routeDestination = _RouterSolver.SolveType(path);
                index = path.LastIndexOf('/');
            }

            if (routeDestination == null)
                return null;

            var root = GetInstance(routeDestination);
            return CreateChildren(root, paths, routeName) ? root : null;
        }

        private object GetInstance(RouteDestination routeDestination)
        {
            var key = routeDestination.ResolutionKey;
            var type = routeDestination.Type;
            return key == null ? _ServiceLocator.Value.GetInstance(type) : _ServiceLocator.Value.GetInstance(type, key);
        }

        private bool CreateChildren(object root, IEnumerable<string> paths, string route)
        {
            if (!(root is ISubNavigator subNavigator))
            {
                OnError($"Problem when solving {route}. Sub-path not found: {paths.First()}, root viewModel {root} does not implement ISubNavigator");
                return false;
            }

            foreach (var path in paths)
            {
                subNavigator = subNavigator.NavigateTo(path);
                if (subNavigator == null)
                {
                    OnError($"Problem when solving {route}. Sub-path not found: {path}");
                    return false;
                }
            }
            return true;
        }

        private RouteContext CreateRouteContext(object viewModel, string routeName)
        {
            var routeContext = new RouteContext(viewModel, routeName);
            _CurrentNavigations.Enqueue(routeContext);
            return routeContext;
        }

        private void AfterResolve(string routeName)
        {
            //Possible on hot-reload or after crash recovery
            //Or on redirect on load
            if (_CurrentNavigations.Count == 0)
            {
                Route = routeName;
                return;
            }

            var context = _CurrentNavigations.Dequeue();
            if (context.Route != routeName)
            {
                OnError($"Navigation inconsistency: from browser {routeName}, from context: {context.Route}. Maybe rerouted?");
            }
            context.Complete();

            Route = routeName;
            OnNavigated?.Invoke(this, new RoutedEventArgs(context));
        }

        public Task Navigate(object viewModel, string routeName)
        {
            var route = routeName ?? GetRouteForViewModel(viewModel);

            if (route == null)
            {
                OnError($"Route not found for vm: {viewModel} of type {viewModel?.GetType()}");
                return Task.CompletedTask;
            }

            if (Route == route)
            {
                OnInformation($"Route unchanged: {routeName}");
                if (!ReferenceEquals(_ViewModel, viewModel))
                    OnNavigated?.Invoke(this, new RoutedEventArgs(viewModel, route));

                _ViewModel = viewModel;
                return Task.CompletedTask;
            }

            var routeContext = CreateRouteContext(viewModel, route);
            Route = route;
            return routeContext.Task;
        }

        private string GetRouteForViewModel(object viewModel)
        {
            var root = _RouterSolver.SolveRoute(viewModel);
            if ((root == null) || !(viewModel is ISubNavigator subNavigator))
                return root;

            var builder = new StringBuilder(root);
            while (subNavigator != null)
            {
                var relativeName = subNavigator.RelativeName;
                if (relativeName != null)
                {
                    builder.Append('/');
                    builder.Append(relativeName);
                }
                subNavigator = subNavigator.Child;
            }
            return builder.ToString();
        }

        public Task Navigate<T>(NavigationContext<T> context = null)
        {
            var resolutionKey = context?.ResolutionKey;
            var vm = (resolutionKey == null) ? _ServiceLocator.Value.GetInstance<T>() : _ServiceLocator.Value.GetInstance<T>(resolutionKey);
            context?.BeforeNavigate?.Invoke(vm);
            return Navigate(vm, context?.RouteName);
        }

        public async Task Navigate(Type type, NavigationContext context = null)
        {
            if (type == null)
            {
                OnError("Navigate to null type is not allowed");
                throw new ArgumentNullException(nameof(type));
            }
            var resolutionKey = context?.ResolutionKey;
            var vm = (resolutionKey == null) ? _ServiceLocator.Value.GetInstance(type) : _ServiceLocator.Value.GetInstance(type, resolutionKey);
            await Navigate(vm, context?.RouteName);
        }

        public Task Navigate(string routeName)
        {
            OnInformation($"Navigating to: {routeName}");

            if (Route == routeName)
            {
                OnInformation($"Route unchanged: {routeName}");
                return Task.CompletedTask;
            }

            var ctx = CreateRouteContext(routeName);
            if (ctx == null)
            {
                OnError($"Route not registered: {routeName}.");
                return Task.FromException(new NotImplementedException($"Route not found: {routeName}"));
            }

            Route = routeName;
            return ctx.Task;
        }

        private void OnInformation(string message)
        {
            OnRoutingMessage?.Invoke(this, new RoutingMessageArgs(message, MessageType.Information));
        }

        private void OnError(string message)
        {
            OnRoutingMessage?.Invoke(this, new RoutingMessageArgs(message, MessageType.Error));
        }
    }
}
