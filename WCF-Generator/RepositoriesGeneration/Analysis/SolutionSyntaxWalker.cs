using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VersionedRepositoryGeneration.Generator.Analysis
{
    internal static class SolutionSyntaxWalker //: CSharpSyntaxWalker
    {
        private static CSharpCompilation _allProjectsCompilation;
        private static List<InterfaceDeclarationSyntax> _allInterfaces;
        private static List<ClassDeclarationSyntax> _allClasses;

        public static void SetCurrentSolution(Solution solution, List<string> ignoredPath)
        {
            // Full solution compilation //TODO - take used project only
            _allProjectsCompilation = CSharpCompilation.Create("comp").AddSyntaxTrees(solution.Projects.SelectMany(p => p.GetCompilationAsync().Result.SyntaxTrees));

            _allInterfaces = new List<InterfaceDeclarationSyntax>();
            _allClasses = new List<ClassDeclarationSyntax>();

            foreach (var tree in _allProjectsCompilation.SyntaxTrees)
            {
                var nodes = tree.GetRoot().DescendantNodes().ToList();

                var interfaces = nodes.OfType<InterfaceDeclarationSyntax>();
                _allInterfaces.AddRange(interfaces);

                if (ignoredPath.All(p => !tree.FilePath.Contains(p)))
                {
                    var classes = nodes.OfType<ClassDeclarationSyntax>();
                    _allClasses.AddRange(classes);
                }
            }
        }

        public static IEnumerable<ClassDeclarationSyntax> GetAllClassesWhithAttribute(string projectName,string attribute)
        {
             var classes = _allClasses.Where(x => x.AttributeLists
                    .Any(att => att.Attributes
                        .Any(att2 => att2.Name.ToString().Contains(attribute))));

            return classes;
        }

        /// <summary>
        ///     Search class by name. Skip genereted classes.
        /// </summary>
        ///  /// <param name="generatedRepositoryPath">Path to project folder with generated repositories - it should be skipped on analysis</param>
        /// <returns></returns>
        public static IEnumerable<ClassDeclarationSyntax> FindClassByName(string className, string generatedRepositoryPath)
        {
            var resultList = _allClasses.Where(с => с.Identifier.Text == className);

            return resultList;
        }

        public static IEnumerable<string> GetImplementedInterfaceNames(ClassDeclarationSyntax repositoryClass)
        {
            var semanticMoidel = _allProjectsCompilation.GetSemanticModel(repositoryClass.SyntaxTree);

            var symbol = semanticMoidel.GetDeclaredSymbol(repositoryClass);

            return symbol.AllInterfaces.Select(i=>i.Name);
        }

        public static IEnumerable<InterfaceDeclarationSyntax> GetImplementedInterfaces(string interfaceName)
        {
            var resultList = _allInterfaces.Where(i => i.BaseList !=null && i.BaseList.Types.Any(t => t.Type.ToString() == interfaceName));

            return resultList;
        }

        public static List<MethodDeclarationSyntax> GetMethodsFromMembers(List<MemberDeclarationSyntax> members)
        {
            return members.OfType<MethodDeclarationSyntax>().ToList();
        }

        public static string GetNameSpaceByClass(ClassDeclarationSyntax codeclass)
        {
            var semanticMoidel = _allProjectsCompilation.GetSemanticModel(codeclass.SyntaxTree);

            var symbol = semanticMoidel.GetDeclaredSymbol(codeclass);

            return symbol.ContainingNamespace + "." + codeclass.Identifier.Text;
        }
    }
}
