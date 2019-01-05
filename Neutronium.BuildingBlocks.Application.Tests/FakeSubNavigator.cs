using Neutronium.BuildingBlocks.Application.Navigation;

namespace Neutronium.BuildingBlocks.Application.Tests
{
    public class FakeSubNavigator : ISubNavigator
    {
        public string RelativeName { get; }
        public ISubNavigator Child => SubNavigator;

        internal FakeSubNavigator SubNavigator { get; set; }     

        public FakeSubNavigator(string name)
        {
            RelativeName = name;
        }

        public ISubNavigator NavigateTo(string relativePath)
        {
            SubNavigator = new FakeSubNavigator(relativePath);
            return SubNavigator;
        }

        public override string ToString()
        {
            return $"{RelativeName}/{Child}";
        }
    }
}
