using System;

namespace Neutronium.BuildingBlocks.ApplicationTools
{
    /// <summary>
    /// Result of a command that may throw exception
    /// </summary>
    /// <typeparam name="TResult">
    /// Command result type
    /// </typeparam>
    public class CommandResult<TResult>
    {
        /// <summary>
        /// Result
        /// </summary>
        public TResult Result { get; }

        /// <summary>
        /// Exception if any
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Success
        /// </summary>
        public bool Success => (!Cancelled && !HasError);

        /// <summary>
        /// HasError
        /// </summary>
        public bool HasError => (Exception != null);

        /// <summary>
        /// Cancelled
        /// </summary>
        public bool Cancelled { get; }

        public CommandResult()
        {
            Result = default(TResult);
            Cancelled = true;
        }

        public CommandResult(TResult result)
        {
            Result = result;
            Exception = null;
        }

        public CommandResult(Exception exception)
        {
            Result = default(TResult);
            Exception = exception;
        }
    }
}