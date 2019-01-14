using Neutronium.Core.Infra;
using Neutronium.Core.Navigation;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Neutronium.BuildingBlocks.SetUp
{
    /// <summary>
    /// Set-up viewModel
    /// </summary>
    public class SetUpViewModel
    {
        /// <summary>
        /// View uri
        /// </summary>
        public Uri Uri => _ApplicationSetUp.Uri;

        /// <summary>
        /// True if debug mode
        /// </summary>
        public bool Debug => _ApplicationSetUp.Debug;

        /// <summary>
        /// Application mode
        /// </summary>
        public ApplicationMode Mode => _ApplicationSetUp.Mode;

        /// <summary>
        /// Commands to be injected to the Neutronium components
        /// Including "To Live" and "Reload" commands
        /// </summary>
        public IDictionary<string, ICommand<ICompleteWebViewComponent>> DebugCommands { get; } = new Dictionary<string, ICommand<ICompleteWebViewComponent>>();

        private readonly ApplicationSetUpBuilder _Builder;
        private ApplicationSetUp _ApplicationSetUp;

        public SetUpViewModel(ApplicationSetUpBuilder builder)
        {
            _Builder = builder;
        }

        private async void GoLive(IWebViewComponent viewControl)
        {
            if (Mode != ApplicationMode.Dev)
                return;

            var resourceLoader = GetResourceReader();
            var createOverlay = resourceLoader.Load("loading.js");
            viewControl.ExecuteJavascript(createOverlay);

            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            DebugCommands.Clear();
            DebugCommands["Cancel to live"] = new RelayToogleCommand<ICompleteWebViewComponent>
            (_ =>
            {
                cancellationTokenSource.Cancel();
                UpdateCommands();
            });

            try
            {
                await Task.Run(() => DoGoLive(viewControl, token), token);
            }
            catch (TaskCanceledException)
            {
                var removeOverlay = resourceLoader.Load("removeOverlay.js");
                viewControl.ExecuteJavascript(removeOverlay);
                return;
            }

            await viewControl.SwitchViewAsync(Uri);
        }

        private async Task DoGoLive(IWebViewComponent viewControl, CancellationToken token)
        {
            var resourceLoader = GetResourceReader();
            var updateOverlay = resourceLoader.Load("update.js");
            void OnNpmLog(string information)
            {
                if (information == null)
                    return;

                var text = JavascriptNamer.GetCreateExpression(information);
                var code = updateOverlay.Replace("{information}", text);
                viewControl.ExecuteJavascript(code);
            }

            UpdateSetUp(await _Builder.BuildFromMode(ApplicationMode.Live, token, OnNpmLog));
        }

        private ResourceReader GetResourceReader() => new ResourceReader("script", this);

        public async Task InitFromArgs(string[] args)
        {
            var setup = await _Builder.BuildFromApplicationArguments(args).ConfigureAwait(false);
            UpdateSetUp(setup);
        }

        public void InitForProduction()
        {
            UpdateSetUp(_Builder.BuildForProduction());
        }

        private void UpdateSetUp(ApplicationSetUp applicationSetUp)
        {
            _ApplicationSetUp = applicationSetUp;
            UpdateCommands();
        }

        private void UpdateCommands()
        {
            DebugCommands.Clear();
            switch (Mode)
            {
                case ApplicationMode.Live:
                    DebugCommands["Reload"] = new RelayToogleCommand<ICompleteWebViewComponent>(htmlView => htmlView.ReloadAsync());
                    break;

                case ApplicationMode.Dev:
                    DebugCommands["To Live"] = new RelayToogleCommand<ICompleteWebViewComponent>(GoLive);
                    break;
            }
        }

        public override string ToString()
        {
            return _ApplicationSetUp?.ToString() ?? "Not initialized";
        }
    }
}
