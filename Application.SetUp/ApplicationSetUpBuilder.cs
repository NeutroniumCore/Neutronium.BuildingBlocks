using System;
using System.Collections.Generic;

namespace Application.SetUp
{
    public class ApplicationSetUpBuilder
    {
        private const string Mode = "mode";
        private const string Live = "live";
        private const string Dev = "dev";
        private const string Prod = "prod";
        private const string Port = "port";
        private const string Url = "url";

        private readonly Uri _ProductionUri;
        private readonly ApplicationMode _Default;
        private readonly int _DefaultPort;

        private static readonly Dictionary<string, ApplicationMode> _Modes = new Dictionary<string, ApplicationMode>
        {
            [Live] = ApplicationMode.Live,
            [Dev] = ApplicationMode.Dev,
            [Prod] = ApplicationMode.Production
        };

        public ApplicationSetUpBuilder(Uri productionUri, ApplicationMode @default = ApplicationMode.Dev, int defaultPort = 8080)
        {
            _ProductionUri = productionUri;
            _DefaultPort = defaultPort;
            _Default = @default;
        }

        public ApplicationSetUp BuildForProduction()
        {
            return new ApplicationSetUp(ApplicationMode.Production, _ProductionUri);
        }

        public ApplicationSetUp BuildFromApplicationArguments(string[] arguments)
        {
            var argument = ArgumentParser.Parse(arguments);
            return BuildFromArgument(argument);
        }

        private ApplicationSetUp BuildFromArgument(IDictionary<string, string> argumentsDictionary)
        {
            var mode = GetApplicationMode(argumentsDictionary);
            var uri = BuildDevUri(mode, argumentsDictionary);
            return new ApplicationSetUp(mode, uri);
        }

        private ApplicationMode GetApplicationMode(IDictionary<string, string> argumentsDictionary)
        {
            if (argumentsDictionary.TryGetValue(Mode, out var explicitMode) &&
                _Modes.TryGetValue(explicitMode, out var mode))
                return mode;

            return _Default;
        }

        private Uri BuildDevUri(ApplicationMode mode, IDictionary<string, string> argumentsDictionary)
        {
            if (argumentsDictionary.TryGetValue(Url, out var uri))
                return new Uri(uri);

            if (mode != ApplicationMode.Live)
                return _ProductionUri;

            var port = _DefaultPort;
            if (argumentsDictionary.TryGetValue(Port, out var portString))
                int.TryParse(portString, out port);

            return new Uri($"http://localhost:{port}/index.html");
        }
    }
}
