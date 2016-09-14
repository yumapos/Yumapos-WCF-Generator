using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Nito.AsyncEx;
using WCFGenerator.RepositoriesGeneration.Configuration;
using WCFGenerator.RepositoriesGeneration.Core;

namespace WCFGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Arguments should be contains path of app.config file
            if (args == null || !args.Any() || args[0] == null)
            {
                Console.WriteLine("Configuration file wasn't specified");
                return;
            }

            // Set path to app.config for current application domain
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", args[0]);

            try
            {
                RunRepositoryGeneration();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured on repository generation. Exception:\n" + e);
            }

            try
            {
                RunWcfGeneration();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured on wcf generation. Exception:\n" + e);
            }
        }

        private static void RunWcfGeneration()
        {
            var solutionPath = ConfigurationManager.AppSettings["SolutionPath"];
            var absoluteSlnPath = Path.GetFullPath(solutionPath);

            if (!File.Exists(absoluteSlnPath))
            {
                Console.WriteLine("File of solution not found");
                return;
            }

            WCFGenerator wcf = new WCFGenerator
            {
                SolutionPath = absoluteSlnPath,
                ProjectName = "YumaPos.Client.WCF",
                ProjectFolders = new List<string>(),
                FaultProject = "YumaPos.Shared.API.Faults",
                ProjectApi = "YumaPos.Shared.Infrastructure",
                ProjectApiFolders = new List<string>()
                    {
                        "API"
                    },
                Services = new List<ServiceDetail>
                    {
                        new ServiceDetail()
                        {
                            UserName = "IBackOffice",
                            FileName = "IBackOfficeService.cs"
                        },
                        new ServiceDetail()
                        {
                            UserName = "ITerminal",
                            FileName = "IService.cs"
                        },
                        new ServiceDetail()
                        {
                            UserName = "IOnlineService",
                            FileName = "IOnlineService.cs"
                        }
                    }
            };

            AsyncContext.Run(() => wcf.Start(new []{solutionPath}));
        }

        private static void RunRepositoryGeneration()
        {
            Console.WriteLine("Start repository generation...");

            // Configure generator 
            var config = RepositoryGeneratorSettings.GetConfigs();
            var slnP = RepositoryGeneratorSettings.GetSolutionPath();

            var repositoryGenerator = new RepositoryCodeFactory(config, slnP);

            // run generation
            AsyncContext.Run(() => repositoryGenerator.GenerateRepository());

            Console.WriteLine("Repository generation complated.");
        }
    }

    public class ServiceDetail
    {
        public string UserName { get; set; }
        public string FileName { get; set; }
    }
}
