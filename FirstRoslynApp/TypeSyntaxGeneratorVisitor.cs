using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FirstRoslynApp
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
