using System;
using System.Collections;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WCFGenerator.RepositoriesGeneration.Helpers
{
    internal static class ExtensionMethods
    {
        #region Enum extensions

        public static string GetName(this System.Enum en)
        {
            var t = en.GetType();
            var n = System.Enum.GetName(t, en);
            return n;
        }

        #endregion

        #region Code formatting extensions

        public static string FirstSymbolToLower(this string str)
        {
            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        public static string SurroundWithComments(this string codeText)
        {
            return "/*\r\n" + codeText + "\r\n*/";
        }

        public static string SurroundWithRegion(this string codeText, string regionName)
        {
            return "#region " + regionName + "\r\n\r\n" + codeText + "\r\n\r\n#endregion";
        }

        public static string SurroundWithQuotes(this string codeText)
        {
            return "\"" + codeText + "\"";
        }

        #endregion

        #region Syntax analysis extensions

        public static bool AttributeExist(this PropertyDeclarationSyntax p, string attributeName)
        {
            return p.AttributeLists.Any(l => l.Attributes.Any(a => a.Name.ToString() == attributeName));
        }
        public static bool AttributeExist(this ClassDeclarationSyntax c, string attributeName)
        {
            return c.AttributeLists.Any(l => l.Attributes.Any(a => a.Name.ToString() == attributeName));
        }

        public static bool ConstructorImplementationExist(this ClassDeclarationSyntax customRepository)
        {
            return customRepository.Members.Any(e => (e as ConstructorDeclarationSyntax) != null);
        }

        public static bool BaseTypeExist(this ClassDeclarationSyntax syntax, string baseTypeName)
        {
            return syntax.BaseList != null && syntax.BaseList.Types.Any(t => t.ToString() == baseTypeName);
        }

        public static bool SetterExist(this PropertyDeclarationSyntax codeproperty)
        {
            try
            {
                if (codeproperty.AccessorList.Accessors.FirstOrDefault(x => x.Kind() == SyntaxKind.SetAccessorDeclaration) != null)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        public static bool GetterExist(this PropertyDeclarationSyntax codeproperty)
        {
            try
            {
                if (codeproperty.AccessorList.Accessors.FirstOrDefault(x => x.Kind() == SyntaxKind.GetAccessorDeclaration) != null)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        public static bool IsEnumerable(this string t)
        {
            return t.Contains("IEnumerable<");
        }

        public static bool IsInt(this string t)
        {
            return t.Contains("int");
        }

        public static bool IsGuid(this string t)
        {
            return t.Contains("Guid");
        }

        #endregion

        #region Type analysis extensions

        public static bool IsEnumerable(this Type t)
        {
            return t != null && typeof(IEnumerable).IsAssignableFrom(t);
        }

        #endregion
    }
}