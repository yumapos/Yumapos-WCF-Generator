using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFGenerator.Common.Infrastructure
{
    public class EnumCompilerInfo
    {
        public EnumDeclarationSyntax EnumDeclarationSyntax { get; set; }
        public INamedTypeSymbol NamedTypeSymbol { get; set; }
    }
}
