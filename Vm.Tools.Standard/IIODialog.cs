namespace Vm.Tools.Standard 
{
    public interface IIODialog 
    {
        string Title { get; set; }
        string Directory { get; set; }

        string Choose();
    }
}
