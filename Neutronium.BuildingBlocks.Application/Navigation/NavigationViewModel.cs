using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommonServiceLocator;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;

namespace Neutronium.BuildingBlocks.Application.Navigation
{
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
            var paths = routeName.Split('/');
            var type = _RouterSolver.SolveType(paths[0]);
            if (type == null)
                return null;

            var root = _ServiceLocator.Value.GetInstance(type);
            return CreateChildren(root, paths, routeName) ? root : null;
        }

        private bool CreateChildren(object root, string[] paths, string route)
        {
            if (paths.Length < 2)
                return true;

            if (!(root is ISubNavigator subNavigator))
            {
                OnError($"Problem when solving {route}. Sub-path not found: {paths[1]}, root viewModel {root} does not implement ISubNavigator");
                return false;
            }

            for (var i = 1; i < paths.Length; i++)
            {
                subNavigator = subNavigator.NavigateTo(paths[i]);
                if (subNavigator == null)
                {
                    OnError($"Problem when solving {route}. Sub-path not found: {paths[i]}, path index: {i}");
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
            if ((routeName == Route) && (_CurrentNavigations.Count == 0))
                return;

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
            var child = subNavigator.Child;
            while (child != null)
            {
                builder.Append('/');
                builder.Append(child.RelativeName);
                child = child.Child;
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
