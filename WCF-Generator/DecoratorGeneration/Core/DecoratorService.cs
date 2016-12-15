﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using WCFGenerator.Common;
using WCFGenerator.DecoratorGeneration.Configuration;
using WCFGenerator.DecoratorGeneration.Infrastructure;
using SyntaxWalker = WCFGenerator.DecoratorGeneration.Analysis.SyntaxWalker;

namespace WCFGenerator.DecoratorGeneration.Core
{
    internal class DecoratorService
    {
        #region Private

        private readonly SyntaxWalker _syntaxWalker;
        private readonly GeneratorWorkspace _workSpace;
        private readonly DecoratorConfiguration _configuration;

        private string _suffix = "Decorator";
        private string _entryMethodName = "OnEntry";
        private string _exitMethodName = "OnExit";
        private string _exceptionMethodName = "OnException";
        private string _finallyMethodName = "OnFinally";

        #endregion

        #region Constructor

        public DecoratorService(GeneratorWorkspace workSpace, DecoratorConfiguration configuration)
        {
            _workSpace = workSpace;
            _configuration = configuration;
            _syntaxWalker = new SyntaxWalker(_workSpace.Solution, configuration.SolutionProjectName);
        }

        #endregion

        #region Find class for decorate

        public IReadOnlyCollection<ICodeClassDecoratorGenerator> GetDecorators()
        {
            var ret = new List<ICodeClassDecoratorGenerator>();

            foreach (var cls in _configuration.DecoratedClassNames)
            {
                // Init
                var codeClassGenerator = new CodeClassDecoratorGenerator();
                var decoratedClassInfo = new DecoratorInfo()
                {
                    DecoratedClassTypeFullName = cls,
                    DecoratorClassTypeShortName = cls.Split('.').Last() + _suffix
                };
                codeClassGenerator.DecoratorInfo = decoratedClassInfo;

                // Get decorated class
                var decoratedClass = _syntaxWalker.GetClassByFullName(cls);
                if(decoratedClass==null)
                {
                    codeClassGenerator.AnalysisError = string.Format("Decorated class {0} not found!", cls);
                    ret.Add(codeClassGenerator);
                    continue;
                }

                decoratedClassInfo.DecoratedClassTypeShortName = decoratedClass.Name;
                decoratedClassInfo.Namespace = decoratedClass.ContainingNamespace.ToString();
                //decoratedClassInfo.RequiredNamespaces = _syntaxWalker.GetUsings(decoratedClass);

                // Get decorator class
                var decoratorClass = _syntaxWalker.GetClassByFullName(cls + _suffix);
                if (decoratorClass == null)
                {
                    codeClassGenerator.AnalysisError = string.Format("Partial decorator class {0} not found!", decoratedClassInfo.DecoratorClassTypeShortName);
                    ret.Add(codeClassGenerator);
                    continue;
                }
               
                // Get methods for decorating
                var methods = decoratedClass.GetMembers()
                    .Where(m => m.Kind == SymbolKind.Method && m.DeclaredAccessibility == Accessibility.Public && m.MetadataName != ".ctor")
                    .Select(m=> m as IMethodSymbol)
                    .Where(m => m.AssociatedSymbol == null || m.AssociatedSymbol.Kind != SymbolKind.Property)
                    .ToList();

                // Get methods from decorator
                var decoratorImplementedMethods = decoratorClass.GetMembers()
                    .Where(m => m.Kind == SymbolKind.Method)
                    .Select(m => m as IMethodSymbol)
                    .ToList();

                foreach (var m in methods)
                {
                    // Skip already decorated methods
                    if(decoratorImplementedMethods.Any(dm => dm.Name == m.Name)) continue;

                    var method = new MethodInfo()
                    {
                        Name = m.Name,
                        ReturnTypeName = m.ReturnType.ToString(),
                        IsAsync = m.IsAsync,
                        ReturnTypeIsNullble = !m.ReturnType.IsValueType,
                        Parameters = m.Parameters.Select(p => new ParameterInfo()
                        {
                            Name = p.Name,
                            Type = p.Type.ToString()
                        }).ToList(),
                    };

                    decoratedClassInfo.MethodInfos.Add(method);
                }

                // Set flags for include decorator methods
                decoratedClassInfo.OnEntryExist = decoratorImplementedMethods.FirstOrDefault(m => m.Name == _entryMethodName) != null;
                decoratedClassInfo.OnExitExist = decoratorImplementedMethods.FirstOrDefault(m => m.Name == _exitMethodName) != null;
                decoratedClassInfo.OnExceptionExist = decoratorImplementedMethods.FirstOrDefault(m => m.Name == _exceptionMethodName) != null;
                decoratedClassInfo.OnFinallyExist = decoratorImplementedMethods.FirstOrDefault(m => m.Name == _finallyMethodName) != null;

                ret.Add(codeClassGenerator);
            }

            return ret;
        }

        #endregion

        #region Analysis methods

        

        #endregion

      
    }
}