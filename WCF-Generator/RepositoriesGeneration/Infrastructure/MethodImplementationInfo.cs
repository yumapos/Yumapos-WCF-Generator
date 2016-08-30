using System;
using System.Collections.Generic;
using System.Linq;
using WCFGenerator.RepositoriesGeneration.Infrastructure;

namespace VersionedRepositoryGeneration.Generator.Infrastructure
{
    internal class MethodImplementationInfo
    {
        public MethodImplementationInfo()
        {
            Parameters = new List<ParameterInfo>();
        }

        /// <summary>
        ///    Allowable type of method
        /// </summary>
        public RepositoryMethod Method { get; set; }
        
        /// <summary>
        ///     Name of primary or filter key (for key based method only)
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Parameters of methods
        /// </summary>
        public List<ParameterInfo> Parameters { get; set; }

        /// <summary>
        ///     Requires of Implementation 
        /// </summary>
        public bool RequiresImplementation { get; set; }

    }
}