using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FirstRoslynApp
{
    public class ClassVirtualizationVisitor : CSharpSyntaxRewriter
    {
        public readonly List<ClassDeclarationSyntax> classes = new List<ClassDeclarationSyntax>();

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            node = (ClassDeclarationSyntax)base.VisitClassDeclaration(node);

            classes.Add(node);
            return node;
        }
    }

    internal class InterfaceVirtualizationVisitor : CSharpSyntaxRewriter
    {
        public readonly List<InterfaceDeclarationSyntax> interfaces = new List<InterfaceDeclarationSyntax>();

        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            node = (InterfaceDeclarationSyntax)base.VisitInterfaceDeclaration(node);
            interfaces.Add(node);

            return node;
        }
    }

    public class NamedTypeVisitor : SymbolVisitor
    {
        public readonly List<INamespaceSymbol> symbols = new List<INamespaceSymbol>();

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            symbols.Add(symbol);

            foreach (var childSymbol in symbol.GetMembers())
            {
                childSymbol.Accept(this);
            }
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            foreach (var childSymbol in symbol.GetTypeMembers())
            {
                childSymbol.Accept(this);
            }
        }
    }
}
