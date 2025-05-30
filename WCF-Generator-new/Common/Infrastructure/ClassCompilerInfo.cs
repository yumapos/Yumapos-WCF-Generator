using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WCFGenerator.Common.Infrastructure
{
    public class ClassCompilerInfo
    {
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; set; }
        public INamedTypeSymbol NamedTypeSymbol { get; set; }
    }
}
