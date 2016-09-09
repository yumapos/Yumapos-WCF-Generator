﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VersionedRepositoryGeneration.Generator.Analysis
{
    internal class SolutionSyntaxWalker 
    {
        private readonly CSharpCompilation _repositoryModelsCompilation;
        private readonly CSharpCompilation _customRepositoriesCompilation;
        private readonly CSharpCompilation _fullCompilation;

        // classes from all projects with repository models marked repostiry attribute
        private readonly List<ClassDeclarationSyntax> _repositoryModelClasses;

        // classes from target projects
        private readonly List<ClassDeclarationSyntax> _customRepositoryClasses;

        // interfaces from all projects with repository interfaces
        private readonly List<InterfaceDeclarationSyntax> _repositoryInterfaces;

        // classes from all configured projects
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

            // Get repository model classes
            var repositoryModelTrees = GetTrees(solution, repositoryModelsProjects);
            foreach (var tree in repositoryModelTrees)
            {
                var nodes = tree.GetRoot().DescendantNodes().ToList();
                var classes = nodes.OfType<ClassDeclarationSyntax>().ToList();

                var repositoryClasses = classes.Where(x => x.AttributeLists
                   .Any(att => att.Attributes
                       .Any(att2 => att2.Name.ToString().Contains(repositoryAttributeName))));

                _allClasses.AddRange(classes);
                _repositoryModelClasses.AddRange(repositoryClasses);
            }

            // Get repository classes
            var repositoryTrees = GetTrees(solution, new List<string> {targetProject});
            foreach (var tree in repositoryTrees)
            {
                var nodes = tree.GetRoot().DescendantNodes().ToList();
                var classes = nodes.OfType<ClassDeclarationSyntax>().ToList();
                _customRepositoryClasses.AddRange(classes);
            }

            // Get repository intrefaces
            var repositoryInterfaceTrees = GetTrees(solution, repositoryInterfaceProjects);
            foreach (var tree in repositoryInterfaceTrees)
            {
                var nodes = tree.GetRoot().DescendantNodes().ToList();
                var interfaces = nodes.OfType<InterfaceDeclarationSyntax>().ToList();
                _repositoryInterfaces.AddRange(interfaces);
            }

            // Get additional classes
            var additionalTrees = GetTrees(solution, additionalProjectsForAnalysis);
            foreach (var tree in additionalTrees)
            {
                var nodes = tree.GetRoot().DescendantNodes().ToList();
                var classes = nodes.OfType<ClassDeclarationSyntax>().ToList();
                _allClasses.AddRange(classes);
            }

            _repositoryModelsCompilation = CSharpCompilation.Create("RepositoryModelCompilation").AddSyntaxTrees(repositoryModelTrees);
            _customRepositoriesCompilation = CSharpCompilation.Create("CustomRepositoriesCompilation").AddSyntaxTrees(repositoryTrees);

            var allTrees = new List<SyntaxTree>();
            allTrees.AddRange(repositoryModelTrees);
            allTrees.AddRange(repositoryInterfaceTrees);
            allTrees.AddRange(repositoryTrees);
            allTrees.AddRange(additionalTrees);

            _fullCompilation = CSharpCompilation.Create("FullCompilation").AddSyntaxTrees(allTrees);
        }

        public IEnumerable<ClassDeclarationSyntax> GetRepositoryClasses(string attribute)
        {
            return _repositoryModelClasses;
        }

        /// <summary>
        ///     Search class by name. Skip genereted classes.
        /// </summary>
        public IEnumerable<ClassDeclarationSyntax> FindCustomRepositoryClassByName(string className)
        {
            var resultList = _customRepositoryClasses.Where(c => c.Identifier.Text == className);

            return resultList;
        }

        public IEnumerable<InterfaceDeclarationSyntax> GetInheritedInterfaces(string interfaceName)
        {
            var resultList = _repositoryInterfaces.Where(i => i.BaseList !=null && i.BaseList.Types.Any(t => t.Type.ToString() == interfaceName));

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

        public string GetFullPropertyTypeName(PropertyDeclarationSyntax prop)
        {
            var semanticModel = _fullCompilation.GetSemanticModel(prop.SyntaxTree);

            var symbol = semanticModel.GetDeclaredSymbol(prop);

            return symbol.GetMethod.ReturnType.ToString();
        }

        public bool IsBaseClass(BaseTypeSyntax syntax)
        {
            return _allClasses.Any(c => c.Identifier.Text == syntax.Type.ToString());
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
