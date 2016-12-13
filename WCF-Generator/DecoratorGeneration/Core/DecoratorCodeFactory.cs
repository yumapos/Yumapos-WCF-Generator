using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCFGenerator.Common;
using WCFGenerator.DecoratorGeneration.Configuration;
using WCFGenerator.DecoratorGeneration.Infrastructure;

namespace WCFGenerator.DecoratorGeneration.Core
{

    /// <summary>
    ///     Decorator code factory.
    /// </summary>
    /// <remarks> Supported version. </remarks>
    internal class DecoratorCodeFactory
    {
        #region Fields

        private IEnumerable<DecoratorProject> _configurations;
        private GeneratorWorkspace _generatorWorkspace;

        #endregion

        #region Constructor

        public DecoratorCodeFactory(IEnumerable<DecoratorProject> configs, GeneratorWorkspace ws)
        {
            _configurations = configs;
            _generatorWorkspace = ws;
        }

        #endregion

        /// <summary>
        ///     Scan configured project for search classes and generate code of decorator class
        /// </summary>
        public async Task Generate()
        {
            // Generate decorators for configured project
            foreach (var config in _configurations)
            {
                // Configure service
                var decoratorService = new DecoratorService(_generatorWorkspace, config.Project);

                // Set target project for saving generated classes
                _generatorWorkspace.SetTargetProject(config.Project);

                // Create list of decorators for generate
                IEnumerable<ICodeClassGeneratorDecorator> candidatesDecorator;

                try
                {
                    candidatesDecorator = decoratorService.GetDecorators();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                foreach (var decorator in candidatesDecorator)
                {
                    // Get code of decorator class
                    var code = decorator.GetFullCode();

                    // Add document to creation
                    _generatorWorkspace.UpdateFile(decorator.FileName, "", code);
                }

                // Save all files
                await _generatorWorkspace.ApplyTargetProjectChanges(true);
            }
        }
    }
}