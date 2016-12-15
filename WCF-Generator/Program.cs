using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Nito.AsyncEx;
using WCFGenerator.Common;
using WCFGenerator.DecoratorGeneration.Configuration;
using WCFGenerator.DecoratorGeneration.Core;
using WCFGenerator.RepositoriesGeneration.Configuration;
using WCFGenerator.RepositoriesGeneration.Core;
using WCFGenerator.SerializeGeneration.Generation;
using WCFGenerator.WcfClientGeneration;
using WCFGenerator.WcfClientGeneration.Configuration;

namespace WCFGenerator
{
    internal class Program
    {
        private static GeneratorWorkspace _generatorWorkspace;

        static void Main(string[] args)
        {
            // Set path to app.config for current application domain
            if (args != null && args.Any() && !string.IsNullOrEmpty(args[0]))
            {
                var absoluteConfigPath = Path.GetFullPath(args[0]);

                if (!File.Exists(absoluteConfigPath))
                {
                    throw new ArgumentException("File of configuration file not found");
                }
                
                AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", absoluteConfigPath);
            }

            // Get solution
            Console.WriteLine("Open solulion");

            var solutionPath = ConfigurationManager.AppSettings["SolutionPath"];
            var absoluteSlnPath = Path.GetFullPath(solutionPath);

            if (!File.Exists(absoluteSlnPath))
            {
                throw new ArgumentException("File of solution not found");
            }

            // Create workspace - open solution
            _generatorWorkspace = new GeneratorWorkspace(absoluteSlnPath);

            Console.WriteLine("Solulion opened");

            try
            {
                RunRepositoryGeneration();
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error occured on repository generation", e);
            }

            try
            {
               RunSerializeGeneration();
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error occured on serialize generation", e);
            }

            try
            {
                RunWcfGeneration();
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error occured on wcf generation.", e);
            }

            try
            {
                RunDecoratorGeneration();
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error occured on decorator generation.", e);
            }

            // Apply Changes, close solution
            _generatorWorkspace.CloseSolution();
        }

        private static void RunWcfGeneration()
        {
            Console.WriteLine("Start Wcf client generation...");

            // Configure generator 
            var configs = WcfServiceClientGeneratorSettings.GetConfigs();

            var wcf = new WcfGenerator(_generatorWorkspace)
            {
                // TODO сделать возможность генерить клиенты в разные проекты
                // Перенести эти настройки в ServiceDetail
                ProjectName = configs.First().TargetProjectName, 
                FaultProject = configs.First().FaultNamespace,
                ProjectApi = configs.First().ApiInterfaceProjectName,
                ProjectApiFolders = new List<string>()
                    {
                        configs.First().ApiInterfaceProjectFolder
                    },

                Services = configs.Select(c => new ServiceDetail()
                {
                    UserName = c.ClientInterfaceName,
                    FileName = c.ClientInterfaceFileName,
                }).ToList()
            };

            AsyncContext.Run(() => wcf.Start());

            Console.WriteLine("Wcf client generation completed.");
        }

        private static void RunRepositoryGeneration()
        {
            Console.WriteLine("Start repository generation...");

            // Configure generator 
            var config = RepositoryGeneratorSettings.GetConfigs();

            var repositoryGenerator = new RepositoryCodeFactory(config, _generatorWorkspace);

            // run generation
            AsyncContext.Run(() => repositoryGenerator.GenerateRepository());

            Console.WriteLine("Repository generation completed.");
        }

        private static void RunSerializeGeneration()
        {
            Console.WriteLine("Start serialize generation...");

            var repositoryGenerator = new SerilizationGeneration(_generatorWorkspace);

            // run generation
            AsyncContext.Run(() => repositoryGenerator.GenerateAll());

            Console.WriteLine("Serialize generation completed.");
        }

        private static void RunDecoratorGeneration()
        {
            Console.WriteLine("Start decoration generation...");

            // Configure generator 
            var config = DecoratorGeneratorSettings.GetConfigs();

            var generator = new DecoratorCodeFactory(config, _generatorWorkspace);

            // run generation
            AsyncContext.Run(() => generator.Generate());

            Console.WriteLine("Decoration generation completed.");
        }
    }
}
