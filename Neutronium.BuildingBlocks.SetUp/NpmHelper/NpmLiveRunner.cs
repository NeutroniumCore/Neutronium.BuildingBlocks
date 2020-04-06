using Neutronium.BuildingBlocks.SetUp.Utils;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Neutronium.BuildingBlocks.SetUp.NpmHelper
{
    /// <summary>
    /// Npm runner implementation
    /// </summary>
    public class NpmLiveRunner : INpmLiveRunner
    {
        private static readonly Regex _LocalHost = new Regex(@"http:\/\/localhost:(\d{1,4})", RegexOptions.Compiled);

        private readonly TaskCompletionSource<int> _PortFinderCompletionSource = new TaskCompletionSource<int>();
        private readonly GenericNpmRunner _GenericNpmRunner;
        public event EventHandler<MessageEventArgs> OnMessageReceived;

        public NpmLiveRunner(string directory, string script)
        {
            _GenericNpmRunner = new GenericNpmRunner(directory, script);
            _GenericNpmRunner.OnMessageReceived += GenericNpmRunnerOnMessageReceived;
        }

        public Task<int> GetPortAsync(CancellationToken cancellationToken)
        {
            _GenericNpmRunner.Run(cancellationToken);
            return _PortFinderCompletionSource.Task.WithCancellation(cancellationToken, false);
        }

        public Task<bool> Cancel()
        {
            return _GenericNpmRunner.Cancel();
        }

        private void GenericNpmRunnerOnMessageReceived(object sender, MessageEventArgs e)
        {
            if (!e.Error)
            {
                TryParsePort(e.Message);
            }
            OnMessageReceived?.Invoke(this, e);
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

        public void Dispose()
        {
            Cancel().Wait();
        }

        public override string ToString() => _GenericNpmRunner.ToString();
    }
}
