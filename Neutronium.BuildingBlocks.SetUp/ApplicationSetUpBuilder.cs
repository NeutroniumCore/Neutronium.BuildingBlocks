using Neutronium.BuildingBlocks.SetUp.NpmHelper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Neutronium.BuildingBlocks.SetUp
{
    /// <summary>
    /// <see cref="ApplicationSetUp"/> builder
    /// </summary>
    public class ApplicationSetUpBuilder : IDisposable
    {
        private const string Live = "live";
        private const string Dev = "dev";
        private const string Prod = "prod";

        public Uri Uri { get; set; }

        private readonly ApplicationMode _Default;
        private readonly INpmLiveRunner _NpmLiveRunner;
        private readonly INoResultNpmRunner _NpmBuildRunner;


        /// <summary>
        /// Send when npm runner receives log message
        /// </summary>
        public event EventHandler<MessageEventArgs> OnRunnerMessageReceived
        {
            add
            {
                _NpmLiveRunner.OnMessageReceived += value;
                _NpmBuildRunner.OnMessageReceived += value;
            }
            remove
            {
                _NpmLiveRunner.OnMessageReceived -= value;
                _NpmBuildRunner.OnMessageReceived -= value;
            }
        }

        /// <summary>
        /// Sent on command line arguments parsing error
        /// </summary>
        public event EventHandler<MessageEventArgs> OnArgumentParsingError;
  
        private static readonly Dictionary<string, ApplicationMode> _Modes = new Dictionary<string, ApplicationMode>
        {
            [Live] = ApplicationMode.Live,
            [Dev] = ApplicationMode.Dev,
            [Prod] = ApplicationMode.Production
        };

        private enum Option
        {
            Mode,
            Url,
        }

        private static readonly Dictionary<Option, Tuple<string, string>> _Options = new Dictionary<Option, Tuple<string, string>>
        {
            [Option.Mode] = Tuple.Create("mode", "m"),
            [Option.Url] = Tuple.Create("url", "u"),
        };

        /// <summary>
        /// Construct a ApplicationSetUpBuilder with given view directory and default mode
        /// </summary>
        /// <param name="viewDirectory"></param>
        /// <param name="default"></param>
        /// <param name="liveScript">name of the npm live script</param>
        /// <param name="runScript">name of the npm build cached script</param>
        public ApplicationSetUpBuilder(string viewDirectory = "View", ApplicationMode @default = ApplicationMode.Dev,
            string liveScript = "live", string runScript = "cached-build") :
            this(new Uri($"pack://application:,,,/{viewDirectory.Replace(@"\", "/")}/dist/index.html"),
                @default,
                new NpmLiveRunner(viewDirectory, liveScript),
                new NpmBuilder(viewDirectory, runScript))
        {
        }

        internal ApplicationSetUpBuilder(Uri productionUri, ApplicationMode @default, INpmLiveRunner npmLiveRunner,
            INoResultNpmRunner npmBuildRunner)
        {
            Uri = productionUri;
            _Default = @default;
            _NpmLiveRunner = npmLiveRunner;
            _NpmBuildRunner = npmBuildRunner;
            if (_NpmLiveRunner == null)
                throw new ArgumentNullException(nameof(npmLiveRunner));
        }


        /// <summary>
        /// Build set-up from given mode
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="onNpmLog"></param>
        /// <returns></returns>
        public async Task<ApplicationSetUp> BuildFromMode(ApplicationMode mode, CancellationToken cancellationToken, Action<string> onNpmLog = null)
        {
            var uri = await BuildUri(mode, cancellationToken, onNpmLog).ConfigureAwait(false);
            return new ApplicationSetUp(mode, uri);
        }

        /// <summary>
        /// Build set-up for production: no debug, local url
        /// </summary>
        /// <returns></returns>
        public ApplicationSetUp BuildForProduction()
        {
            return new ApplicationSetUp(ApplicationMode.Production, Uri);
        }

        /// <summary>
        /// Build set-up from application command line argument
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public Task<ApplicationSetUp> BuildFromApplicationArguments(string[] arguments)
        {
            var argument = ArgumentParser.Parse(arguments, FireError);
            return BuildFromArgument(argument);
        }

        private void FireError(string error)
        {
            OnArgumentParsingError?.Invoke(this, new MessageEventArgs(error, true));
        }

        private async Task<ApplicationSetUp> BuildFromArgument(IDictionary<string, string> argumentsDictionary)
        {
            var mode = GetApplicationMode(argumentsDictionary);
            var uri = await BuildDevUri(mode, argumentsDictionary).ConfigureAwait(false);
            return new ApplicationSetUp(mode, uri);
        }

        private ApplicationMode GetApplicationMode(IDictionary<string, string> argumentsDictionary)
        {
            if (TryGetValue(argumentsDictionary, Option.Mode, out var explicitMode) &&
                _Modes.TryGetValue(explicitMode, out var mode))
                return mode;

            return _Default;
        }

        private static bool TryGetValue(IDictionary<string, string> argumentsDictionary, Option option, out string explicitMode)
        {
            var (fullName, shortName) = _Options[option];
            return (argumentsDictionary.TryGetValue(fullName, out explicitMode) ||
                argumentsDictionary.TryGetValue(shortName, out explicitMode));
        }

        private async Task<Uri> BuildDevUri(ApplicationMode mode, IDictionary<string, string> argumentsDictionary)
        {
            if (TryGetValue(argumentsDictionary, Option.Url, out var uri))
                return new Uri(uri);

            return await BuildUri(mode, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task<Uri> BuildUri(ApplicationMode mode, CancellationToken cancellationToken, Action<string> onNpmLog = null)
        {
            if (mode == ApplicationMode.Production)
                return Uri;

            void OnDataReceived(object _, MessageEventArgs dataReceived)
            {
                onNpmLog?.Invoke(dataReceived.Message);
            }

            using (GetMessageLogger(OnDataReceived))
            {
                return await RunScriptAndReturnUri(mode, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<Uri> RunScriptAndReturnUri(ApplicationMode mode, CancellationToken cancellationToken)
        {
            switch (mode)
            {
                case ApplicationMode.Live:
                    var port = await _NpmLiveRunner.GetPortAsync(cancellationToken).ConfigureAwait(false);
                    return new Uri($"http://localhost:{port}/index.html");

                case ApplicationMode.Dev:
                    await _NpmBuildRunner.Run(cancellationToken).ConfigureAwait(false);
                    return Uri;

                default:
                    throw new NotImplementedException();
            }
        }

        private IDisposable GetMessageLogger(EventHandler<MessageEventArgs> logger)
        {
            return new MessageLogger(this, logger);
        }

        private class MessageLogger : IDisposable
        {
            private readonly EventHandler<MessageEventArgs> _Logger;
            private readonly ApplicationSetUpBuilder _ApplicationSetUpBuilder;

            public MessageLogger(ApplicationSetUpBuilder applicationSetUpBuilder, EventHandler<MessageEventArgs> logger)
            {
                _Logger = logger;
                _ApplicationSetUpBuilder = applicationSetUpBuilder;
                _ApplicationSetUpBuilder.OnRunnerMessageReceived += _Logger;
            }

            public void Dispose()
            {
                _ApplicationSetUpBuilder.OnRunnerMessageReceived -= _Logger;
            }
        }

        public void Dispose()
        {
            _NpmLiveRunner.Dispose();
            _NpmBuildRunner?.Dispose();
        }
    }
}
