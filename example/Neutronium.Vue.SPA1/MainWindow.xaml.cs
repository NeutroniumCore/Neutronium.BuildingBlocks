using System;
using System.Diagnostics;
using Neutronium.BuildingBlocks.Application.Navigation;
using Neutronium.BuildingBlocks.Application.ViewModels;
using Neutronium.BuildingBlocks.SetUp;

namespace Neutronium.Vue.SPA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public SetUpViewModel SetUp => App.SetUp;

        private ApplicationViewModelBuilder _ApplicationViewModelBuilder;

        public MainWindow()
        {
            this.Initialized += MainWindow_Initialized;
            InitializeComponent();
        }

        private void MainWindow_Initialized(object sender, EventArgs e)
        {
            DataContext = BuildApplicationViewModel(); ;
            Initialized -= MainWindow_Initialized;
        }

        private ApplicationViewModel<ApplicationInformation> BuildApplicationViewModel()
        {
            _ApplicationViewModelBuilder = new ApplicationViewModelBuilder(this);
            var mainViewModel = _ApplicationViewModelBuilder.ApplicationViewModel;
#if DEBUG
            mainViewModel.Router.OnRoutingMessage += Router_OnRoutingMessage;
#endif
            return mainViewModel;
        }

#if DEBUG
        private void Router_OnRoutingMessage(object sender, RoutingMessageArgs e)
        {
            if (e.Type == MessageType.Error)
            {
                Trace.TraceError(e.Message);
                return;
            }

            Trace.TraceInformation(e.Message);
        }
#endif

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.HtmlView.Dispose();
        }
    }
}
