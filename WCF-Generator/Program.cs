using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Nito.AsyncEx;
using VersionedRepositoryGeneration.Generator.Core;

namespace WCFGenerator
{
    class Program
    {
        private static string ProjectName = "TestRepositoryGeneration";
        private static string RepositoryMainPlace = "TestRepositoryGeneration";
        private static string RepositorySuffix = "Repository";
        private static string RepositoryInterfaces = "TestRepositoryGeneration";
        private static string RepositoryTargetFolder = @"Repositories\Genereted";


        static void Main(string[] args)
        {
            if (args == null || !args.Any() || args[0] == null || !File.Exists(args[0]))
            {
                Console.WriteLine("The directory of a solution wasn't specified");
            }

            var absoluteSlnPath = Path.GetFullPath(args[0]);

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

            RunRepositoryGeneration(absoluteSlnPath);
            RunWcfGeneration(args);
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

        private static void RunRepositoryGeneration(string slnPath)
        {
            Console.WriteLine("Start repository generation...");

            // Configure generator
            var config = new GeneratorConfiguration()
            {
                SolutionPath = slnPath,
                ProjectName = ProjectName,
                RepositoryMainPlace = RepositoryMainPlace,
                RepositorySuffix = RepositorySuffix,
                RepositoryInterfacesProjectName = RepositoryInterfaces,
                RepositoryTargetFolder = RepositoryTargetFolder
            };

            config.RepositoryClassProjects.Add(RepositoryInterfaces);

            var repositoryGenerator = new RepositoryCodeFactory(config);


            // run generation
            var timer = new Stopwatch();
            timer.Start();
            repositoryGenerator.GenerateRepository();
            timer.Stop();

            Console.WriteLine("Generation completed. Elapsed time: {0}", timer.Elapsed.ToString("c"));

            Thread.Sleep(3000);
        }

    }

    public class ServiceDetail
    {
        public string UserName { get; set; }
        public string FileName { get; set; }
    }
}
