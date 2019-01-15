using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using CommonServiceLocator;
using FluentAssertions;
using Neutronium.BuildingBlocks.Application.Navigation;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Neutronium.BuildingBlocks.Application.Tests
{
    public class NavigationViewModelTests
    {
        private readonly NavigationViewModel _NavigationViewModel;
        private readonly IServiceLocator _ServiceLocator;
        private readonly IRouterSolver _RouterSolver;
        private readonly FakeClass _ExpectedNewViewModel;
        private readonly SecondFakeClass _ExpectedRedirectViewModel;
        private readonly string _OriginalRoute = "FirstRoute";
        private static readonly Type _FakeType = typeof(FakeClass);
        private static readonly Type _FakeTypeRedirect = typeof(SecondFakeClass);
        private static readonly Type _FakeSubNavigatorType = typeof(FakeSubNavigator); 

        public class FakeClass
        {
        }

        public class SecondFakeClass
        {
        }

        public NavigationViewModelTests()
        {
            _ExpectedNewViewModel = new FakeClass();
            _ExpectedRedirectViewModel = new SecondFakeClass();
            _ServiceLocator = Substitute.For<IServiceLocator>();
            _ServiceLocator.GetInstance(_FakeType).Returns(_ExpectedNewViewModel);
            _ServiceLocator.GetInstance<FakeClass>().Returns(_ExpectedNewViewModel);
            _ServiceLocator.GetInstance(_FakeTypeRedirect).Returns(_ExpectedRedirectViewModel);
            _ServiceLocator.GetInstance(null).Throws(new ArgumentNullException("serviceType"));
            _RouterSolver = Substitute.For<IRouterSolver>();
            _RouterSolver.SolveRoute(Arg.Any<object>()).Returns(default(string));
            _NavigationViewModel = new NavigationViewModel(new Lazy<IServiceLocator>(() => _ServiceLocator),
                _RouterSolver, _OriginalRoute);
        }

        [Fact]
        public async Task BeforeResolveCommand_cancels_navigation_when_route_not_found()
        {
            var expected = BeforeRouterResult.Cancel();

            var res = await _NavigationViewModel.BeforeResolveCommand.Execute("route not found");

            res.Should().BeEquivalentTo(expected);
        }

        [Theory, AutoData]
        public async Task BeforeResolveCommand_cancels_navigation_when_sub_route_not_found(string route)
        {
            var expected = BeforeRouterResult.Cancel();
            SetupRoute(route);

            var res = await _NavigationViewModel.BeforeResolveCommand.Execute($"{route}/path1");

            res.Should().BeEquivalentTo(expected);
        }

        [Theory, AutoData]
        public async Task BeforeResolveCommand_cancels_navigation_when_nested_sub_route_not_found(string route)
        {
            var expected = BeforeRouterResult.Cancel();
            var subNavigator = Substitute.For<ISubNavigator>();
            subNavigator.NavigateTo("path1").Returns(default(ISubNavigator));
            SetupSubNavigation(subNavigator, route);

            var res = await _NavigationViewModel.BeforeResolveCommand.Execute($"{route}/path1");

            res.Should().BeEquivalentTo(expected);
        }

        [Theory, AutoData]
        public async Task BeforeResolveCommand_continues_navigation_when_route_found(string route)
        {
            var expected = BeforeRouterResult.Ok(_ExpectedNewViewModel);
            SetupRoute(route);

            var res = await _NavigationViewModel.BeforeResolveCommand.Execute(route);

            res.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task BeforeResolveCommand_continues_navigation_when_route_with_sub_path_is_found()
        {
            var expectedViewModel = SetupForSuccessfulNavigation("root");
            var expected = BeforeRouterResult.Ok(expectedViewModel);         

            var res = await _NavigationViewModel.BeforeResolveCommand.Execute("root/path1/path2");

            res.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task BeforeResolveCommand_calls_sub_navigator_to_resolve_view_model()
        {
            var expectedViewModel = SetupForSuccessfulNavigation("root");
            await _NavigationViewModel.BeforeResolveCommand.Execute("root/path1/path2/path3");

            expectedViewModel.Received(3).NavigateTo(Arg.Any<string>());

            Received.InOrder(() => {
                expectedViewModel.NavigateTo("path1");
                expectedViewModel.NavigateTo("path2");
                expectedViewModel.NavigateTo("path3");
            });
        }

        [Theory]
        [InlineAutoData(true)]
        [InlineAutoData(false)]
        public async Task BeforeResolveCommand_cancels_when_other_navigation_is_happening(bool sameRouteNavigation, string route)
        {
            var originalRoute = sameRouteNavigation ? route : $"different_{route}";
            var expected = sameRouteNavigation ? BeforeRouterResult.Ok(_ExpectedNewViewModel) : BeforeRouterResult.Cancel();
            SetupRoute(originalRoute);
            SetupRoute(route);
            var task = _NavigationViewModel.Navigate(originalRoute);

            var res = await _NavigationViewModel.BeforeResolveCommand.Execute(route);

            res.Should().BeEquivalentTo(expected);
        }

        [Theory, AutoData]
        public async Task BeforeResolveCommand_sends_event(string route)
        {
            SetupRoute(route);
            using (var monitor = _NavigationViewModel.Monitor())
            {
                await _NavigationViewModel.BeforeResolveCommand.Execute(route);
                monitor.Should().Raise("OnNavigating").WithArgs<RoutingEventArgs>(arg =>
                    arg.To.ViewModel == _ExpectedNewViewModel);
            }
        }

        [Theory, AutoData]
        public async Task BeforeResolveCommand_can_be_cancelled(string route)
        {
            var expected = BeforeRouterResult.Cancel();
            SetupRoute(route);

            void OnNavigating(object _, RoutingEventArgs e)
            {
                _NavigationViewModel.OnNavigating -= OnNavigating;
                if (e.To.ViewModel == _ExpectedNewViewModel)
                    e.Cancel = true;
            }

            _NavigationViewModel.OnNavigating += OnNavigating;

            var res = await _NavigationViewModel.BeforeResolveCommand.Execute(route);

            res.Should().BeEquivalentTo(expected);
        }

        [Theory, AutoData]
        public async Task BeforeResolveCommand_can_be_redirected(string route, string redirectRoute)
        {
            var expected = BeforeRouterResult.CreateRedirect(redirectRoute);
            SetupRedirect(route, redirectRoute);

            var res = await _NavigationViewModel.BeforeResolveCommand.Execute(route);

            res.Should().BeEquivalentTo(expected);
        }

        [Theory, AutoData]
        public async Task BeforeResolveCommand_cancels_when_redirect_route_not_found(string route)
        {
            var expected = BeforeRouterResult.Cancel();
            SetupRoute(route);

            void OnNavigating(object _, RoutingEventArgs e)
            {
                _NavigationViewModel.OnNavigating -= OnNavigating;
                if (e.To.ViewModel == _ExpectedNewViewModel)
                    e.RedirectToRoute("route 2");
            }

            _NavigationViewModel.OnNavigating += OnNavigating;

            var res = await _NavigationViewModel.BeforeResolveCommand.Execute(route);

            res.Should().BeEquivalentTo(expected);
        }

        [Theory, AutoData]
        public void BeforeResolveCommand_solves_navigate_task_when_cancelled(string route)
        {
            SetupRoute(route);
            var task = _NavigationViewModel.Navigate(route);
            void OnNavigating(object _, RoutingEventArgs e)
            {
                e.Cancel = true;
            }
            _NavigationViewModel.OnNavigating += OnNavigating;

            _NavigationViewModel.BeforeResolveCommand.Execute(route);

            task.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public void AfterResolveCommand_does_not_throw_when_initializing_on_same_route()
        {
            Action execute = () => _NavigationViewModel.AfterResolveCommand.Execute(_OriginalRoute);
            execute.Should().NotThrow();
        }

        [Theory, AutoData]
        public async Task AfterResolveCommand_updates_route(string route)
        {
            SetupRoute(route);
            await _NavigationViewModel.BeforeResolveCommand.Execute(route);

            _NavigationViewModel.AfterResolveCommand.Execute(route);

            _NavigationViewModel.Route.Should().Be(route);
        }

        [Theory, AutoData]
        public void AfterResolveCommand_solves_navigate_task(string route)
        {
            SetupRoute(route);
            var task = _NavigationViewModel.Navigate(route);

            _NavigationViewModel.AfterResolveCommand.Execute(route);

            task.IsCompleted.Should().BeTrue();
        }

        [Theory, AutoData]
        public async Task AfterResolveCommand_update_route_on_mismatch(string route)
        {
            var newRoute = $"new_{route}";
            SetupRoute(route);
            await _NavigationViewModel.BeforeResolveCommand.Execute(route);

            _NavigationViewModel.AfterResolveCommand.Execute(newRoute);

            _NavigationViewModel.Route.Should().Be(newRoute);
        }

        [Theory, AutoData]
        public async Task AfterResolveCommand_send_event(string route)
        {
            SetupRoute(route);
            await _NavigationViewModel.BeforeResolveCommand.Execute(route);

            using (var monitor = _NavigationViewModel.Monitor())
            {
                _NavigationViewModel.AfterResolveCommand.Execute(route);
                monitor.Should().Raise("OnNavigated").WithArgs<RoutedEventArgs>(arg =>
                    arg.NewRoute.ViewModel == _ExpectedNewViewModel && arg.NewRoute.RouteName == route);
            }
        }

        [Theory, AutoData]
        public async Task AfterResolveCommand_send_event_after_redirect(string route, string redirectRoute)
        {
            SetupRedirect(route, redirectRoute);

            await _NavigationViewModel.BeforeResolveCommand.Execute(route);

            using (var monitor = _NavigationViewModel.Monitor())
            {
                _NavigationViewModel.AfterResolveCommand.Execute(redirectRoute);
                monitor.Should().Raise("OnNavigated").WithArgs<RoutedEventArgs>(arg =>
                    arg.NewRoute.ViewModel == _ExpectedRedirectViewModel && arg.NewRoute.RouteName == redirectRoute);
            }
        }

        [Fact]
        public async Task AfterResolveCommand_construct_subNavigator_for_complex_path()
        {
            var expected = GetFakeSubNavigator();
            var type = typeof(FakeSubNavigator);
            const string route = "path1/path2/path3";
            _RouterSolver.SolveType("path1").Returns(type);
            _ServiceLocator.GetInstance(type).Returns(new FakeSubNavigator());

            await _NavigationViewModel.BeforeResolveCommand.Execute(route);

            using (var monitor = _NavigationViewModel.Monitor())
            {
                _NavigationViewModel.AfterResolveCommand.Execute(route);
                monitor.Should().Raise("OnNavigated").WithArgs<RoutedEventArgs>(arg =>
                    arg.NewRoute.RouteName == route);

                var eventRaised = (RoutedEventArgs)monitor.OccurredEvents.First(ev => ev.EventName == "OnNavigated").Parameters[1];
                eventRaised.NewRoute.ViewModel.Should().BeEquivalentTo(expected);
            }
        }

        [Fact]
        public void Navigate_string_returns_immediately_when_route_not_changed()
        {
            var res = _NavigationViewModel.Navigate(_OriginalRoute);
            res.IsCompleted.Should().BeTrue();
        }

        [Theory, AutoData]
        public void Navigate_string_update_route(string route)
        {
            SetupRoute(route);
            _NavigationViewModel.Navigate(route);
            _NavigationViewModel.Route.Should().Be(route);
        }

        [Theory, AutoData]
        public async Task Navigate_string_returns_an_uncompleted_task(string route)
        {
            SetupRoute(route);
            var task = _NavigationViewModel.Navigate(route);
            await Task.Delay(30);
            task.IsCompleted.Should().BeFalse();
        }

        [Theory, AutoData]
        public async Task Navigate_string_returns_throws_when_route_not_found(string route)
        {
            var expectedMessage = $"Route not found: {route}";
            Func<Task> navigate = () => _NavigationViewModel.Navigate(route);
            var exception = await navigate.Should().ThrowAsync<NotImplementedException>();
            exception.WithMessage(expectedMessage);
        }

        [Fact]
        public async Task Navigate_type_context_throws_on_type_null()
        {
            var expectedMessage = new ArgumentNullException("type").Message;
            Func<Task> navigate = () => _NavigationViewModel.Navigate(null, null);
            var exception = await navigate.Should().ThrowAsync<ArgumentNullException>();
            exception.WithMessage(expectedMessage);
        }

        [Theory, AutoData]
        public void Navigate_type_context_updates_route(string route)
        {
            SetupRouteFromType(route);
            var task = _NavigationViewModel.Navigate(_FakeType, null);
            _NavigationViewModel.Route.Should().Be(route);
        }

        [Fact]
        public void Navigate_type_context_resolves_complex_route()
        {
            var viewModel = GetFakeSubNavigator();
            _ServiceLocator.GetInstance(_FakeSubNavigatorType).Returns(viewModel);
            _RouterSolver.SolveRoute(viewModel).Returns("root");
           

            var task = _NavigationViewModel.Navigate(_FakeSubNavigatorType);
            _NavigationViewModel.Route.Should().Be("root/path2/path3");
        }

        [Theory, AutoData]
        public void Navigate_type_context_updates_route_with_context_route_when_provided(string route, string contextRoute)
        {
            SetupRouteFromType(route);
            var task = _NavigationViewModel.Navigate(_FakeType, new NavigationContext{ RouteName = contextRoute });
            _NavigationViewModel.Route.Should().Be(contextRoute);
        }

        [Theory, AutoData]
        public void Navigate_type_context_uses_resolution_key_from_context_route_when_provided(string route, string routeWithoutKey, string resolutionKey)
        {
            var newViewModel = new FakeClass();
            _ServiceLocator.GetInstance(_FakeType, resolutionKey).Returns(newViewModel);
            _RouterSolver.SolveRoute(newViewModel).Returns(route);
            SetupRouteFromType(routeWithoutKey);

            var task = _NavigationViewModel.Navigate(_FakeType, new NavigationContext { ResolutionKey = resolutionKey });
            _NavigationViewModel.Route.Should().Be(route);
        }

        [Theory, AutoData]
        public void Navigate_type_context_is_not_synchronous(string route)
        {
            SetupRouteFromType(route);
            var task = _NavigationViewModel.Navigate(_FakeType, null);
            task.IsCompleted.Should().BeFalse();
        }

        [Fact]
        public void Navigate_type_context_does_not_update_route_when_not_found()
        {
            var task = _NavigationViewModel.Navigate(_FakeType, null);
            _NavigationViewModel.Route.Should().Be(_OriginalRoute);
            task.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public void Navigate_type_context_is_synchronous_when_route_is_the_same()
        {
            SetupRouteFromType(_OriginalRoute);
            var task = _NavigationViewModel.Navigate(_FakeType, null);
            _NavigationViewModel.Route.Should().Be(_OriginalRoute);
            task.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public void Navigate_type_context_send_event_when_route_is_the_same()
        {
            SetupRouteFromType(_OriginalRoute);

            using (var monitor = _NavigationViewModel.Monitor())
            {
                var task = _NavigationViewModel.Navigate(_FakeType, null);
                monitor.Should().Raise("OnNavigated").WithArgs<RoutedEventArgs>(arg =>
                    arg.NewRoute.ViewModel == _ExpectedNewViewModel && arg.NewRoute.RouteName == _OriginalRoute);
            }
        }

        [Theory, AutoData]
        public void Navigate_generic_context_updates_route(string route)
        {
            SetupRouteFromType(route);
            var task = _NavigationViewModel.Navigate<FakeClass>(null);
            _NavigationViewModel.Route.Should().Be(route);
        }

        [Theory, AutoData]
        public void Navigate_generic_context_updates_route_with_context_route_when_provided(string route, string contextRoute)
        {
            SetupRouteFromType(route);
            var task = _NavigationViewModel.Navigate(new NavigationContext<FakeClass> { RouteName = contextRoute });
            _NavigationViewModel.Route.Should().Be(contextRoute);
        }

        [Theory, AutoData]
        public void Navigate_generic_context_uses_resolution_key_from_context_route_when_provided(string route, string routeWithoutKey, string resolutionKey)
        {
            var newViewModel = new FakeClass();
            _ServiceLocator.GetInstance<FakeClass>(resolutionKey).Returns(newViewModel);
            _RouterSolver.SolveRoute(newViewModel).Returns(route);
            SetupRouteFromType(routeWithoutKey);

            var task = _NavigationViewModel.Navigate(new NavigationContext<FakeClass> { ResolutionKey = resolutionKey });
            _NavigationViewModel.Route.Should().Be(route);
        }

        [Theory, AutoData]
        public void Navigate_generic_context_call_cb_on_vm_route_when_provided(string route)
        {
            SetupRouteFromType(route);
            var context = new NavigationContext<FakeClass> {BeforeNavigate = Substitute.For<Action<FakeClass>>()};
            var task = _NavigationViewModel.Navigate(context);
            context.BeforeNavigate.Received(1).Invoke(Arg.Any<FakeClass>());
            context.BeforeNavigate.Received().Invoke(_ExpectedNewViewModel);
        }

        [Theory, AutoData]
        public void Navigate_generic_context_is_not_synchronous(string route)
        {
            SetupRouteFromType(route);
            var task = _NavigationViewModel.Navigate<FakeClass>(null);
            task.IsCompleted.Should().BeFalse();
        }

        [Fact]
        public void Navigate_generic_context_does_not_update_route_when_not_found()
        {
            var task = _NavigationViewModel.Navigate<FakeClass>(null);
            _NavigationViewModel.Route.Should().Be(_OriginalRoute);
            task.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public void Navigate_generic_context_is_synchronous_when_route_is_the_same()
        {
            SetupRouteFromType(_OriginalRoute);
            var task = _NavigationViewModel.Navigate<FakeClass>(null);
            _NavigationViewModel.Route.Should().Be(_OriginalRoute);
            task.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public void Navigate_generic_context_send_event_when_route_is_the_same()
        {
            SetupRouteFromType(_OriginalRoute);

            using (var monitor = _NavigationViewModel.Monitor())
            {
                var task = _NavigationViewModel.Navigate<FakeClass>(null);
                monitor.Should().Raise("OnNavigated").WithArgs<RoutedEventArgs>(arg =>
                    arg.NewRoute.ViewModel == _ExpectedNewViewModel && arg.NewRoute.RouteName == _OriginalRoute);
            }
        }

        [Fact]
        public void Navigate_object_route_resolves_complex_route()
        {
            var viewModel = GetFakeSubNavigator();
            _RouterSolver.SolveRoute(viewModel).Returns("root");

            var task = _NavigationViewModel.Navigate(viewModel, null);

            _NavigationViewModel.Route.Should().Be("root/path2/path3");
        }

        private FakeSubNavigator GetFakeSubNavigator()
        {
            return new FakeSubNavigator()
            {
                RelativeName = "path2",
                SubNavigator = new FakeSubNavigator()
                {
                    RelativeName = "path3",
                    SubNavigator = new FakeSubNavigator()
                }
            };
        }

        private ISubNavigator SetupForSuccessfulNavigation(string root)
        {
            var subNavigator = Substitute.For<ISubNavigator>();
            subNavigator.NavigateTo(Arg.Any<string>()).Returns(subNavigator);
            SetupSubNavigation(subNavigator, root);
            return subNavigator;
        }

        private void SetupSubNavigation(object rootViewModel, string root)
        {
            var typeForSubNavigation = rootViewModel.GetType();
            _RouterSolver.SolveType(root).Returns(typeForSubNavigation);
            _ServiceLocator.GetInstance(typeForSubNavigation).Returns(rootViewModel);
        }

        private void SetupRedirect(string route, string redirectRoute)
        {
            SetupRoute(route);
            _RouterSolver.SolveType(redirectRoute).Returns(_FakeTypeRedirect);

            void OnNavigating(object _, RoutingEventArgs e)
            {
                _NavigationViewModel.OnNavigating -= OnNavigating;
                if (e.To.ViewModel == _ExpectedNewViewModel)
                    e.RedirectToRoute(redirectRoute);
            }

            _NavigationViewModel.OnNavigating += OnNavigating;
        }

        private void SetupRoute(string route)
        {
            _RouterSolver.SolveType(route).Returns(_FakeType);
        }

        private void SetupRouteFromType(string route)
        {
            _RouterSolver.SolveRoute(_ExpectedNewViewModel).Returns(route);
        }
    }
}
