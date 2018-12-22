using FluentAssertions;
using Vm.Tools.Application.ViewModel;
using Xunit;

namespace Vm.Tools.Tests
{
    public class ApplicationInformationTests
    {
        [Fact]
        public void Version_returns_call_assembly_version()
        {
            var res = new ApplicationInformation(string.Empty, string.Empty);
            res.Version.Should().Be("12.10.3.0");
        }
    }
}
