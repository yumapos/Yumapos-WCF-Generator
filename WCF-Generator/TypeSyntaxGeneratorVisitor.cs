using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WCFGenerator
{
    class TypeSyntaxGeneratorVisitor : SymbolVisitor<TypeSyntax>
    {
        public static readonly TypeSyntaxGeneratorVisitor Instance = new TypeSyntaxGeneratorVisitor();

        private TypeSyntaxGeneratorVisitor()
        {
        }

        public override TypeSyntax DefaultVisit(ISymbol node)
        {
            throw new NotImplementedException();
        }


    }
}
