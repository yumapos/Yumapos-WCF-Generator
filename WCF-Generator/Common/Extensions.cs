using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WCFGenerator.Common
{
    public static partial class Extensions
    {
        #region Solution

        public static IReadOnlyCollection<SyntaxTree> GetTrees(this Solution solution, string[] selectedProjects = null)
        {
            var mProjects = selectedProjects != null
                ? solution.Projects.Where(proj => selectedProjects.Any(p => p == proj.Name))
                : solution.Projects;
            var mDocuments = mProjects.SelectMany(p => p.Documents.Where(d => !d.Name.Contains(".g.cs")));
            var mSyntaxTrees = mDocuments.Select(d => CSharpSyntaxTree.ParseText(d.GetTextAsync().Result)).Where(t => t != null).ToList();
            return mSyntaxTrees;
        }

        #endregion


        #region SyntaxTree

        public static IReadOnlyCollection<ClassDeclarationSyntax> GetClasses(this SyntaxTree tree)
        {
            return tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
        }

        #endregion

        #region CSharpCompilation

        public static INamedTypeSymbol GetClass(this CSharpCompilation compilation, string fullTypeName)
        {
            return compilation.GetTypeByMetadataName(fullTypeName);
        }

        public static INamedTypeSymbol GetClass(this CSharpCompilation compilation, BaseTypeDeclarationSyntax codeclass)
        {
            var semanticModel = compilation.GetSemanticModel(codeclass.SyntaxTree);

            var symbol = semanticModel.GetDeclaredSymbol(codeclass);

            return compilation.GetClass(symbol.ContainingNamespace + "." + codeclass.Identifier.Text);
        }

        #endregion

        #region INamedTypeSymbol

        public static string GetFullName(this INamedTypeSymbol symbol)
        {
            return symbol.ContainingNamespace
                   + "." + symbol.Name
                   + ", " + symbol.ContainingAssembly;
        }

        #endregion


        #region ITypeSymbol
        public static string GetFullName(this ITypeSymbol symbol)
        {
            return symbol.ContainingNamespace
                   + "." + symbol.Name
                   + ", " + symbol.ContainingAssembly;
        }
        #endregion

    }
}
