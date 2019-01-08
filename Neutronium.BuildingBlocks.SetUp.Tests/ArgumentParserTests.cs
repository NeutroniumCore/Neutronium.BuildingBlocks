using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Neutronium.BuildingBlocks.SetUp.Tests
{
    public class ArgumentParserTests
    {
        public static IEnumerable<object[]> GetParameters()
        {
            yield return new object[] { null, new Dictionary<string, string>() };
            yield return new object[] { new string[] { }, new Dictionary<string, string>() };
            yield return new object[] { new[] { "--s" }, new Dictionary<string, string>() };
            yield return new object[] { new[] { "--s=23" }, new Dictionary<string, string>() };
            yield return new object[] { new[] { "--switch1" }, new Dictionary<string, string> { ["switch1"] = "true" } };
            yield return new object[] { new[] { "-s" }, new Dictionary<string, string> { ["s"] = "true" } };         
            yield return new object[] { new[] { "-d=value" }, new Dictionary<string, string> { ["d"] = "value" } };
            yield return new object[] { new[] { "--switch2=value2" }, new Dictionary<string, string> { ["switch2"] = "value2" } };
            yield return new object[] { new[] { "-d=value2" }, new Dictionary<string, string> { ["d"] = "value2" } };
            yield return new object[] { new[] { "--switch1", "--switch2=value2" }, new Dictionary<string, string> { ["switch1"] = "true", ["switch2"] = "value2" } };
            yield return new object[] { new[] { "-a", "-b=value2" }, new Dictionary<string, string> { ["a"] = "true", ["b"] = "value2" } };
            yield return new object[] { new[] { "--switch1", "--switch1" }, new Dictionary<string, string> { ["switch1"] = "true" } };
        }

        [Theory]
        [MemberData(nameof(GetParameters))]
        public void Parse_parses_arguments(string[] parameters, IDictionary<string, string> parsed)
        {
            var res = ArgumentParser.Parse(parameters);
            res.Should().BeEquivalentTo(parsed);
        }
    }
}
