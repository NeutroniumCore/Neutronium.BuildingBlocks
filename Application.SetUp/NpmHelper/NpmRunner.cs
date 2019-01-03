using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.SetUp.NpmHelper
{
    public class NpmRunner : INpmRunner
    {
        private static readonly Regex _LocalHost = new Regex(@"http:\/\/localhost:(\d{1,4})", RegexOptions.Compiled);
        private static readonly Regex _Key = new Regex(@"\((\w)\/\w\)\?", RegexOptions.Compiled);

        private Process _Process;
        private readonly string _Script;
        private readonly TaskCompletionSource<int> _PortFinderCompletionSource = new TaskCompletionSource<int>();
        private readonly TaskCompletionSource<string> _StopKeyCompletionSource = new TaskCompletionSource<string>();
        private Task<string> StopKeyAsync => _StopKeyCompletionSource.Task;
        private State _State = State.NotStarted;

        public event DataReceivedEventHandler OutputDataReceived;
        public event DataReceivedEventHandler ErrorDataReceived;

        private enum State
        {
            NotStarted,
            Initializing,
            Running,
            Closing,
            Closed
        };

        public NpmRunner(string directory, string script)
        {
            _Script = script;
            var root = DirectoryHelper.GetCurrentDirectory();
            var workingDirectory = Path.Combine(root, directory);

            _Process = new Process
            {
                StartInfo =
                {
                    FileName = "cmd",
                    RedirectStandardInput = true,
                    WorkingDirectory = workingDirectory,
                    CreateNoWindow = true,
                    ErrorDialog = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };

            _Process.ErrorDataReceived += Process_ErrorDataReceived;
            _Process.OutputDataReceived += Process_OutputDataReceived;
        }

        public Task<int> GetPortAsync()
        {
            Start();
            return _PortFinderCompletionSource.Task;
        }

        private void Start()
        {
            if (_State != State.NotStarted)
                return;

            _Process.Start();
            _Process.StandardInput.WriteLine($"npm run {_Script}");
            _Process.BeginErrorReadLine();
            _Process.BeginOutputReadLine();
            _State = State.Initializing;
        }

        public async Task<bool> Cancel()
        {
            if ((_State == State.Closed) || (_State == State.NotStarted))
                return false;

            _State = State.Closing;

            if (!_Process.SendControlC())
                return false;

            var standardInput = _Process.StandardInput;
            standardInput.WriteLine();
            standardInput.WriteLine(await StopKeyAsync.ConfigureAwait(false));
            _Process.Dispose();
            _Process = null;
            _State = State.Closed;
            return true;
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            var data = e.Data;
            OutputDataReceived?.Invoke(this, e);

            switch (_State)
            {
                case State.Initializing:
                    TryParsePort(data);
                    break;

                case State.Closing:
                    TryParseKey(data);
                    break;
            };
        }

        private void TryParsePort(string data)
        {
            var match = _LocalHost.Match(data);
            if (!match.Success)
                return;

            var portString = match.Groups[1].Value;
            if (!int.TryParse(portString, out var port))
                return;

            _PortFinderCompletionSource.TrySetResult(port);
        }

        private void TryParseKey(string data)
        {
            var match = _Key.Match(data);
            if (!match.Success)
                return;

            _StopKeyCompletionSource.TrySetResult(match.Groups[1].Value);
            _State = State.Running;
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            ErrorDataReceived?.Invoke(this, e);
        }

        public void Dispose()
        {
            Cancel().Wait();
        }
    }
}
