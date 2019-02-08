using Neutronium.BuildingBlocks.Application.Navigation;

namespace Neutronium.BuildingBlocks.Application.Tests
{
    public class FakeSubNavigatorFactory : ISubNavigatorFactory
    {
        public string ChildName { get; set; }
        public ISubNavigator Child => SubNavigatorFactory;

        internal FakeSubNavigatorFactory SubNavigatorFactory { get; set; }     

        public ISubNavigator Create(string relativePath)
        {
            ChildName = relativePath;
            SubNavigatorFactory = new FakeSubNavigatorFactory();
            return SubNavigatorFactory;
        }

        public override string ToString()
        {
            return $"{ChildName}/{Child}";
        }
    }
}
