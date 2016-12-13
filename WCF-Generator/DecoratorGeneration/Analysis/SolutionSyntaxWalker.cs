using System;
using Microsoft.CodeAnalysis;

namespace WCFGenerator.DecoratorGeneration.Analysis
{
    internal class SyntaxWalker 
    {
        public SyntaxWalker(Solution solution, string project)
        {
            if (solution == null) throw new ArgumentException("solution");
            if (project == null) throw new ArgumentException("classProject");
        }
    }
}
