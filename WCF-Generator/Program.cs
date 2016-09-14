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
            // Set path to app.config for current application domain
            if (args != null && args.Any() && !string.IsNullOrEmpty(args[0]))
            {
                var absoluteConfigPath = Path.GetFullPath(args[0]);

                if (!File.Exists(absoluteConfigPath))
                {
                    throw new ArgumentException("File of configuration file not found");
                }
                
                AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", args[0]);
            }

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
                RunWcfGeneration();
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error occured on wcf generation. ", e);
            }
        }

        private static void RunWcfGeneration()
        {
            var solutionPath = ConfigurationManager.AppSettings["SolutionPath"];
            var absoluteSlnPath = Path.GetFullPath(solutionPath);

            if (!File.Exists(absoluteSlnPath))
            {
                throw new ArgumentException("File of solution not found");
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
            var solutionPath = ConfigurationManager.AppSettings["SolutionPath"];
            var absoluteSlnPath = Path.GetFullPath(solutionPath);

            if (!File.Exists(absoluteSlnPath))
            {
                throw new ArgumentException("File of solution not found. " + absoluteSlnPath);
            }

            var repositoryGenerator = new RepositoryCodeFactory(config, absoluteSlnPath);

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
