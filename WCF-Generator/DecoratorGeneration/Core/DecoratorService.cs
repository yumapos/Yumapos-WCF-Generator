using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WCFGenerator.Common;
using WCFGenerator.DecoratorGeneration.Analysis;
using WCFGenerator.DecoratorGeneration.Configuration;
using WCFGenerator.DecoratorGeneration.Infrastructure;

namespace WCFGenerator.DecoratorGeneration.Core
{
    internal class DecoratorService
    {
        #region Private

        private readonly SyntaxWalker _syntaxWalker;
        private readonly GeneratorWorkspace _workSpace;

        #endregion

        #region Constructor

        public DecoratorService(GeneratorWorkspace workSpace, string project)
        {
            _workSpace = workSpace;
            _syntaxWalker = new SyntaxWalker(_workSpace.Solution, project);
        }

        #endregion

        #region Find class for decorate

        public IReadOnlyCollection<ICodeClassGeneratorDecorator> GetDecorators()
        {
           return new List<ICodeClassGeneratorDecorator>(){};
        }

        #endregion

        #region Analysis methods

        

        #endregion

      
    }
}