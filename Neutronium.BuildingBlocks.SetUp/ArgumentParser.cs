using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Neutronium.BuildingBlocks.SetUp
{
    public static class ArgumentParser
    {
        private static readonly Regex _Switch = new Regex("^-", RegexOptions.Compiled);
        private static readonly Regex _SwitchWithValue = new Regex("^-(\\w+)=(.*)$", RegexOptions.Compiled);

        public static Dictionary<string, string> Parse(IEnumerable<string> arguments)
        {
            var arg = new Dictionary<string, string>();
            if (arguments == null)
                return arg;

            foreach (var rawArgument in arguments)
            {
                var argument = rawArgument.ToLower();
                var match = _SwitchWithValue.Match(argument);
                var switchValue = default(string);
                if (!match.Success)
                {
                    switchValue = _Switch.Replace(argument, String.Empty);
                    arg[switchValue] = "true";
                    continue;
                }

                switchValue = match.Groups[1].Value;
                arg[switchValue] = match.Groups[2].Value; ;
            }
            return arg;
        }
    }
}
