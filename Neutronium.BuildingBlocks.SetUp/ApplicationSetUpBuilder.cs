using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Neutronium.BuildingBlocks.SetUp.NpmHelper;

namespace Neutronium.BuildingBlocks.SetUp {
    public class ApplicationSetUpBuilder : IDisposable {
        private const string Mode = "mode";
        private const string ShortMode = "m";
        private const string Live = "live";
        private const string Dev = "dev";
        private const string Prod = "prod";
        private const string Url = "url";
        private const string ShortUrl = "u";

        public Uri Uri { get; set; }

        private readonly ApplicationMode _Default;
        private readonly INpmRunner _NpmRunner;

        public event EventHandler<RunnerMessageEventArgs> OnRunnerMessageReceived {
            add => _NpmRunner.OnMessageReceived += value;
            remove => _NpmRunner.OnMessageReceived -= value;
        }

        public event EventHandler<RunnerMessageEventArgs> OnRunnerErrorReceived {
            add => _NpmRunner.OnErrorReceived += value;
            remove => _NpmRunner.OnErrorReceived -= value;
        }

        private static readonly Dictionary<string, ApplicationMode> _Modes = new Dictionary<string, ApplicationMode> {
            [Live] = ApplicationMode.Live,
            [Dev] = ApplicationMode.Dev,
            [Prod] = ApplicationMode.Production
        };

        private enum Flag {
            Mode,
            Url,
        }

        private static readonly Dictionary<Flag, Tuple<string, string>> _Flags = new Dictionary<Flag, Tuple<string, string>> {
            [Flag.Mode] = Tuple.Create("mode", "m"),
            [Flag.Url] = Tuple.Create("url", "u"),
        };

        public ApplicationSetUpBuilder(string viewDirectory = "View", ApplicationMode @default = ApplicationMode.Dev,
            string liveScript = "live") :
            this(new Uri($"pack://application:,,,/{viewDirectory.Replace(@"\", "/")}/dist/index.html"),
                @default,
                new NpmRunner(viewDirectory, liveScript)) {
        }

        internal ApplicationSetUpBuilder(Uri productionUri, ApplicationMode @default, INpmRunner npmRunner) {
            Uri = productionUri;
            _Default = @default;
            _NpmRunner = npmRunner;
            if (_NpmRunner == null)
                throw new ArgumentNullException(nameof(npmRunner));
        }

        public async Task<ApplicationSetUp> BuildFromMode(ApplicationMode mode, CancellationToken cancellationToken, Action<string> onNpmLog = null) {
            var uri = await BuildUri(mode, cancellationToken, onNpmLog).ConfigureAwait(false);
            return new ApplicationSetUp(mode, uri);
        }

        public ApplicationSetUp BuildForProduction() {
            return new ApplicationSetUp(ApplicationMode.Production, Uri);
        }

        public Task<ApplicationSetUp> BuildFromApplicationArguments(string[] arguments) {
            var argument = ArgumentParser.Parse(arguments);
            return BuildFromArgument(argument);
        }

        private async Task<ApplicationSetUp> BuildFromArgument(IDictionary<string, string> argumentsDictionary) {
            var mode = GetApplicationMode(argumentsDictionary);
            var uri = await BuildDevUri(mode, argumentsDictionary).ConfigureAwait(false);
            return new ApplicationSetUp(mode, uri);
        }

        private ApplicationMode GetApplicationMode(IDictionary<string, string> argumentsDictionary) {
            if (TryGetValue(argumentsDictionary, Flag.Mode, out var explicitMode) &&
                _Modes.TryGetValue(explicitMode, out var mode))
                return mode;

            return _Default;
        }

        private static bool TryGetValue(IDictionary<string, string> argumentsDictionary, Flag flag, out string explicitMode) {
            (var fullName, var shortName) = _Flags[flag];
            return (argumentsDictionary.TryGetValue(fullName, out explicitMode) ||
                argumentsDictionary.TryGetValue(shortName, out explicitMode));
        }

        private async Task<Uri> BuildDevUri(ApplicationMode mode, IDictionary<string, string> argumentsDictionary) {
            if (argumentsDictionary.TryGetValue(Url, out var uri))
                return new Uri(uri);

            return await BuildUri(mode, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task<Uri> BuildUri(ApplicationMode mode, CancellationToken cancellationToken, Action<string> onNpmLog = null) {
            if (mode != ApplicationMode.Live)
                return Uri;

            void OnDataReceived(object _, RunnerMessageEventArgs dataReceived) {
                onNpmLog?.Invoke(dataReceived.Message);
            }

            OnRunnerMessageReceived += OnDataReceived;
            var port = await _NpmRunner.GetPortAsync(cancellationToken).ConfigureAwait(false);
            OnRunnerMessageReceived -= OnDataReceived;
            return new Uri($"http://localhost:{port}/index.html");
        }

        public void Dispose() {
            _NpmRunner?.Dispose();
        }
    }
}
