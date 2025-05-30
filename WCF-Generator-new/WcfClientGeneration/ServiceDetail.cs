using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WCFGenerator.WcfClientGeneration
{
    public class ServiceDetail
    {
        public string ClientInterfaceName { get; set; }
        public string FileName { get; set; }

        public string ProjectName { get; set; }
        public string FaultProject { get; set; }
        public string ProjectApi { get; set; }

        public List<string> ProjectApiFolders { get; set; }


        public List<EndPoint> ServiceMethods { get; set; }
        public List<MethodDeclarationSyntax> ImplementedMethods { get; set; }
    }
}