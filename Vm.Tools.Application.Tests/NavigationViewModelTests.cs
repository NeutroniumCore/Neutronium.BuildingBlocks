using CommonServiceLocator;
using NSubstitute;
using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using NSubstitute.ExceptionExtensions;
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
        private static readonly Type _FakeType = typeof(FakeClass);

        private class FakeClass { }

        public NavigationViewModelTests()
        {
            _ExpectedNewViewModel = new FakeClass();
            _ServiceLocator = Substitute.For<IServiceLocator>();
            _ServiceLocator.GetInstance(_FakeType).Returns(_ExpectedNewViewModel);
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

        [Theory,AutoData]
        public async Task BeforeResolveCommand_continues_navigation_when_route_found(string route)
        {
            var expected = BeforeRouterResult.Ok(_ExpectedNewViewModel);

            _RouterSolver.SolveType(route).Returns(_FakeType);
            var res = await _NavigationViewModel.BeforeResolveCommand.Execute(route);
            
            res.Should().BeEquivalentTo(expected);
        }
    }
}
