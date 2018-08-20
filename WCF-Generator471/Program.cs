using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCF_Generator471.Common;

namespace WCF_Generator471
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
            Console.Write("Open solulion: ");
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
                throw new ApplicationException($"Error occured on repository generation: {e.Message}", e);
            }

            private static void RunRepositoryGeneration()
            {
                if (!RepositoryGeneratorSettings.Current.Enabled)
                {
                    Console.WriteLine("Repository generation disabled.");
                    return;
                }

                Console.WriteLine("Start repository generation...");

                // Configure generator 
                var config = RepositoryGeneratorSettings.Current.GetConfigs();

                var repositoryGenerator = new RepositoryCodeFactory(config, _generatorWorkspace);

                // run generation
                AsyncContext.Run(repositoryGenerator.GenerateRepository);

                Console.WriteLine("Repository generation completed.");
            }
        }
    }
}
