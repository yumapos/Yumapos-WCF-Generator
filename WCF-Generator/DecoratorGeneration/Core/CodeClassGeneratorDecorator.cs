using System;
using System.Linq;
using System.Text;
using WCFGenerator.DecoratorGeneration.Infrastructure;
using WCFGenerator.RepositoriesGeneration.Heplers;

namespace WCFGenerator.DecoratorGeneration.Core
{
    internal class CodeClassDecoratorGenerator : ICodeClassDecoratorGenerator
    {
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
            return "public partial class " + DecoratorInfo.DecoratorClassTypeShortName + " : " + string.Join("," , DecoratorInfo.ImplementedInterfaces);
        }

        public string GetFields()
        {
            return "";
        }

        public string GetProperties()
        {
            return "";
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

        public string AnalysisError { get; set; }

        #endregion

        #region Private

        private object GenerateMethod(MethodInfo methodInfo)
        {
            return "// " + methodInfo.Name + " " + "can be generate";
        }

        #endregion

    }
}