using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WCFGenerator
{
    class GeneratorHelper : CSharpSyntaxWalker
    {
        public static Solution Solution { get; set; }

        public static Project Project { get; set; }

        public static string GetFullMetadataName(INamespaceOrTypeSymbol symbol)
        {
            ISymbol s = symbol;
            var sb = new StringBuilder(s.MetadataName);

            var last = s;
            s = s.ContainingSymbol;
            while (!IsRootNamespace(s))
            {
                if (s is ITypeSymbol && last is ITypeSymbol)
                {
                    sb.Insert(0, '+');
                }
                else
                {
                    sb.Insert(0, '.');
                }
                sb.Insert(0, s.MetadataName);
                s = s.ContainingSymbol;
            }

            return sb.ToString();
        }

        private static bool IsRootNamespace(ISymbol s)
        {
            return s is INamespaceSymbol && ((INamespaceSymbol)s).IsGlobalNamespace;
        }

        public static async Task<IEnumerable<ClassDeclarationSyntax>> GetAllClasses(string projectName, bool isSkipAttribute, string attribute)
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
                    classes = classVisitor.classes.Where(x => x.AttributeLists
                        .Any(att => att.Attributes
                            .Any(att2 => att2.Name.ToString() == attribute))).ToList();
                }
                else
                {
                    classes = classVisitor.classes;
                }
            }

            return classes;
        }

        public static async Task<ClassDeclarationSyntax> FindClassWithBasedClass(string projectName, string className, string implClass = null)
        {
            var implementedClass = implClass ?? "I" + className;
            var project = Solution.Projects.First(x => x.Name == projectName);

            if (project != null)
            {
                var compilation = await project.GetCompilationAsync();
                var classVisitor = new ClassVirtualizationVisitor();

                foreach (var syntaxTree in compilation.SyntaxTrees)
                {
                    classVisitor.Visit(syntaxTree.GetRoot());
                }

                var classesByName = classVisitor.classes.Where(x => x.Identifier.ToString() == className).ToList();

                foreach (var foundClass in classesByName)
                {
                    if (foundClass.BaseList != null && foundClass.BaseList.Types.Any(elem => elem.ToString() == implementedClass))
                    {
                        return foundClass;
                    }
                }
            }

            return null;
        }

        public static async Task<IEnumerable<InterfaceDeclarationSyntax>> GetImplementedInterfaces(string interfacesProjectName, ClassDeclarationSyntax repositoryClass)
        {
            var interfaceVisitor = new InterfaceVirtualizationVisitor();
            var project = Solution.Projects.First(x => x.Name == interfacesProjectName);

            var tree = repositoryClass.SyntaxTree;
            var cls = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Last();

            var compilation = CSharpCompilation.Create("comp").AddSyntaxTrees(tree);
            var projectCompilation = await project.GetCompilationAsync();

            var names = compilation.GetSemanticModel(tree).GetDeclaredSymbol(cls).Interfaces;

            foreach (var syntaxTree in projectCompilation.SyntaxTrees)
            {
                interfaceVisitor.Visit(syntaxTree.GetRoot());
            }

            return interfaceVisitor.interfaces.Where(x => names.Any(a => a.Name == x.Identifier.ToString()));
        }

        public static async Task<List<InterfaceDeclarationSyntax>> GetImplementedInterfaces(string interfaceProjectName, string interfaceName)
        {
            var interfaceProject = Solution.Projects.First(x => x.Name == interfaceProjectName);
            var projectCompilation = await interfaceProject.GetCompilationAsync();

            var interfaceVisitor = new InterfaceVirtualizationVisitor();

            foreach (var syntaxTree in projectCompilation.SyntaxTrees)
            {
                interfaceVisitor.Visit(syntaxTree.GetRoot());
            }

            var interfaces = interfaceVisitor.interfaces;
            var resultList = new List<InterfaceDeclarationSyntax>();

            foreach (var inf in interfaces.Where(inf => inf.BaseList != null))
            {
                foreach (var type in inf.BaseList.Types)
                {
                    if (type.ToString() == interfaceName)
                    {
                        resultList.Add(inf);
                    }
                }
            }

            return resultList;
        }

        

        public static List<MethodDeclarationSyntax> GetMethodsFromMembers(List<MemberDeclarationSyntax> members)
        {
            return members.OfType<MethodDeclarationSyntax>().ToList();
        }

        public static List<PropertyDeclarationSyntax> GetPropertiesFromMembers(List<MemberDeclarationSyntax> members)
        {
            return members.OfType<PropertyDeclarationSyntax>().ToList();
        }

        public static SemanticModel GetSemanticModelFromTree(SyntaxTree syntaxTree)
        {
            var Mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation", syntaxTrees: new[] { syntaxTree }, references: new[] { Mscorlib });

            return compilation.GetSemanticModel(syntaxTree);
        }

    }
}
