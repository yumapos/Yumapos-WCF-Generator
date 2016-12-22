using System;
using System.Linq;
using System.Text;
using WCFGenerator.DecoratorGeneration.Infrastructure;
using WCFGenerator.RepositoriesGeneration.Heplers;

namespace WCFGenerator.DecoratorGeneration.Core
{
    internal class CodeClassDecoratorGenerator : ICodeClassDecoratorGenerator
    {
        private string _decoratedComponent = "DecoratedComponent";

        public DecoratorInfo DecoratorInfo { get; set; }


        #region Implementation of ICodeClassGenerator

        public string GetUsings()
        {
            var sb = new StringBuilder();

            foreach (var nameSpace in DecoratorInfo.RequiredNamespaces)
            {
                sb.AppendLine("using " + nameSpace + ";");
            }

            return sb.ToString();
        }

        public string GetNamespaceDeclaration()
        {
            return "namespace " + DecoratorInfo.Namespace;
        }

        public string GetClassDeclaration()
        {
            return "public partial class " + DecoratorInfo.DecoratorClassTypeShortName;
        }

        public string GetFields()
        {
            return "";
        }

        public string GetProperties()
        {
            return "private "+ DecoratorInfo.DecoratedClassTypeFullName + " " +_decoratedComponent + "{get;set;}";
        }

        public string GetConstructors()
        {
            return "";
        }

        public string GetMethods()
        {
            return DecoratorInfo.MethodInfos.Aggregate("", (s, method) => s + GenerateMethod(method));
        }

        public string GetFullCode()
        {
            var sb = new StringBuilder();

            // auto-generated header
            sb.AppendLine(CodeHelper.GeneratedDocumentHeader);

            // check analysis error
            if (AnalysisError == null)
            {
                try
                {
                    // usings
                    sb.AppendLine(GetUsings());
                    sb.AppendLine("");
                    // namespace
                    sb.AppendLine(GetNamespaceDeclaration());
                    // open namespace
                    sb.AppendLine("{");
                    // class
                    sb.AppendLine(GetClassDeclaration());
                    // open class
                    sb.AppendLine("{");
                    // members
                    sb.AppendLine(GetFields());
                    sb.AppendLine(GetProperties());
                    sb.AppendLine(GetConstructors());
                    sb.AppendLine(GetMethods());
                    // close class
                    sb.AppendLine("}");
                    // close namespase
                    sb.AppendLine("}");
                }
                catch (Exception e)// catch generation error
                {
                    Console.WriteLine(e);
                    sb.AppendLine(("Generation ERROR: " + e).SurroundWithComments());
                }
            }
            else
            {
                sb.AppendLine(("Analysis ERROR: " + AnalysisError).SurroundWithComments());
            }

            return sb.ToString();
        }

        #endregion

        #region Implementation of ICodeClassGeneratorDecorator

        public string FileName { get { return DecoratorInfo.DecoratorClassTypeShortName + ".g.cs"; } }

        public string ProjectFolder { get { return DecoratorInfo.DecoratedClassProjectFolder ?? ""; } }

        public string AnalysisError { get; set; }

        #endregion

        #region Private

        private string GenerateMethod(MethodInfo methodInfo)
        {
            var sb = new StringBuilder();

            var methodParameters = string.Join(",", methodInfo.Parameters.Select(p => p.Type + " " + p.Name));
            var methodParameterNames = string.Join(",", methodInfo.Parameters.Select(p => p.Name));
            var typesNotReturned = new[] {"void", "System.Threading.Tasks.Task"};
            var returnValue = typesNotReturned.All(t => t != methodInfo.ReturnTypeName);

            // Method declaration
            sb.AppendLine("public " + (methodInfo.IsAsync? "async ":"") + methodInfo.ReturnTypeName + " " + methodInfo.Name + "(" + methodParameters + ")");
            sb.AppendLine("{");

            if(returnValue)
            {
                var retType = methodInfo.IsAsync ? methodInfo.GetTaskRetunTypeName() : methodInfo.ReturnTypeName;
                sb.AppendLine("var ret = default("+ retType + ");");
            }

            // Try-catch
            if (DecoratorInfo.OnExceptionExist)
            {
                sb.AppendLine("try");
                sb.AppendLine("{");
            }

            // OnEntry method
            if (DecoratorInfo.OnEntryExist)
            {
                sb.AppendLine((methodInfo.IsAsync ? "await OnEntryAsync" : "OnEntry") + "(\"" + methodInfo.Name + "\", new object[] { " + methodParameterNames + " });");
            }

            if (returnValue)
            {
                sb.Append("ret = ");
            }
            sb.AppendLine((methodInfo.IsAsync ? "await " : " ") + _decoratedComponent + "." + methodInfo.Name + "(" + methodParameterNames + ");");

            // OnExit method
            if (DecoratorInfo.OnExitExist)
            {
                sb.AppendLine((methodInfo.IsAsync ? "await OnExitAsync" : "OnExit") + "("+ (returnValue ? "ret" : "")  + ");");
            }

            // Try-catch
            if (DecoratorInfo.OnExceptionExist)
            {
                sb.AppendLine("}");
                sb.AppendLine("catch(System.Exception e)");
                sb.AppendLine("{");
                sb.AppendLine("OnException(e);");
                sb.AppendLine("throw;");
                sb.AppendLine("}");

                // Finally
                if (DecoratorInfo.OnFinallyExist)
                {
                    sb.AppendLine("finally");
                    sb.AppendLine("{");
                    sb.AppendLine("OnFinally();");
                    sb.AppendLine("}");
                }
            }

            if (returnValue)
            {
                sb.AppendLine("return ret;");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        #endregion

    }
}