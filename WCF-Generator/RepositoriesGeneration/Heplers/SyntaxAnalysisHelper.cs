using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using VersionedRepositoryGeneration.Generator.Services;

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

        public static bool SetterInCodePropertyExist(PropertyDeclarationSyntax codeproperty)
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
        public static bool GetterInCodePropertyExist(PropertyDeclarationSyntax codeproperty)
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

        public static IEnumerable<AttributeAndPropeperties> GetAttributesAndPropepertiesCollection(MemberDeclarationSyntax element)
        {
            SyntaxList<AttributeListSyntax> attributes = new SyntaxList<AttributeListSyntax>();

            var codeClass = element as ClassDeclarationSyntax;
            if (codeClass != null)
                attributes = codeClass.AttributeLists;

            var codeProperty = element as PropertyDeclarationSyntax;
            if (codeProperty != null)
                attributes = codeProperty.AttributeLists;

            var attributeCollection = new List<AttributeAndPropeperties>();
            var listOfStringProperties = new List<string>();

            //            Regex attributesRegex = new Regex(@"\[\s*(?<Name>\w*)\s*(\((?<arguments>.*)\))?\s*\]", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //            MatchCollection matchesAttrs = attributesRegex.Matches(attributeString);

            foreach (var ca in attributes)
            {
                foreach (var attr in ca.Attributes)
                {
                    var properties = attr.ArgumentList?.ToString() ?? "";

                    var dictionaryOfAttributes = new Dictionary<string, string>();
                    var countProperties = 0;
                    listOfStringProperties.Clear();

                    Regex attributesRegex = new Regex(@"(@""(?:""""|[^""])*"")|(""(?:\\""|\\r|\\n|\\t|\\\\|[^""\\])*"")",
                        RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    MatchCollection matchesProperties = attributesRegex.Matches(properties);

                    foreach (var property in matchesProperties)
                    {
                        properties = properties.Replace(property.ToString(), "%%string" + countProperties + "%%");
                        listOfStringProperties.Add(property.ToString());
                        countProperties++;
                    }

                    countProperties = 0;
                    foreach (string prop in properties.Split(',').ToList())
                    {
                        var property = prop.Replace("(", "").Replace(")", "");
                        if (property.Contains("%%string"))
                        {
                            if (property.Split(':', '=').Count() == 2)
                            {
                                if (property.Split(':', '=')[1].Contains("%%string"))
                                    dictionaryOfAttributes.Add(property.Split(':', '=')[0],
                                        listOfStringProperties[
                                            Convert.ToInt32(property.Split(':', '=')[1].Replace("%%string", "")
                                                .Replace("%", ""))].Replace("\"", ""));
                            }
                            else
                                dictionaryOfAttributes.Add(countProperties.ToString(),
                                    listOfStringProperties[
                                        Convert.ToInt32(property.Replace("%%string", "").Replace("%", ""))].Replace("\"", ""));
                        }
                        else
                        {
                            if (property.Split(':', '=').Count() == 2)
                                dictionaryOfAttributes.Add(property.Split(':', '=')[0], property.Split(':', '=')[1]);
                            else
                                dictionaryOfAttributes.Add(countProperties.ToString(), property);
                        }

                        countProperties++;
                    }

                    attributeCollection.Add(new AttributeAndPropeperties
                    {
                        Name = attr.Name.ToString(),
                        Parameters = dictionaryOfAttributes
                    });
                }
            }

            return attributeCollection;
        }

        public static bool ConstructorImplementationExist(ClassDeclarationSyntax customRepository)
        {
            return customRepository.Members.Any(e => (e as ConstructorDeclarationSyntax) != null);
        }

    }
}