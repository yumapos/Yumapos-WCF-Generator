using System.Collections.Generic;
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

            foreach (var cls in _configuration.DecoratedClass)
            {
                // Init
                var codeClassGenerator = new CodeClassDecoratorGenerator();
                var decoratedClassInfo = new DecoratorInfo()
                {
                    DecoratedClassTypeFullName = cls.SourceClassName,
                    DecoratorClassTypeShortName = cls.TargetClassName,
                };
                codeClassGenerator.DecoratorInfo = decoratedClassInfo;

                // Get decorated class
                var decoratedClass = _syntaxWalker.GetClassByFullName(cls.SourceClassName);
                if(decoratedClass==null)
                {
                    codeClassGenerator.AnalysisError = string.Format("Decorated class {0} not found!", cls.SourceClassName);
                    ret.Add(codeClassGenerator);
                    continue;
                }

                decoratedClassInfo.DecoratedClassTypeShortName = decoratedClass.Name;
                decoratedClassInfo.Namespace = decoratedClass.ContainingNamespace.ToString();
                decoratedClassInfo.DecoratorClassTypeFullName = decoratedClassInfo.Namespace + "." + cls.TargetClassName;

                //decoratedClassInfo.RequiredNamespaces = _syntaxWalker.GetUsings(decoratedClass);

                // Get decorator class
                var decoratorClass = _syntaxWalker.GetClassByFullName(decoratedClassInfo.DecoratorClassTypeFullName);
                if (decoratorClass == null)
                {
                    codeClassGenerator.AnalysisError = string.Format("Partial decorator class {0} not found!", decoratedClassInfo.DecoratorClassTypeFullName);
                    decoratedClassInfo.DecoratedClassProjectFolder = _syntaxWalker.GetSrcFileProjectFolder(decoratedClass);
                    ret.Add(codeClassGenerator);
                    continue;
                }
                decoratedClassInfo.DecoratedClassProjectFolder = _syntaxWalker.GetSrcFileProjectFolder(decoratorClass);

                // Get methods for decorating
                var methods = decoratedClass.GetMembers()
                    .Where(m => m.Kind == SymbolKind.Method && m.DeclaredAccessibility == Accessibility.Public && m.MetadataName != ".ctor" )
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
                    if(_configuration.IgnoreMethodAttributeName != null && m.GetAttributes().Any(a => a.AttributeClass.Name == _configuration.IgnoreMethodAttributeName)) continue;

                    var method = new MethodInfo()
                    {
                        Name = m.Name,
                        ReturnTypeName = m.ReturnType.ToString(),
                        ReturnTypeIsNullble = !m.ReturnType.IsValueType,
                        Parameters = m.Parameters.Select(p => new ParameterInfo()
                        {
                            Name = p.Name,
                            Type = p.Type.ToString()
                        }).ToList(),
                    };
                    var retTask = method.ReturnTypeName.Contains("System.Threading.Tasks.Task");
                    // Method with return type "Task" has not mark as async, but method decorator should be generate as async
                    method.IsAsync = m.IsAsync || retTask;

                    // Map ResponseDto TODO think about good idia
                    if (cls.OnEntryReturnType.Contains("ICommandExecutionResult"))
                    {
                        var fullName = retTask ? method.GetTaskRetunTypeName() : method.ReturnTypeName;

                        method.OnEntryResultMap = "\r\nif (!res.Success)\r\n{\r\nret = new " + fullName + "()\r\n{\r\nPostprocessingType = res.PostprocessingType,\r\nContext = SerializationService.Serialize(res.Context, SerializationOptions)\r\n};}";
                        method.ReturnValueWrap = "else\r\n{{replace}\r\n}";
                    }

                    decoratedClassInfo.MethodInfos.Add(method);
                }

                // Set flags for include decorator methods
                if(!cls.UseAllOption)
                {
                    decoratedClassInfo.OnEntryExist = decoratorImplementedMethods.FirstOrDefault(m => m.Name == _entryMethodName || m.Name == _entryMethodName + "Async") != null;
                    decoratedClassInfo.OnExitExist = decoratorImplementedMethods.FirstOrDefault(m => m.Name == _exitMethodName || m.Name == _exitMethodName + "Async") != null;
                    decoratedClassInfo.OnExceptionExist = decoratorImplementedMethods.FirstOrDefault(m => m.Name == _exceptionMethodName) != null;
                    decoratedClassInfo.OnFinallyExist = decoratorImplementedMethods.FirstOrDefault(m => m.Name == _finallyMethodName) != null;
                }

                ret.Add(codeClassGenerator);
            }

            return ret;
        }

        #endregion

        #region Analysis methods

        

        #endregion

      
    }
}