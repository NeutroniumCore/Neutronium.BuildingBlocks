namespace Neutronium.BuildingBlocks.ApplicationTools 
{
    public interface IIODialog 
    {
        string Title { get; set; }
        string Directory { get; set; }
        string Choose();
    }
}
