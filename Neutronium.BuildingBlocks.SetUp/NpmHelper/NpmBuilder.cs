using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neutronium.BuildingBlocks.SetUp.NpmHelper
{
    public class NpmBuilder : INoResultNpmRunner
    {
        private readonly string _Directory;
        private readonly string _Script;
        public event EventHandler<MessageEventArgs> OnMessageReceived;
        private CancellationTokenSource _CancellationTokenSource;

        public NpmBuilder(string directory, string script)
        {
            _Directory = directory;
            _Script = script;
        }

        public void Dispose()
        {
            _CancellationTokenSource?.Cancel();
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            var builder = new GenericNpmRunner(_Directory, _Script);
            builder.OnMessageReceived += Builder_OnMessageReceived;

            _CancellationTokenSource?.Cancel();
            _CancellationTokenSource = new CancellationTokenSource();
            var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(_CancellationTokenSource.Token, cancellationToken);
            
            try
            {
                await builder.Run(linkedToken.Token).ConfigureAwait(false);
            }
            finally
            {
                builder.OnMessageReceived -= Builder_OnMessageReceived;
                builder.Dispose();
            }
        }

        private void Builder_OnMessageReceived(object sender, MessageEventArgs e)
        {
            OnMessageReceived?.Invoke(this, e);
        }
    }
}
