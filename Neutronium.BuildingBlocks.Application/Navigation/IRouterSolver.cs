using System;

namespace Neutronium.BuildingBlocks.Application.Navigation
{
    public interface IRouterSolver
    {
        string SolveRoute(object viewModel);

        string SolveRoute<T>();

        Type SolveType(string route);
    }
}