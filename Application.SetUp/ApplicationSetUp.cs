using System;

namespace Application.SetUp
{
    public class ApplicationSetUp
    {
        public bool Debug => Mode != ApplicationMode.Production;
        public ApplicationMode Mode { get; }
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
