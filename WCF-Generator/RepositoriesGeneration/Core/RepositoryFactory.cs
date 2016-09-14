using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VersionedRepositoryGeneration.Generator.Core;
using VersionedRepositoryGeneration.Generator.Infrastructure;
using WCFGenerator.RepositoriesGeneration.Configuration;
using WCFGenerator.RepositoriesGeneration.Services;

namespace WCFGenerator.RepositoriesGeneration.Core
{

    /// <summary>
    ///     Repository code factory.
    /// </summary>
    /// <remarks> Supported version. </remarks>
    internal class RepositoryCodeFactory
    {
        #region Fields

        private IEnumerable<RepositoryProject> _configurations;
        private readonly string _slnPath;

        #endregion

        #region Constructor

        public RepositoryCodeFactory(IEnumerable<RepositoryProject> configs, string slnPath)
        {
            _configurations = configs;
            _slnPath = slnPath;
        }

        #endregion

        /// <summary>
        ///     Scan configured project for search repository model and generate code of repository class
        /// </summary>
        public async Task GenerateRepository()
        {
            var repositoryGeneratorWorkSpace = new RepositoryGeneratorWorkSpace();
            // open target solution
            repositoryGeneratorWorkSpace.OpenSolution(_slnPath);

            // Generate repositories for configured project
            foreach (var config in _configurations)
            {
                // open target project
                repositoryGeneratorWorkSpace.OpenProject(config.TargetProjectName);

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
                await repositoryGeneratorWorkSpace.ApplyChanges();
            }
        }
    }
}