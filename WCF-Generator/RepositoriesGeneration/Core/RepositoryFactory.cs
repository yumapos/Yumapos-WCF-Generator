using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VersionedRepositoryGeneration.Generator.Analysis;
using VersionedRepositoryGeneration.Generator.Infrastructure;
using VersionedRepositoryGeneration.Generator.Services;
using WCFGenerator.RepositoriesGeneration.Configuration;
using WCFGenerator.RepositoriesGeneration.Services;

namespace VersionedRepositoryGeneration.Generator.Core
{

    /// <summary>
    ///     Repository code factory.
    /// </summary>
    /// <remarks> Supported version. </remarks>
    internal class RepositoryCodeFactory
    {
        #region Fields

        private IEnumerable<RepositoryProject> _configurations;

        #endregion

        #region Constructor

        public RepositoryCodeFactory(IEnumerable<RepositoryProject> configs)
        {
            _configurations = configs;
        }

        #endregion

        /// <summary>
        ///     Scan configured project for search repository model and generate code of repository class
        /// </summary>
        public void GenerateRepository()
        {
            foreach (var config in _configurations.Where(c=>c.Enable))
            {
                var repositoryGeneratorWorkSpace = new RepositoryGeneratorWorkSpace(config.SolutionPath, config.TargetProjectName);

                var repositoryService = new RepositoryService(repositoryGeneratorWorkSpace, config);

                // Create list of repository for generate
                IEnumerable<ICodeClassGeneratorRepository> candidatesRepository;

                try
                {
                    candidatesRepository = repositoryService.GetRepositories();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return;
                }

                foreach (var repository in candidatesRepository)
                {
                    // Get code of repository class
                    var code = repository.GetFullCode();

                    // Add document to creation
                    repositoryGeneratorWorkSpace.AddFileToCreation(repository.RepositoryName + ".g.cs", config.RepositoryTargetFolder, code);
                }

                // Save all files
                repositoryGeneratorWorkSpace.ApplyChanges();
            }
        }
    }
}