using System.Threading.Tasks;

namespace Vm.Tools.Dialog 
{
    public interface IIODialog 
    {
        string Title { get; set; }
        string Directory { get; set; }

        Task<string> Choose();
    }
}
