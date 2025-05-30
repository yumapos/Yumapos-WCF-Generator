using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WCFGenerator.Common;
using WCFGenerator.RepositoriesGeneration.Services;

namespace WCFGenerator
{
    public class SyntaxSerilizationHelper : CSharpSyntaxWalker
    {
        public static Solution Solution { get; set; }

        public static Project Project { get; set; }

        public static List<ClassVirtualizationVisitor> Visitors { get; set; } 


        public static IEnumerable<ClassDeclarationSyntax> GetAllClasses(string projectName, bool isSkipAttribute, string attribute = "", bool skipGeneratedClasses= true)
        {
            var project = Solution.Projects.First(x => x.Name == projectName);
            var classes = new List<ClassDeclarationSyntax>();
            
            if (project != null)
            {
                
                var mDocuments = skipGeneratedClasses 
                    ? project.Documents.Where(d => !d.Name.Contains(".g.cs"))
                    : project.Documents;
                var mSyntaxTrees = mDocuments.Select(d => CSharpSyntaxTree.ParseText(d.GetTextAsync().Result)).Where(t => t != null).ToList();
                var classVisitor = new ClassVirtualizationVisitor();

                foreach (var syntaxTree in mSyntaxTrees)
                {
                    classVisitor.Visit(syntaxTree.GetRoot());
                }

                if (!isSkipAttribute)
                {
                    classes = classVisitor.Classes.Where(x => x.AttributeLists
                        .Any(att => att.Attributes
                            .Any(att2 => att2.Name.ToString() == attribute))).ToList();
                }
                else
                {
                    classes = classVisitor.Classes;
                }
            }

            return classes;
        }

        public static void InitVisitor(IEnumerable<string> projectNames)
        {
            Visitors = new List<ClassVirtualizationVisitor>();
            foreach (var name in projectNames)
            {
                var project = Solution.Projects.FirstOrDefault(x => x.Name == name);
                if (project != null)
                {
                    var mDocuments = project.Documents.Where(d => !d.Name.Contains(".g.cs"));
                    var mSyntaxTrees = mDocuments.Select(d => CSharpSyntaxTree.ParseText(d.GetTextAsync().Result)).Where(t => t != null).ToList();
                    var classVisitor = new ClassVirtualizationVisitor();

                    foreach (var syntaxTree in mSyntaxTrees)
                    {
                        classVisitor.Visit(syntaxTree.GetRoot());
                    }
                    Visitors.Add(classVisitor);
                }
            }
        }

        public static ClassDeclarationSyntax FindClass(string className)
        {
            foreach (var classVirtualizationVisitor in Visitors)
            {
                var findedClass = classVirtualizationVisitor.Classes.FirstOrDefault(x => x.Identifier.ToString().Trim() == className);
                if (findedClass != null)
                {
                    return findedClass;
                }
            }
            return null;
        }

        public static bool PropertyAttributeExist(BasePropertyDeclarationSyntax p, string attributeName)
        {
            return p.AttributeLists.Any(l => l.Attributes.Any(a => a.Name.ToString() == attributeName));
        }

        public static bool FieldAttributeExist(FieldDeclarationSyntax p, string attributeName)
        {
            return p.AttributeLists.Any(l => l.Attributes.Any(a => a.Name.ToString() == attributeName));
        }

        public static bool ClassAttributeExist(ClassDeclarationSyntax p, string attributeName)
        {
            return p.AttributeLists.Any(l => l.Attributes.Any(a => a.Name.ToString() == attributeName));
        }


        public static List<AttributeAndProperties> GetAttributesAndPropepertiesCollection(ClassDeclarationSyntax element)
        {
            SyntaxList<AttributeListSyntax> attributes = new SyntaxList<AttributeListSyntax>();

            var codeClass = element;
            if (codeClass != null)
                attributes = codeClass.AttributeLists;

            var attributeCollection = new List<AttributeAndProperties>();
            var listOfStringProperties = new List<string>();

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

                    attributeCollection.Add(new AttributeAndProperties
                    {
                        Name = attr.Name.ToString(),
                        Parameters = dictionaryOfAttributes
                    });

                }
            }

            return attributeCollection;
        }

    }
}
