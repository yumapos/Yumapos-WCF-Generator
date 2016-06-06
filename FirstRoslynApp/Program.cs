using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using Nito.AsyncEx;

namespace FirstRoslynApp
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

            WCFGenerator wcf = new WCFGenerator
            {
                SolutionPath = @"C:\Users\denis.ev\Documents\YumaPos-Client-WPF\YumaPos.Client.sln",
                ProjectName = "YumaPos.Client.WCF",
                ProjectApi = "YumaPos.Shared.Infrastructure",
                Services = new List<string>()
                {
                    "IBackOfficeService.cs",
                    "IService.cs"
                }
            };

            AsyncContext.Run(() => wcf.Start(args));

        }
    }
}
