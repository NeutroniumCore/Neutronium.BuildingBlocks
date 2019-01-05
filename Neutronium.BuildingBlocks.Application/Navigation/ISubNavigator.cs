namespace Neutronium.BuildingBlocks.Application.Navigation
{
    public interface ISubNavigator
    {
        string RelativeName { get; }

        ISubNavigator Child { get; }

        ISubNavigator NavigateTo(string relativePath);
    }
}
