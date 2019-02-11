using AutoFixture.Xunit2;
using FluentAssertions;
using Neutronium.BuildingBlocks.Application.Navigation;
using System;
using Xunit;

namespace Neutronium.BuildingBlocks.Application.Tests
{
    public class RouterTests
    {
        private readonly Router _Router;
        private readonly RouteDestination _RouteDestination1;
        private readonly RouteDestination _RouteDestination2;

        public RouterTests()
        {
            _RouteDestination1 = new RouteDestination(typeof(FakeClass3), resolutionKey: "key");
            _RouteDestination2 = new RouteDestination(typeof(FakeClass4));
            _Router = new Router();
            _Router.Register<FakeClass1>("1");
            _Router.Register(typeof(FakeClass2), "2");
            _Router.Register(new RouteSpecification("3"), _RouteDestination1);
            _Router.Register(new RouteSpecification("a", "1"), _RouteDestination2);
        }

        [Theory]
        [InlineData(typeof(FakeClass1), "1")]
        [InlineData(typeof(FakeClass2), "2")]
        [InlineData(typeof(FakeClass3), "3")]
        [InlineData(typeof(FakeClass5), null)]
        public void SolveRoute_finds_correct_route(Type type, string expected)
        {
            var route = GetRouteForType(type);
            route.Should().Be(expected);
        }

        [Fact]
        public void SolveRoute_uses_derivation()
        {
            var route = _Router.SolveRoute<DerivedClass>();

            route.Should().Be("1");
        }

        [Theory]
        [InlineData(typeof(FakeClass1), "1")]
        [InlineData(typeof(FakeClass2), "2")]
        [InlineData(typeof(FakeClass3), "3")]
        public void SolveType_finds_correct_route(Type expected, string route)
        {
            var context = _Router.SolveType(route);
            context.Type.Should().Be(expected);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("b")]
        [InlineData("c")]
        public void Register_is_context_dependent(string context)
        {
            var route = _Router.SolveType("a", context);
            route.Type.Should().BeNull();
        }

        [Fact]
        public void Register_with_context_does_not_alter_solve_root()
        {
            var context = _Router.SolveRoute<FakeClass4>();
            context.Should().BeNull();
        }

        [Fact]
        public void Solve_finds_route_with_context()
        {
            var context = _Router.SolveType("a", "1");
            context.Type.Should().Be(typeof(FakeClass4));
        }

        [Fact]
        public void SolveType_returns_full_context()
        {
            var routeDestination = _Router.SolveType("3");

            routeDestination.Should().Be(_RouteDestination1);
        }

        [Theory]
        [AutoData]
        public void Register_allows_multiple_register_for_same_path(bool @default)
        {
            _Router.Register<FakeClass5>("1", @default);

            var route = _Router.SolveRoute<FakeClass5>();
            route.Should().Be("1");
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void Register_overrides_path_only_when_default_type_is_true(bool @default, bool @override)
        {
            var expectedType = @override ? typeof(FakeClass5) : typeof(FakeClass1);
            _Router.Register<FakeClass5>("1", @default);

            var context = _Router.SolveType("1");
            context.Type.Should().Be(expectedType);
        }

        private string GetRouteForType(Type type)
        {
            var viewModel = Activator.CreateInstance(type);
            return _Router.SolveRoute(viewModel);
        }
    }
}
