using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCFGenerator.Common;
using WCFGenerator.RepositoriesGeneration.Configuration;
using WCFGenerator.RepositoriesGeneration.Infrastructure;
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
        private GeneratorWorkspace _generatorWorkspace;

        #endregion

        #region Constructor

        public RepositoryCodeFactory(IEnumerable<RepositoryProject> configs, GeneratorWorkspace ws)
        {
            _configurations = configs;
            _generatorWorkspace = ws;
        }

        #endregion

        /// <summary>
        ///     Scan configured project for search repository model and generate code of repository class
        /// </summary>
        public async Task GenerateRepository()
        {
            // Generate repositories for configured project
            foreach (var config in _configurations)
            {
                // open target project
                _generatorWorkspace.SetTargetProject(config.TargetProjectName);

                var repositoryService = new RepositoryService(_generatorWorkspace, config);

                // Create list of repository for generate
                IEnumerable<ICodeClassGeneratorRepository> candidatesRepository;

                try
                {
                    candidatesRepository = repositoryService.GetRepositories(config);
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
                    _generatorWorkspace.UpdateFileInTargetProject(repository.FileName, config.RepositoryTargetFolder, code);
                }

                // Save all files
                await _generatorWorkspace.ApplyTargetProjectChanges(true);
            }
        }
    }
}