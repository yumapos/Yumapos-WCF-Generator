using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WCFGenerator.RepositoriesGeneration.Helpers;

namespace WCFGenerator.RepositoriesGeneration.Analysis
{
    internal class SolutionSyntaxWalker 
    {
        private readonly CSharpCompilation _repositoryModelsCompilation;
        private readonly CSharpCompilation _customRepositoriesCompilation;
        private readonly CSharpCompilation _fullCompilation;

        // Classes from all projects with repository models marked repostiry attribute
        private readonly List<ClassDeclarationSyntax> _repositoryModelClasses;

        // Classes from target projects
        private readonly List<ClassDeclarationSyntax> _customRepositoryClasses;

        // interfaces from all projects with repository interfaces
        private readonly List<InterfaceDeclarationSyntax> _repositoryInterfaces;

        // Classes from all configured projects
        private readonly List<ClassDeclarationSyntax> _allClasses;

        public SolutionSyntaxWalker(Solution solution, List<string> repositoryModelsProjects, string repositoryAttributeName, List<string> repositoryInterfaceProjects, string targetProject,  List<string> additionalProjectsForAnalysis)
        {
            if (solution == null) throw new ArgumentException("solution");
            if (repositoryModelsProjects == null) throw new ArgumentException("repositoryModelsProjects");
            if (repositoryAttributeName == null) throw new ArgumentException("repositoryAttributeName");
            if (repositoryInterfaceProjects == null) throw new ArgumentException("repositoryInterfaceProjects");
            if (targetProject == null) throw new ArgumentException("targetProject");

            _allClasses = new List<ClassDeclarationSyntax>();
            _customRepositoryClasses = new List<ClassDeclarationSyntax>();
            _repositoryModelClasses = new List<ClassDeclarationSyntax>();
            _repositoryInterfaces = new List<InterfaceDeclarationSyntax>();

            // Get repository model Classes
            var repositoryModelTrees = GetTrees(solution, repositoryModelsProjects);
            var allClasses = repositoryModelTrees
                .SelectMany(t => t.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
                .ToList();
            _allClasses.AddRange(allClasses);

            var repositoryModels = allClasses.Where(c => c.AttributeExist(repositoryAttributeName));
            _repositoryModelClasses.AddRange(repositoryModels);

            // Get repository Classes
            var repositoryTrees = GetTrees(solution, new List<string> {targetProject});
            var customClasses = repositoryTrees
                .SelectMany(t => t.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
                .ToList();
            _customRepositoryClasses.AddRange(customClasses);

            // Get repository intrefaces
            var repositoryInterfaceTrees = GetTrees(solution, repositoryInterfaceProjects);
            var repositoryInterfaces = repositoryInterfaceTrees
                .SelectMany(t => t.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>())
                .ToList();
            _repositoryInterfaces.AddRange(repositoryInterfaces);

            // Get additional Classes
            var additionalTrees = GetTrees(solution, additionalProjectsForAnalysis);
            var additionalClasses = repositoryTrees
                .SelectMany(t => t.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
                .ToList();
            _allClasses.AddRange(additionalClasses);

            _repositoryModelsCompilation = CSharpCompilation.Create("RepositoryModelCompilation").AddSyntaxTrees(repositoryModelTrees);
            _customRepositoriesCompilation = CSharpCompilation.Create("CustomRepositoriesCompilation").AddSyntaxTrees(repositoryTrees);

            var allTrees = new List<SyntaxTree>();
            allTrees.AddRange(repositoryModelTrees);
            allTrees.AddRange(repositoryInterfaceTrees);
            allTrees.AddRange(repositoryTrees);
            allTrees.AddRange(additionalTrees);
            
            _fullCompilation = CSharpCompilation.Create("FullCompilation")
                .AddSyntaxTrees(allTrees)
                .WithReferences(new List<MetadataReference>()
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(IEnumerable).Assembly.Location)
                });
        }

        public IEnumerable<ClassDeclarationSyntax> GetRepositoryClasses()
        {
            return _repositoryModelClasses;
        }

        /// <summary>
        ///     Search custom repository class by name. Skip genereted Classes.
        /// </summary>
        public IEnumerable<ClassDeclarationSyntax> FindCustomRepositoryClassByName(string className)
        {
            var resultList = _customRepositoryClasses.Where(c => c.Identifier.Text == className);

            return resultList;
        }

        /// <summary>
        ///     Search type name
        /// </summary>
        public string GetFullTypeName(string typeName)
        {
            var resultList = _fullCompilation.GetSymbolsWithName(s => s == typeName);
            return resultList.FirstOrDefault().ToString();
        }

        public InterfaceDeclarationSyntax GetInheritedInterface(string interfaceName)
        {
            var resultList = _repositoryInterfaces.FirstOrDefault(i => i.BaseList !=null && i.BaseList.Types.Any(t => t.Type.ToString() == interfaceName));
            return resultList;
        }

        public string GetFullRepositoryModelName(BaseTypeDeclarationSyntax codeclass)
        {
            var semanticModel = _repositoryModelsCompilation.GetSemanticModel(codeclass.SyntaxTree);

            var symbol = semanticModel.GetDeclaredSymbol(codeclass);

            return symbol.ContainingNamespace + "." + codeclass.Identifier.Text;
        }

        public string GetCustomRepositoryNamespace(BaseTypeDeclarationSyntax codeclass)
        {
            var semanticModel = _customRepositoriesCompilation.GetSemanticModel(codeclass.SyntaxTree);

            var symbol = semanticModel.GetDeclaredSymbol(codeclass);

            return symbol.ContainingNamespace.ToString();
        }
        public string GetTypeNamespace(BaseTypeDeclarationSyntax codeclass)
        {
            var semanticModel = _fullCompilation.GetSemanticModel(codeclass.SyntaxTree);

            var symbol = semanticModel.GetDeclaredSymbol(codeclass);

            return symbol.ContainingNamespace.ToString();
        }

        public string GetFullPropertyTypeName(PropertyDeclarationSyntax prop)
        {
            var semanticModel = _fullCompilation.GetSemanticModel(prop.SyntaxTree);

            var symbol = semanticModel.GetDeclaredSymbol(prop);
            
            return symbol.GetMethod.ReturnType.ToString();
        }
        public Type GetReturnType(TypeSyntax returnType)
        {
            var semanticModel = _fullCompilation.GetSemanticModel(returnType.SyntaxTree);

            var symbol = semanticModel.GetTypeInfo(returnType);

            return symbol.Type.GetType();
        }

        public bool IsBaseClass(BaseTypeSyntax syntax)
        {
            return _repositoryModelClasses.Any(c => c.Identifier.Text == syntax.Type.ToString());
        }

        private static List<SyntaxTree> GetTrees(Solution solution, List<string> projects)
        {
            var mProjects = solution.Projects.Where(proj => projects.Any(p => p == proj.Name));
            var mDocuments = mProjects.SelectMany(p => p.Documents.Where(d => !d.Name.Contains(".g.cs")));
            var mSyntaxTrees = mDocuments.Select(d => CSharpSyntaxTree.ParseText(d.GetTextAsync().Result)).Where(t => t != null).ToList();
            return mSyntaxTrees;
        }

      
    }
}
