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

        public override int GetHashCode()
        {
            return (NamedTypeSymbol?.GetHashCode() ?? 0)/2 + (ClassDeclarationSyntax?.GetHashCode() ?? 0)/2;
        }

        public override bool Equals(object obj)
        {
            var info = obj as ClassCompilerInfo;
            if (info == null)
            {
                return false;
            }
            return info.ClassDeclarationSyntax == ClassDeclarationSyntax && info.NamedTypeSymbol == NamedTypeSymbol;
        }
    }
}
