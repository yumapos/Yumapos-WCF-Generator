using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WCFGenerator.MappingsGeneration.Infrastructure
{
    public class ClassCompilerInfo
    {
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; set; }
        public INamedTypeSymbol NamedTypeSymbol { get; set; }
    }
}
