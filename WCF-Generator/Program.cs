using System;
using System.Collections.Generic;
using System.Linq;
using Nito.AsyncEx;
using System.Configuration;
using System.IO;

namespace WCFGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
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

            //var repositoryGenerator = new RepositoryGenerator
            //{
            //    SolutionPath = @"C:\Users\denis.ev\Documents\WCF_source\YumaPos.Server.WCF.sln",
            //    ProjectName = "YumaPos.Client.WCF",
            //    RepositoryAttribute = "DataRepository",
            //    RepositoryMainPlace = "YumaPos.FrontEnd.Data",
            //    RepositorySuffix = "Repository",
            //    RepositoryInterfaces = "YumaPos.FrontEnd.Infrastructure"
            //};
            //repositoryGenerator.RepositoryClassProjects.Add("YumaPos.FrontEnd.Infrastructure");

            //repositoryGenerator.GenerateRepository();

            if (args.Any() && args[0] != null)
            {
                AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", args[0]);
                var solutionPath = ConfigurationManager.AppSettings["SolutionPath"];
                WCFGenerator wcf = new WCFGenerator
                {
                    SolutionPath = solutionPath,
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

                var gen = new SerilizationGeneration(solutionPath);
                
                AsyncContext.Run(() => wcf.Start(args));
                AsyncContext.Run(() => gen.GenerateAll());
            }
            else
            {
                Console.WriteLine("The directory of a solution wasn't specified");
            }
        }
    }

    public class ServiceDetail
    {
        public string UserName { get; set; }
        public string FileName { get; set; }
    }
}
