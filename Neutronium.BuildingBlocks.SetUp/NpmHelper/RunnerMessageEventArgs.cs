using System;

namespace Neutronium.BuildingBlocks.SetUp.NpmHelper
{
    public class RunnerMessageEventArgs : EventArgs
    {
        public string Message { get; }

        public RunnerMessageEventArgs(string message)
        {
            Message = message;
        }
    }
}
