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
                   + "." + symbol.Name;
                   //+ ", " + symbol.ContainingAssembly;
        }

        #endregion

        #region ITypeSymbol

        public static string GetFullName(this ITypeSymbol symbol)
        {
            return symbol.ContainingNamespace
                   + "." + symbol.Name;
                   //+ ", " + symbol.ContainingAssembly;
        }

        public static ITypeSymbol[] GetGenericArguments(this ITypeSymbol symbol)
        {
            return ((INamedTypeSymbol)symbol).TypeArguments.ToArray();
        }
        
        #endregion

        #region IPropertySymbol

        public static AttributeData GetAttributeByName(this IPropertySymbol symbol, string name)
        {
            return symbol.GetAttributes().FirstOrDefault(a => a.AttributeClass.Name == name || a.AttributeClass.Name == (name + "Attribute"));
        }

        #endregion

        #region IMethodSymbol

        public static string GetSignature(this IMethodSymbol symbol)
        {
            var sb = new StringBuilder();
            var returnType = symbol.ReturnsVoid ? "void" : symbol.ReturnType.ToString();
            sb.Append(returnType + " " + symbol.Name +"(");
            sb.Append(String.Join(", ", symbol.Parameters.Select(p => p.Type.ToString() + " " + p.Name)));
            sb.Append(")");

            return sb.ToString();
        }

        public static string GetMethodCall(this IMethodSymbol symbol)
        {
            return symbol.Name + '(' + String.Join(", ", symbol.Parameters.Select(p => p.Name.ToString())) + ')';
        }

        public static bool CanBeAwaited(this IMethodSymbol symbol)
        {
            var retTask = symbol.ReturnType.GetFullName().Contains("System.Threading.Tasks.Task");
            // Method with return type "Task" has not mark as async, but method decorator should be generate as async
            return symbol.IsAsync || retTask;
        }

        #endregion

        #region AttributeData

        public static string GetAttributePropertyValue(this AttributeData attribute, string propertyName)
        {
            var prop = attribute.NamedArguments.FirstOrDefault(a => a.Key == propertyName);
            if (prop.Equals(default(KeyValuePair<string,TypedConstant>))) return null;
            return prop.Value.Value.ToString();
        }

        #endregion
    }
}
