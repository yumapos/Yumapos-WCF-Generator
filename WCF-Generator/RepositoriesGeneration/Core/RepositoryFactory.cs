using System;
using System.Collections.Generic;
using VersionedRepositoryGeneration.Generator.Infrastructure;
using VersionedRepositoryGeneration.Generator.Services;

namespace VersionedRepositoryGeneration.Generator.Core
{

    /// <summary>
    ///     Repository code factory.
    /// </summary>
    /// <remarks> Supported version. </remarks>
    internal class RepositoryCodeFactory
    {
        #region Fields

        private readonly GeneratorConfiguration _configuration;

        private readonly RepositoryGeneratorWorkSpace _repositoryGeneratorWorkSpace;

        private readonly RepositoryService _repositoryService;

        #endregion

        #region Constructor

        public RepositoryCodeFactory(GeneratorConfiguration config)
        {
            _configuration = config;

            _repositoryGeneratorWorkSpace = new RepositoryGeneratorWorkSpace(config.SolutionPath, config.ProjectName);

            _repositoryService = new RepositoryService(_repositoryGeneratorWorkSpace, config);
        }

        #endregion

        /// <summary>
        ///     Scan configured project for search repository model and generate code of repository class
        /// </summary>
        public void GenerateRepository()
        {
            // Create list of repository for generate
            IEnumerable<ICodeClassGeneratorRepository> candidatesRepository;

            try
            {
                candidatesRepository = _repositoryService.GetRepositories();
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

                // TODO apply standart formating for code 

                // Add document to creation
                _repositoryGeneratorWorkSpace.AddFileToCreation(repository.RepositoryName + ".g.cs", _configuration.RepositoryTargetFolder, code);
            }
            
            // Save all files
            _repositoryGeneratorWorkSpace.ApplyChanges();
        }
    }
}