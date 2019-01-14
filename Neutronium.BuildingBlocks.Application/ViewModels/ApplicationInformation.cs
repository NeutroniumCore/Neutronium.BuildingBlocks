using System;
using System.Reflection;

namespace Neutronium.BuildingBlocks.Application.ViewModels
{
    /// <summary>
    /// Application MetaData 
    /// </summary>
    public class ApplicationInformation
    {
        public ApplicationInformation(string name, string madeBy)
        {
            Name = name;
            MadeBy = madeBy;
            Year = DateTime.Now.Year;
            Version = Assembly.GetCallingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// Application name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Application version
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Application author
        /// </summary>
        public string MadeBy { get; }

        /// <summary>
        /// Copyright year
        /// </summary>
        public int Year { get; }
    }
}
