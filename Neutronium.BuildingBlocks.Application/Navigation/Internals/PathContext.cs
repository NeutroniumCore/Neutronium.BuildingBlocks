using System;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.BuildingBlocks.Application.Navigation.Internals
{
    internal class PathContext
    {
        private readonly string[] _Paths;
        private int _Index;

        internal string CompletePath { get; }
        internal string CurrentRelativePath => (_Index != _Paths.Length) ? _Paths[_Index] : null;
        internal string RootToCurrent => _Index == 0 ? null : BuilPath(_Paths.Where((p, i) => i < _Index));
        internal string CurrentToEnd => _Index == _Paths.Length ? null : BuilPath(_Paths.Where((p, i) => i >= _Index));

        public PathContext(string path)
        {
            CompletePath = path;
            _Paths = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            _Index = _Paths.Length;
        }

        private static string BuilPath(IEnumerable<string> paths) => String.Join("/", paths);

        public bool Back()
        {
            if (_Index == 1)
                return false;

            _Index--;
            return true;
        }

        public bool Next()
        {
            if (_Index == _Paths.Length)
                return false;

            _Index++;
            return true;
        }

        public override string ToString()
        {
            return $"{RootToCurrent} - {CurrentToEnd}";
        }
    }
}
