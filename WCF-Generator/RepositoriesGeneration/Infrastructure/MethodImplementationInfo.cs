using System.Collections.Generic;

namespace WCFGenerator.RepositoriesGeneration.Infrastructure
{
    internal class MethodImplementationInfo
    {
        public MethodImplementationInfo()
        {
            Parameters = new List<ParameterInfo>();
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
        ///     Parameters of methods
        /// </summary>
        public List<ParameterInfo> Parameters { get; set; }

        /// <summary>
        ///     Requires of Implementation 
        /// </summary>
        public bool RequiresImplementation { get; set; }

    }
}