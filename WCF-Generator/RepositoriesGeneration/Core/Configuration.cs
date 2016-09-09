using System.Collections.Generic;

namespace VersionedRepositoryGeneration.Generator.Core
{
    internal class GeneratorConfiguration
    {
        public GeneratorConfiguration()
        {
            AdditionalProjects = new List<string>();
        }

        /// <summary>
        ///     Absolute path of solution
        /// </summary>
        public string SolutionPath { get; set; }

        /// <summary>
        ///     Name of target project (for generated files)
        /// </summary>
        public string TargetProjectName { get; set; }

        /// <summary>
        ///     Name of target folder in project
        /// </summary>
        public string RepositoryTargetFolder { get; set; }

        /// <summary>
        ///     Standard suffix repository
        /// </summary>
        public string RepositorySuffix { get; set; }

        /// <summary>
        ///     Name of project which contains interfaces
        /// </summary>
        public string RepositoryInterfacesProjectName { get; set; }

        /// <summary>
        ///     Project name which include repository models
        /// </summary>
        public string RepositoryClassProjects { get; private set; }

        /// <summary>
        ///     List of project for analysis
        /// </summary>
        public List<string> AdditionalProjects { get; private set; }
    }
}
 