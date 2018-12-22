using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;
using AutoFixture.Xunit2;

namespace Application.SetUp.Tests
{
    public class ApplicationSetUpBuilderTests
    {
        private readonly ApplicationSetUpBuilder _ApplicationSetUpBuilder;
        private readonly Uri _ProductionUri;

        public ApplicationSetUpBuilderTests()
        {
            _ProductionUri = GetDummyUri();
            _ApplicationSetUpBuilder = new ApplicationSetUpBuilder(_ProductionUri);
        }

        private static Uri GetDummyUri()
        {
            return new Uri("file://test/index.html");
        }

        private static Uri GetLiveUri(int port = 8080)
        {
            return new Uri($"http://localhost:{port}/index.html");
        }

        public static IEnumerable<object[]> GetParameters()
        {
            yield return new object[] { null, new ApplicationSetUp(ApplicationMode.Dev, GetDummyUri()) };
            yield return new object[] { new string[] { }, new ApplicationSetUp(ApplicationMode.Dev, GetDummyUri()) };
            yield return new object[] { new[] { "-mode=dev" }, new ApplicationSetUp(ApplicationMode.Dev, GetDummyUri()) };
            yield return new object[] { new[] { "-mode=live" }, new ApplicationSetUp(ApplicationMode.Live, GetLiveUri()) };
            yield return new object[] { new[] { "-mode=live", "-port=90" }, new ApplicationSetUp(ApplicationMode.Live, GetLiveUri(90)) };
            yield return new object[] { new[] { "-mode=live", "-port=oo" }, new ApplicationSetUp(ApplicationMode.Live, GetLiveUri()) };
            yield return new object[] { new[] { "-mode=prod" }, new ApplicationSetUp(ApplicationMode.Production, GetDummyUri()) };
            yield return new object[] { new[] { "-mode=prod", "-port=90" }, new ApplicationSetUp(ApplicationMode.Production, GetDummyUri()) };
            yield return new object[] { new[] { "-mode=live", "-url=http://www.google.com" }, new ApplicationSetUp(ApplicationMode.Live, new Uri("http://www.google.com")) };
            yield return new object[] { new[] { "-mode=dev", "-url=http://www.duck.com" }, new ApplicationSetUp(ApplicationMode.Dev, new Uri("http://www.duck.com")) };
            yield return new object[] { new[] { "-mode=prod", "-url=http://www.github.com" }, new ApplicationSetUp(ApplicationMode.Production, new Uri("http://www.github.com")) };
        }

        [Theory]
        [MemberData(nameof(GetParameters))]
        public void Parse_parses_arguments(string[] parameters, ApplicationSetUp expectedApplicationSetUp)
        {
            var res = _ApplicationSetUpBuilder.BuildFromApplicationArguments(parameters);
            res.Should().BeEquivalentTo(expectedApplicationSetUp);
        }

        [Theory, AutoData]
        public void Parse_uses_default_port_in_live_mode(int port)
        {
            var expectedString = GetLiveUri(port);
            var applicationSetUpBuilder = new ApplicationSetUpBuilder(_ProductionUri, defaultPort: port);
            var res = applicationSetUpBuilder.BuildFromApplicationArguments(new [] { "-mode=live" });
            res.Uri.Should().Be(expectedString);
        }

        [Theory, AutoData]
        public void Parse_uses_default_mode(ApplicationMode mode)
        {
            var applicationSetUpBuilder = new ApplicationSetUpBuilder(_ProductionUri, @default: mode);
            var res = applicationSetUpBuilder.BuildFromApplicationArguments(new string [] { });
            res.Mode.Should().Be(mode);
        }

        [Fact]
        public void BuildForProduction_creates_a_production_set_up()
        {
            var expected = new ApplicationSetUp(ApplicationMode.Production, _ProductionUri);
            var res = _ApplicationSetUpBuilder.BuildForProduction();
            res.Should().BeEquivalentTo(expected);
        }
    }
}
