using Neutronium.BuildingBlocks.Application.Navigation;

namespace Neutronium.BuildingBlocks.Application.Tests
{
    public class FakeSubNavigator : ISubNavigator
    {
        public string RelativeName { get; set; }
        public ISubNavigator Child => SubNavigator;

        internal FakeSubNavigator SubNavigator { get; set; }     

        public ISubNavigator NavigateTo(string relativePath)
        {
            RelativeName = relativePath;
            SubNavigator = new FakeSubNavigator();
            return SubNavigator;
        }

        public override string ToString()
        {
            return $"{RelativeName}/{Child}";
        }
    }
}
