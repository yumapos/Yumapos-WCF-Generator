using System.Collections.Generic;

namespace VersionedRepositoryGeneration.Generator.Core
{
    internal class GeneratorConfiguration
    {
        public GeneratorConfiguration()
        {
            RepositoryClassProjects = new List<string>();
        }

        public string ProjectName { get; set; }
        public string SolutionPath { get; set; }
        public string RepositoryMainPlace { get; set; }
        public string RepositorySuffix { get; set; }
        public string RepositoryTargetFolder { get; set; }


        public string RepositoryInterfacesProjectName { get; set; }

        /// <summary>
        ///     List of project name which include repositories models
        /// </summary>
        public List<string> RepositoryClassProjects { get; }
    }
}
 