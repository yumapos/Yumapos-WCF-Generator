using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WCFGenerator
{
    public class SyntaxSerilizationHelper : CSharpSyntaxWalker
    {
        public static Solution Solution { get; set; }

        public static Project Project { get; set; }

        public static List<ClassVirtualizationVisitor> Visitors { get; set; } 


        public static IEnumerable<ClassDeclarationSyntax> GetAllClasses(string projectName, bool isSkipAttribute, string attribute)
        {
            var project = Solution.Projects.First(x => x.Name == projectName);
            var classes = new List<ClassDeclarationSyntax>();

            if (project != null)
            {
                var mDocuments = project.Documents.Where(d => !d.Name.Contains(".g.cs"));
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

    }
}
