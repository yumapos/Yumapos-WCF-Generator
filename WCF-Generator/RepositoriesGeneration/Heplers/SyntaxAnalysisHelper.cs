using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

namespace VersionedRepositoryGeneration.Generator.Heplers
{
    internal static class SyntaxAnalysisHelper
    {
        public static string GetNameSpaceByClass(ClassDeclarationSyntax codeclass, Project project)
        {
            var nameSpace = SymbolFinder.FindDeclarationsAsync(project, codeclass.Identifier.ValueText, ignoreCase: false).Result.Last().ContainingNamespace.ToString();

            return nameSpace.ToString();
        }

        public static string FindType(ClassDeclarationSyntax DOClass, string parameter)
        {
            PropertyDeclarationSyntax elementTrue = null;
            foreach (var elem in DOClass.Members)
            {
                var codeProperty = elem as PropertyDeclarationSyntax;
                if (codeProperty != null && codeProperty.Identifier.ToString() == parameter)
                    elementTrue = codeProperty;
            }

            if (elementTrue == null)
            {
                throw new Exception("Property " + parameter + ", specified in " + DOClass.Identifier + " wasn't found.");
            }

            return elementTrue.Type.ToString();
        }

        public static string FindTypeSql(ClassDeclarationSyntax DOClass, string parameter)
        {
            var variable = FindType(DOClass, parameter);
            return SystemToSqlTypeMapper.GetSqlType(variable);
        }

       
    }
}