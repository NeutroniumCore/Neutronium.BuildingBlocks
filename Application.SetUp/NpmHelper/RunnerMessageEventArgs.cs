using System;

namespace Application.SetUp.NpmHelper
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
