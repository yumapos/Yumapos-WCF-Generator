using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WCFGenerator;

namespace VersionedRepositoryGeneration.Generator.Analysis
{
    internal static class SolutionSyntaxWalker //: CSharpSyntaxWalker
    {
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
            return s is INamespaceSymbol && ((INamespaceSymbol) s).IsGlobalNamespace;
        }

        public static async Task<IEnumerable<ClassDeclarationSyntax>> GetAllClassesWhithAttribute(Solution solution, string projectName,
            bool isSkipAttribute, string attribute)
        {
            var project = solution.Projects.First(x => x.Name == projectName);
            var classes = new List<ClassDeclarationSyntax>();

            if (project != null)
            {
                var compilation = await project.GetCompilationAsync();
                var classVisitor = new ClassVirtualizationRewriter();

                foreach (var syntaxTree in compilation.SyntaxTrees)
                {
                    classVisitor.Visit(syntaxTree.GetRoot());
                }

                if (!isSkipAttribute)
                {
                    classes = classVisitor.classes.Where(x => x.AttributeLists
                        .Any(att => att.Attributes
                            .Any(att2 => att2.Name.ToString().Contains(attribute)))).ToList();
                }
                else
                {
                    classes = classVisitor.classes;
                }
            }

            return classes;
        }

        /// <summary>
        ///     Search class by name. Skip genereted classes.
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="projectName"></param>
        /// <param name="className"></param>
        /// <param name="implClass"></param>
        ///  /// <param name="generatedRepositoryPath">Path to project folder with generated repositories - it should be skipped on analysis</param>
        /// <returns></returns>
        public static async Task<ClassDeclarationSyntax> FindClassByName(Solution solution, string projectName, string className, string generatedRepositoryPath, string implClass = null)
        {
            var implementedClass = implClass ?? "I" + className;
            var project = solution.Projects.First(x => x.Name == projectName);

            if (project != null)
            {
                var compilation = await project.GetCompilationAsync();
                var classVisitor = new ClassVirtualizationRewriter();

                foreach (var syntaxTree in compilation.SyntaxTrees)
                {
                    if(syntaxTree.FilePath.Contains(generatedRepositoryPath)) continue;
                    classVisitor.Visit(syntaxTree.GetRoot());
                }

                var classesByName = classVisitor.classes.FirstOrDefault(x => x.Identifier.ToString() == className);
                return classesByName;
            }

            return null;
        }

        public static async Task<IEnumerable<InterfaceDeclarationSyntax>> GetImplementedInterfaces(Solution solution,
            string interfacesProjectName, ClassDeclarationSyntax repositoryClass)
        {
            var interfaceVisitor = new InterfaceVirtualizationRewriter();
            var project = solution.Projects.First(x => x.Name == interfacesProjectName);

            var tree = repositoryClass.SyntaxTree;
            var cls = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Last();

            // Get interfaces from project
            var projectCompilation = await project.GetCompilationAsync();
            foreach (var syntaxTree in projectCompilation.SyntaxTrees)
            {
                interfaceVisitor.Visit(syntaxTree.GetRoot());
            }

            var compilation = CSharpCompilation.Create("comp")
                .AddSyntaxTrees(projectCompilation.SyntaxTrees);

            var semanticMoidel = compilation.GetSemanticModel(tree);
            var symbol = semanticMoidel.GetDeclaredSymbol(cls);
            

            return interfaceVisitor.interfaces.Where(x => symbol.AllInterfaces.Any(a => a.Name == x.Identifier.ToString()));
        }

        public static async Task<IEnumerable<InterfaceDeclarationSyntax>> GetImplementedInterfaces(Solution solution, string interfaceProjectName,
            string interfaceName)
        {
            var interfaceProject = solution.Projects.First(x => x.Name == interfaceProjectName);
            var projectCompilation = await interfaceProject.GetCompilationAsync();

            var interfaceVisitor = new InterfaceVirtualizationRewriter();

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
        public static async Task<IEnumerable<InterfaceDeclarationSyntax>> GetImplementedInterfacesFrom(Solution solution,
            string interfacesProjectName, ClassDeclarationSyntax repositoryClass)
        {
            var interfaceVisitor = new InterfaceVirtualizationVisitor();
            var project = solution.Projects.First(x => x.Name == interfacesProjectName);

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
            var Mscorlib = MetadataReference.CreateFromFile(typeof (object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation", syntaxTrees: new[] {syntaxTree},
                references: new[] {Mscorlib});

            return compilation.GetSemanticModel(syntaxTree);
        }
    }
}
