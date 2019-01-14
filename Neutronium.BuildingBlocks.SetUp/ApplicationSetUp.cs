using System;

namespace Neutronium.BuildingBlocks.SetUp
{
    /// <summary>
    /// Application set-up mode information
    /// </summary>
    public class ApplicationSetUp
    {
        /// <summary>
        /// True when Neutronium component is in debug
        /// </summary>
        public bool Debug => Mode != ApplicationMode.Production;

        /// <summary>
        /// Application mode
        /// </summary>
        public ApplicationMode Mode { get; }

        /// <summary>
        /// Application view uri
        /// </summary>
        public Uri Uri { get; }

        public ApplicationSetUp(ApplicationMode mode, Uri uri)
        {
            Mode = mode;
            Uri = uri;
        }

        public override string ToString()
        {
            return $"Mode: {Mode}, Uri: {Uri}, Debug: {Debug}";
        }
    }
}
