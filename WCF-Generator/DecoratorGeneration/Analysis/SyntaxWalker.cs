using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WCFGenerator.Common;

namespace WCFGenerator.DecoratorGeneration.Analysis
{
    internal class SyntaxWalker 
    {
        private readonly List<ClassDeclarationSyntax> _allClasses;
        private readonly CSharpCompilation _projectCompilation;


        public SyntaxWalker(Solution solution, string project)
        {
            if (solution == null) throw new ArgumentException("solution");
            if (project == null) throw new ArgumentException("classProject");

            _allClasses = new List<ClassDeclarationSyntax>();
            var trees = solution.GetTrees(new[] {project});
            _allClasses = trees.SelectMany(t => t.GetClasses()).ToList();
            _projectCompilation = CSharpCompilation.Create("ProjectCompilation").AddSyntaxTrees(trees);
        }

        public INamedTypeSymbol GetClassByFullName(string className)
        {
            return _projectCompilation.GetClass(className);
        }
    }
}
