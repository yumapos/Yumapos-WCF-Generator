using System;
using System.Collections.Generic;

namespace WCFGenerator.RepositoriesGeneration.Infrastructure
{
    internal class MethodImplementationInfo : MethodInfo
    {
        public MethodImplementationInfo()
        {
            RequiresImplementation = false;
        }

        /// <summary>
        ///    Allowable type of method
        /// </summary>
        public RepositoryMethod Method { get; set; }

        /// <summary>
        ///     Key based methods filter
        /// </summary>
        public FilterInfo FilterInfo { get; set; }

        /// <summary>
        ///     Requires of Implementation 
        /// </summary>
        public bool RequiresImplementation { get; set; }
        public bool RequiresImplementationAsync { get; set; }
    }

    internal class MethodInfo
    {
        /// <summary>
        ///     Name of method
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Type of returned value
        /// </summary>
        public string ReturnType { get; set; }

        public IEnumerable<ParameterInfo> Parameters { get; set; }
    }
}