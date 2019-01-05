using System;
using System.Reflection;

namespace Neutronium.BuildingBlocks.Application.ViewModel 
{
    public class ApplicationInformation
    {
        public ApplicationInformation(string name, string madeBy) {
            Name = name;
            MadeBy = madeBy;
            Year = DateTime.Now.Year;
            Version = Assembly.GetCallingAssembly().GetName().Version.ToString();
        }

        public string Name { get; }

        public string Version { get; }

        public string MadeBy { get; }

        public int Year { get; }
    }
}
