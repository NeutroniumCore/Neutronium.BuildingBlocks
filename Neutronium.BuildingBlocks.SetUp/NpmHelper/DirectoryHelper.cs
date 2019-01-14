using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Neutronium.BuildingBlocks.SetUp.NpmHelper
{
    /// <summary>
    /// Directory utility
    /// </summary>
    public static class DirectoryHelper
    {
        private static readonly Regex _Path = new Regex(@"\\bin(\\x86|\\x64)?\\(Debug|Release)$", RegexOptions.Compiled);

        /// <summary>
        /// Get the current code directory
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDirectory()
        {
            return _Path.Replace(Directory.GetCurrentDirectory(), String.Empty);
        }
    }
}
