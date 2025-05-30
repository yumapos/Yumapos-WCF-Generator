using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WCFGenerator.WcfClientGeneration
{
    public class EndPoint
    {
        public string Service { get; set; }
        public string Name { get; set; }
        public string ReturnTypeApi { get; set; }
        public string ReturnType { get; set; }
        public string InterfaceReturnType { get; set; }
        public ParameterListSyntax ParametersList { get; set; }

        public IEnumerable<AttributeListSyntax> Faults { get; set; }
    }
}