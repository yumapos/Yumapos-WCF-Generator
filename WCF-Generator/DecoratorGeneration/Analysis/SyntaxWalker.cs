using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WCFGenerator.Common;

namespace WCFGenerator.DecoratorGeneration.Analysis
{
    internal class SyntaxWalker 
    {
        private readonly List<SyntaxTree> _allTrees;
        private readonly CSharpCompilation _projectCompilation;
        private readonly CSharpCompilation _solutionCompilation;
        private Solution _solution;
        private Project _project;

        public SyntaxWalker(Solution solution, string project)
        {
            if (solution == null) throw new ArgumentException("solution");
            if (project == null) throw new ArgumentException("classProject");

            _solution = solution;
            _project = solution.Projects.First(proj => proj.Name == project);

                var solutionTrees = solution.GetTrees();
            _solutionCompilation = CSharpCompilation.Create("SolutionCompilation")
                .AddSyntaxTrees(solutionTrees)
                .WithReferences(new List<MetadataReference>()
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(IEnumerable).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Task).Assembly.Location)
                });

            _allTrees = solution.GetTrees(new[] {project}).ToList();
            _projectCompilation = CSharpCompilation.Create("ProjectCompilation")
                .AddSyntaxTrees(_allTrees)
                .WithReferences(new List<MetadataReference>()
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(IEnumerable).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Task).Assembly.Location)
                });
        }

        public INamedTypeSymbol GetClassByFullName(string className)
        {
            return _solutionCompilation.GetClass(className);
        }

        public string GetSrcFileProjectFolder(INamedTypeSymbol type)
        {
            var tree = type.Locations.First().SourceTree.GetRoot().SyntaxTree.GetText();
            var doc = _project.Documents.FirstOrDefault(d => 
            {
                var t = d.GetSyntaxTreeAsync().Result.GetText();
                return t == tree;
            });
            return doc !=null ? string.Join("/", doc.Folders) : "";
        }

        public List<string> GetUsings(INamedTypeSymbol cls)
        {
            if (cls == null) throw new ArgumentException("Class can't be null");
            var reference = cls.DeclaringSyntaxReferences.First();
            var tree = reference.SyntaxTree.GetRoot();
            var compilationUnitSyntax = (CompilationUnitSyntax)(tree);
            var ret = compilationUnitSyntax.Usings.Select(u => u.Name.ToString()).ToList();
            return ret;
        }
    }
}
