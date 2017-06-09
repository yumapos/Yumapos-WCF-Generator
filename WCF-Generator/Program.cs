﻿using System;
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
            Console.WriteLine("WCF-Generator.exe started.");

            // Set path to app.config for current application domain
            var absoluteConfigPath = string.Empty;
            if (args != null && args.Any() && !string.IsNullOrEmpty(args[0]))
            {
                absoluteConfigPath = Path.GetFullPath(args[0]);
                AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", absoluteConfigPath);
            }
            else
            {
                absoluteConfigPath = AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE").ToString();
            }
            if (!File.Exists(absoluteConfigPath))
            {
                throw new ArgumentException("File of configuration file not found. " + absoluteConfigPath);
            }
            Console.WriteLine("Configuration: " + absoluteConfigPath);

            // Get solution
            Console.Write("Open solulion: " );
            var solutionPath = ConfigurationManager.AppSettings["SolutionPath"];
            //Path to solution relative config file
            if (!Path.IsPathRooted(solutionPath))
            {
                var config = new FileInfo(absoluteConfigPath);
                solutionPath = Path.Combine(config.Directory.FullName, solutionPath);
            }
            var absoluteSlnPath = Path.GetFullPath(solutionPath);
            Console.WriteLine(absoluteSlnPath);
            if (!File.Exists(absoluteSlnPath))
            {
                throw new ArgumentException("File of solution not found." + absoluteSlnPath);
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

            if (configs == null)
            {
                return;
            }

            var srvs = configs.Select(c => new ServiceDetail()
            {
                ClientInterfaceName = c.ClientInterfaceName,
                FileName = c.ClientInterfaceFileName,
                ProjectName = c.TargetProjectName,
                FaultProject = c.FaultNamespace,
                ProjectApi = c.ApiInterfaceProjectName,
                ProjectApiFolders = new List<string>()
                {
                    configs.First().ApiInterfaceProjectFolder
                },
            }).ToList();

            var wcf = new WcfGenerator(_generatorWorkspace, srvs);

            AsyncContext.Run(wcf.Start);

            Console.WriteLine("Wcf client generation completed.");
        }

        private static void RunRepositoryGeneration()
        {
            Console.WriteLine("Start repository generation...");

            // Configure generator 
            var config = RepositoryGeneratorSettings.GetConfigs();

            var repositoryGenerator = new RepositoryCodeFactory(config, _generatorWorkspace);

            // run generation
            AsyncContext.Run(repositoryGenerator.GenerateRepository);

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

            try
            {
                // Configure generator 
                var config = DecoratorGeneratorSettings.GetConfigs();

                if (config == null)
                {
                    return;
                }

                var generator = new DecoratorCodeFactory(config, _generatorWorkspace);

                // run generation
                AsyncContext.Run(() => generator.Generate());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
            

            Console.WriteLine("Decoration generation completed.");
        }
    }
}
