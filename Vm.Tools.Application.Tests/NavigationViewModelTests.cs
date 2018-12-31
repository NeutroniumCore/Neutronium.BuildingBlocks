using AutoFixture.Xunit2;
using CommonServiceLocator;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Threading.Tasks;
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
            _NavigationViewModel = new NavigationViewModel(new Lazy<IServiceLocator>(() => _ServiceLocator), _RouterSolver);
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
            _RouterSolver.SolveType(route).Returns(_FakeType);
            _RouterSolver.SolveType(redirectRoute).Returns(_FakeTypeRedirect);
            void OnNavigating(object _, RoutingEventArgs e)
            {
                _NavigationViewModel.OnNavigating -= OnNavigating;
                if (e.To.ViewModel == _ExpectedNewViewModel)
                    e.RedirectToRoute(redirectRoute);
            }
            _NavigationViewModel.OnNavigating += OnNavigating;

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
    }
}
