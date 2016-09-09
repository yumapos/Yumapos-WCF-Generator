using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Nito.AsyncEx;
using VersionedRepositoryGeneration.Generator.Core;
using WCFGenerator.RepositoriesGeneration.Configuration;

namespace WCFGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || !args.Any() || args[0] == null)
            {
                Console.WriteLine("The directory of a solution wasn't specified");
                return;
            }

            var absoluteSlnPath = File.Exists(args[0]) ? args[0] : Path.GetFullPath(args[0]);

            if (!File.Exists(absoluteSlnPath))
            {
                Console.WriteLine("File of solution not found");
            }

            //MappingsGenerator mapping = new MappingsGenerator()
            //{
            //    SolutionPath = @"C:\Users\denis.ev\Documents\WCF_source\YumaPos.Server.WCF.sln"
            //};

            //var generator = new MappingsGenerator()
            //{
            //    MapExtensionNameSpace = "YumaPos.Server.BusinessLogic.Generation",
            //    MapExtensionClassName = "MapExtensions",
            //    MapAttribute = "Map",
            //    MapIgnoreAttribute = "MapIgnore",
            //    DtoSuffix = "Dto",
            //    DoProjects = new List<string>
            //    {
            //        "YumaPos.Server.Infrastructure",
            //        "YumaPos.FrontEnd.Infrastructure"
            //    },
            //    DtoProjects = new List<string>
            //    {
            //        "YumaPos.Shared.Infrastructure"
            //    },
            //    DOSkipAttribute = false,
            //    DTOSkipAttribute = false
            //};

            //mapping.GenerateMap(generator);

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
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured on wcf generation. Exception:\n" + e);
            }
        }

        private static void RunWcfGeneration(string[] args)
        {
            var absoluteSlnPath = Path.GetFullPath(args[0]);

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

            AsyncContext.Run(() => wcf.Start(args));
        }

        private static void RunRepositoryGeneration()
        {
            Console.WriteLine("Start repository generation...");

            // Configure generator 
            var config = RepositoryGeneratorSettings.GetConfigs();
            var repositoryGenerator = new RepositoryCodeFactory(config);

            // run generation
            repositoryGenerator.GenerateRepository();

            Console.WriteLine("Repository generation complated.");
        }
    }

    public class ServiceDetail
    {
        public string UserName { get; set; }
        public string FileName { get; set; }
    }
}
