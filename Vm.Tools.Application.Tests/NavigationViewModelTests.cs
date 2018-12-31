using AutoFixture.Xunit2;
using CommonServiceLocator;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Threading.Tasks;
using NSubstitute.Routing;
using Vm.Tools.Application.Navigation;
using Xunit;

namespace Vm.Tools.Application.Tests
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

        private class FakeClass { }
        private class SecondFakeClass { }

        public NavigationViewModelTests()
        {
            _ExpectedNewViewModel = new FakeClass();
            _ExpectedRedirectViewModel = new SecondFakeClass();
            _ServiceLocator = Substitute.For<IServiceLocator>();
            _ServiceLocator.GetInstance(_FakeType).Returns(_ExpectedNewViewModel);
            _ServiceLocator.GetInstance(_FakeTypeRedirect).Returns(_ExpectedRedirectViewModel);
            _ServiceLocator.GetInstance(null).Throws(new ArgumentNullException("serviceType"));
            _RouterSolver = Substitute.For<IRouterSolver>();
            _NavigationViewModel = new NavigationViewModel(new Lazy<IServiceLocator>(() => _ServiceLocator), _RouterSolver, _OriginalRoute);
        }

        [Fact]
        public async Task BeforeResolveCommand_cancels_navigation_when_route_not_found()
        {
            var expected = BeforeRouterResult.Cancel();

            var res = await _NavigationViewModel.BeforeResolveCommand.Execute("route not found");

            res.Should().BeEquivalentTo(expected);
        }

        [Theory, AutoData]
        public async Task BeforeResolveCommand_continues_navigation_when_route_found(string route)
        {
            var expected = BeforeRouterResult.Ok(_ExpectedNewViewModel);
            _RouterSolver.SolveType(route).Returns(_FakeType);

            var res = await _NavigationViewModel.BeforeResolveCommand.Execute(route);

            res.Should().BeEquivalentTo(expected);
        }

        [Theory, AutoData]
        public async Task BeforeResolveCommand_sends_event(string route)
        {
            _RouterSolver.SolveType(route).Returns(_FakeType);
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
            _RouterSolver.SolveType(route).Returns(_FakeType);
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
            _RouterSolver.SolveType(route).Returns(_FakeType);
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

        [Fact]
        public void AfterResolveCommand_does_not_throw_when_initializing_on_same_route()
        {
            Action execute = () => _NavigationViewModel.AfterResolveCommand.Execute(_OriginalRoute);
            execute.Should().NotThrow();
        }

        [Theory, AutoData]
        public async Task AfterResolveCommand_update_route(string route)
        {
            _RouterSolver.SolveType(route).Returns(_FakeType);
            await _NavigationViewModel.BeforeResolveCommand.Execute(route);

            _NavigationViewModel.AfterResolveCommand.Execute(route);

            _NavigationViewModel.Route.Should().Be(route);
        }

        [Theory, AutoData]
        public async Task AfterResolveCommand_update_route_on_mismatch(string route)
        {
            var newRoute = $"new_{route}";
            _RouterSolver.SolveType(route).Returns(_FakeType);
            await _NavigationViewModel.BeforeResolveCommand.Execute(route);

            _NavigationViewModel.AfterResolveCommand.Execute(newRoute);

            _NavigationViewModel.Route.Should().Be(newRoute);
        }

        [Theory, AutoData]
        public async Task AfterResolveCommand_send_event(string route)
        {
            _RouterSolver.SolveType(route).Returns(_FakeType);
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

        private void SetupRedirect(string route, string redirectRoute)
        {
            _RouterSolver.SolveType(route).Returns(_FakeType);
            _RouterSolver.SolveType(redirectRoute).Returns(_FakeTypeRedirect);
            void OnNavigating(object _, RoutingEventArgs e)
            {
                _NavigationViewModel.OnNavigating -= OnNavigating;
                if (e.To.ViewModel == _ExpectedNewViewModel)
                    e.RedirectToRoute(redirectRoute);
            }
            _NavigationViewModel.OnNavigating += OnNavigating;
        }
    }
}
