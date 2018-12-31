namespace Vm.Tools.Application.Navigation
{
    public interface ISubNavigator
    {
        ISubNavigator NavigateTo(string relativePath);
    }
}
