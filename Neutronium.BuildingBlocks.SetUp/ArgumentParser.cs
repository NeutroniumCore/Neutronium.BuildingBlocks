using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Neutronium.BuildingBlocks.SetUp
{
    /// <summary>
    /// Command line argument parser
    /// </summary>
    public static class ArgumentParser
    {
        private static readonly Regex _Switch = new Regex(@"^-(-(?<option>\w{2,})|(?<option>\w))(?:=(?<value>.*))?$", RegexOptions.Compiled);

        /// <summary>
        /// Parse command line argument into a switch-value dictionary
        /// </summary>
        /// <param name="arguments">
        /// Command line arguments to be parsed
        /// </param>
        /// <param name="onUnexpected">
        /// Callback called on parsing error with unexpected string
        /// </param>
        /// <returns>
        /// Parsed command line arguments: { [switch] = value}
        /// </returns>
        public static Dictionary<string, string> Parse(IEnumerable<string> arguments, Action<string> onUnexpected = null)
        {
            var arg = new Dictionary<string, string>();
            if (arguments == null)
                return arg;

            foreach (var rawArgument in arguments)
            {
                var argument = rawArgument.ToLower();
                var match = _Switch.Match(argument);
                if (!match.Success)
                {
                    onUnexpected?.Invoke(rawArgument);
                    continue;
                }

                var groups = match.Groups;
                var option = groups["option"].Value;
                var groupValue = groups["value"];
                arg[option] = groupValue.Success ? groupValue.Value : "true";
            }
            return arg;
        }
    }
}
