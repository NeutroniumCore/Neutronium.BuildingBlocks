using FluentAssertions;
using Neutronium.BuildingBlocks.Application.Navigation;
using System.Collections.Generic;
using Neutronium.BuildingBlocks.Application.Navigation.Internals;
using Xunit;

namespace Neutronium.BuildingBlocks.Application.Tests
{
    public class PathContextTests
    {
        private readonly PathContext _PathContext;

        public PathContextTests()
        {
            _PathContext = new PathContext("root/p1/p2/p3/p4");
        }

        [Fact]
        public void CompletePath_returns_original_path()
        {
            _PathContext.CompletePath.Should().Be("root/p1/p2/p3/p4");
        }

        [Fact]
        public void CurrentRelativePath_first_returns_null()
        {
            _PathContext.CurrentRelativePath.Should().BeNull();
        }

        [Fact]
        public void CurrentRelativePath_returns_all_Paths()
        {
            var paths = new List<string>();
            while (_PathContext.Back())
            {
                paths.Add(_PathContext.CurrentRelativePath);
            }
            paths.Should().BeEquivalentTo("p4", "p3", "p2", "p1", "root");
        }

        [Fact]
        public void RootToCurrent_first_returns_complete_path()
        {
            _PathContext.RootToCurrent.Should().Be("root/p1/p2/p3/p4");
        }

        [Fact]
        public void RootToCurrent_returns_all_Paths()
        {
            var paths = new List<string>();
            while (_PathContext.Back())
            {
                paths.Add(_PathContext.RootToCurrent);
            }
            paths.Should().BeEquivalentTo("root/p1/p2/p3", "root/p1/p2", "root/p1", "root", null);
        }

        [Fact]
        public void CurrentToEnd_first_returns_null()
        {
            _PathContext.CurrentToEnd.Should().BeNull();
        }

        [Fact]
        public void CurrentToEnd_returns_all_Paths()
        {
            var paths = new List<string>();
            while (_PathContext.Back())
            {
                paths.Add(_PathContext.CurrentToEnd);
            }
            paths.Should().BeEquivalentTo("p4", "p3/p4", "p2/p3/p4", "p1/p2/p3/p4", "root/p1/p2/p3/p4");
        }

        [Fact]
        public void Next_first_returns_false()
        {
            _PathContext.Next().Should().BeFalse();
        }

    }
}
