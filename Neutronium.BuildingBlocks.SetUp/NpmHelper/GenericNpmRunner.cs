using Neutronium.BuildingBlocks.SetUp.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Neutronium.BuildingBlocks.SetUp.NpmHelper
{
    /// <summary>
    /// Npm generic runner implementation
    /// </summary>
    public class GenericNpmRunner : INoResultNpmRunner
    {
        private static readonly Regex _Key = new Regex(@"\((\w)\/\w\)\?", RegexOptions.Compiled);

        private Process ResolvedProcess => _Process.Value;
        private Task<string> StopKeyAsync => _StopKeyCompletionSource.Task;

        private readonly Lazy<Process> _Process;
        private readonly string _Script;
        private readonly TaskCompletionSource<int> _RunCompletionSource = new TaskCompletionSource<int>();
        private readonly TaskCompletionSource<string> _StopKeyCompletionSource = new TaskCompletionSource<string>();
        private volatile State _State = State.NotStarted;
        private readonly string _WorkingDirectory;
        public event EventHandler<MessageEventArgs> OnMessageReceived;

        private enum State
        {
            NotStarted,
            Initializing,
            Cancelling,
            Closing,
            Closed
        };

        public GenericNpmRunner(string directory, string script)
        {
            _Script = script;
            var root = DirectoryHelper.GetCurrentDirectory();
            _WorkingDirectory = Path.Combine(root, directory);
            _Process = new Lazy<Process>(CreateProcess);
        }

        private Process CreateProcess()
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = "cmd",
                    RedirectStandardInput = true,
                    WorkingDirectory = _WorkingDirectory,
                    CreateNoWindow = true,
                    ErrorDialog = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };

            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.OutputDataReceived += Process_OutputDataReceived;
            process.Exited += Process_Exited;
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            return process;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            _State = State.Closed;
            _RunCompletionSource.TrySetResult(0);
        }

        public Task Run(CancellationToken cancellationToken)
        {
            Start(cancellationToken);
            return _RunCompletionSource.Task.WithCancellation(cancellationToken, false);
        }

        private void Start(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => StopIfNeed(cancellationToken));

            if (_State != State.NotStarted)
                return;

            _State = State.Initializing;
            ResolvedProcess.StandardInput.WriteLine($"npm run {_Script} && exit");
        }

        private async void StopIfNeed(CancellationToken cancellationToken)
        {
            if (_RunCompletionSource.Task.IsCompleted)
                return;

            await Stop(State.Cancelling, State.NotStarted);
        }

        public async Task<bool> Cancel()
        {
            if (!_Process.IsValueCreated)
            {
                _State = State.Closed;
                return true;
            }

            var res = await Stop(State.Closing, State.Closed).ConfigureAwait(false);
            if (!res)
                return false;

            ResolvedProcess.ErrorDataReceived -= Process_ErrorDataReceived;
            ResolvedProcess.OutputDataReceived -= Process_OutputDataReceived;
            ResolvedProcess.Exited -= Process_Exited;
            ResolvedProcess.Dispose();
            return true;
        }

        private async Task<bool> Stop(State transitionState, State finalState)
        {
            switch (_State)
            {
                case State.Closed:
                case State.Cancelling:
                    return false;

                case State.NotStarted:
                    _State = finalState;
                    return true;
            }

            var currentState = _State;
            _State = transitionState;
            if (!ResolvedProcess.SendControlC())
            {
                _State = currentState;
                return false;
            }

            var standardInput = ResolvedProcess.StandardInput;
            standardInput.WriteLine();
            standardInput.WriteLine(await StopKeyAsync.ConfigureAwait(false));
            _State = finalState;
            return true;
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            var data = e.Data;
            if (data == null)
            {
                _RunCompletionSource.TrySetResult(0);
                _State = State.Closed;
                return;
            }

            OnMessageReceived?.Invoke(this, new MessageEventArgs(e.Data, false));

            switch (_State)
            {
                case State.Cancelling:
                case State.Closing:
                    TryParseKey(data);
                    break;
            };
        }

        private void TryParseKey(string data)
        {
            var match = _Key.Match(data);
            if (!match.Success)
                return;

            _StopKeyCompletionSource.TrySetResult(match.Groups[1].Value);
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnMessageReceived?.Invoke(this, new MessageEventArgs(e.Data, true));
        }

        public void Dispose()
        {
            Cancel().Wait();
        }

        public override string ToString() => _State.ToString();
    }
}
