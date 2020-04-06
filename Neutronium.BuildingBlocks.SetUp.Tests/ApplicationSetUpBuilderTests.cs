using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Neutronium.BuildingBlocks.SetUp.NpmHelper;
using NSubstitute;
using Xunit;

namespace Neutronium.BuildingBlocks.SetUp.Tests
{
    public class ApplicationSetUpBuilderTests
    {
        private readonly ApplicationSetUpBuilder _ApplicationSetUpBuilder;
        private readonly Uri _ProductionUri;
        private readonly ApplicationMode _Default = ApplicationMode.Dev;
        private readonly INpmLiveRunner _NpmLiveRunner;
        private readonly INoResultNpmRunner _NpmBuildRunner;
        private const int Port = 8080; 

        public ApplicationSetUpBuilderTests()
        {
            RegisterPackScheme();
            _ProductionUri = GetDummyUri();
            _NpmLiveRunner = Substitute.For<INpmLiveRunner>();
            _NpmBuildRunner = Substitute.For<INoResultNpmRunner>();
            _NpmLiveRunner.GetPortAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(Port));
            _NpmBuildRunner.Run(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            _ApplicationSetUpBuilder = new ApplicationSetUpBuilder(_ProductionUri, _Default, _NpmLiveRunner, _NpmBuildRunner);
        }

        private void RegisterPackScheme()
        {
            if (UriParser.IsKnownScheme("pack"))
                return;

            var packParser = new PackParser();
            UriParser.Register(packParser, "pack", 0);
        }

        private class PackParser : UriParser
        {
            protected override string GetComponents(Uri uri, UriComponents components, UriFormat format)
            {
                if (components == UriComponents.AbsoluteUri)
                    return $"pack:{uri.AbsolutePath}";

                return base.GetComponents(uri, components, format);
            }
        }

        private static Uri GetDummyUri()
        {
            return new Uri("file://test/index.html");
        }

        private static Uri GetLiveUri(int port)
        {
            return new Uri($"http://localhost:{port}/index.html");
        }

        public static IEnumerable<object[]> GetParameters()
        {
            yield return new object[] { null, new ApplicationSetUp(ApplicationMode.Dev, GetDummyUri()) };
            yield return new object[] { new string[] { }, new ApplicationSetUp(ApplicationMode.Dev, GetDummyUri()) };
            yield return new object[] { new[] { "--mode=dev" }, new ApplicationSetUp(ApplicationMode.Dev, GetDummyUri()) };
            yield return new object[] { new[] { "--mode=live" }, new ApplicationSetUp(ApplicationMode.Live, GetLiveUri(Port)) };
            yield return new object[] { new[] { "--mode=prod" }, new ApplicationSetUp(ApplicationMode.Production, GetDummyUri()) };
            yield return new object[] { new[] { "-m=dev" }, new ApplicationSetUp(ApplicationMode.Dev, GetDummyUri()) };
            yield return new object[] { new[] { "-m=live" }, new ApplicationSetUp(ApplicationMode.Live, GetLiveUri(Port)) };
            yield return new object[] { new[] { "-m=prod" }, new ApplicationSetUp(ApplicationMode.Production, GetDummyUri()) };
            yield return new object[] { new[] { "--mode=live", "--url=http://www.google.com" }, new ApplicationSetUp(ApplicationMode.Live, new Uri("http://www.google.com")) };
            yield return new object[] { new[] { "--mode=dev", "--url=http://www.duck.com" }, new ApplicationSetUp(ApplicationMode.Dev, new Uri("http://www.duck.com")) };
            yield return new object[] { new[] { "--mode=prod", "--url=http://www.github.com" }, new ApplicationSetUp(ApplicationMode.Production, new Uri("http://www.github.com")) };
            yield return new object[] { new[] { "--mode=live", "-u=http://www.google.com" }, new ApplicationSetUp(ApplicationMode.Live, new Uri("http://www.google.com")) };
            yield return new object[] { new[] { "--mode=dev", "-u=http://www.duck.com" }, new ApplicationSetUp(ApplicationMode.Dev, new Uri("http://www.duck.com")) };
            yield return new object[] { new[] { "-m=prod", "-u=http://www.github.com" }, new ApplicationSetUp(ApplicationMode.Production, new Uri("http://www.github.com")) };
        }

        [Theory]
        [MemberData(nameof(GetParameters))]
        public async Task BuildFromApplicationArguments_parses_arguments(string[] parameters, ApplicationSetUp expectedApplicationSetUp)
        {
            var res = await _ApplicationSetUpBuilder.BuildFromApplicationArguments(parameters);
            res.Should().BeEquivalentTo(expectedApplicationSetUp);
        }

        [Theory, AutoData]
        public async Task BuildFromApplicationArguments_uses_port_from_runner(int port)
        {
            var expected = new ApplicationSetUp(ApplicationMode.Live, GetLiveUri(port));
            _NpmLiveRunner.GetPortAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(port));

            var res = await _ApplicationSetUpBuilder.BuildFromApplicationArguments(new [] { "--mode=live"});
            res.Should().BeEquivalentTo(expected);
        }

        [Theory, AutoData]
        public async Task BuildFromApplicationArguments_uses_default_mode(ApplicationMode mode)
        {
            var applicationSetUpBuilder = new ApplicationSetUpBuilder(_ProductionUri, mode, _NpmLiveRunner, _NpmBuildRunner);
            var res = await applicationSetUpBuilder.BuildFromApplicationArguments(new string [] { });
            res.Mode.Should().Be(mode);
        }

        [Theory, AutoData]
        public async Task BuildFromMode_uses_mode(ApplicationMode mode)
        {
            var expected = new ApplicationSetUp(mode, GetDummyUri());
            var res = await _ApplicationSetUpBuilder.BuildFromMode(mode, CancellationToken.None);
            res.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void BuildForProduction_creates_a_production_set_up()
        {
            var expected = new ApplicationSetUp(ApplicationMode.Production, _ProductionUri);
            var res = _ApplicationSetUpBuilder.BuildForProduction();
            res.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("View", "pack://application:,,,/View/dist/index.html")]
        [InlineData(@"View\Main", "pack://application:,,,/View/Main/dist/index.html")]
        public async Task Constructor_public_build_url(string viewDirectory, string expected)
        {
            var applicationSetUpBuilder = new ApplicationSetUpBuilder(viewDirectory);
            var res = await applicationSetUpBuilder.BuildFromApplicationArguments(new string[] { });
            res.Uri.ToString().Should().Be(expected);
        }
    }
}
