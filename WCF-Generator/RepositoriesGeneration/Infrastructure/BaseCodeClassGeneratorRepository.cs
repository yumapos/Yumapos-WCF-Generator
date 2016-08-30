using System;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using VersionedRepositoryGeneration.Generator.Heplers;

namespace VersionedRepositoryGeneration.Generator.Infrastructure
{
    internal abstract class BaseCodeClassGeneratorRepository : ICodeClassGeneratorRepository
    {
        #region Properties
        
        /// <summary>
        ///     Repository model information
        /// </summary>
        public RepositoryInfo RepositoryInfo { get; set; }

        #endregion

        #region Implementation of ICodeClassGeneratorRepository

        public virtual string RepositoryName
        {
            get { return RepositoryInfo.ClassName + RepositoryInfo.RepositorySuffix; }
        }

        public virtual string GetUsings()
        {
            var compilation = CSharpCompilation.Create("compilation").AddSyntaxTrees(RepositoryInfo.RepositoryInterface.SyntaxTree);
            var model = compilation.GetSemanticModel(RepositoryInfo.RepositoryInterface.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(RepositoryInfo.RepositoryInterface);
            var fullName = symbol.ToString();
            var nameSpace = fullName.Replace("." + RepositoryInfo.RepositoryInterface.Identifier.Text, "");

            var sb = new StringBuilder();
            sb.AppendLine("using YumaPos.Server.Data.Sql;");
            sb.AppendLine("using " + nameSpace + ";");

            return sb.ToString();
        }

        public virtual string GetNamespaceDeclaration()
        {
            return "namespace " + RepositoryInfo.RepositoriesNamespace;
        }

        public virtual string GetClassDeclaration()
        {
            var classDeclaration = string.Format("public{0}class {1} : {2}{3}",
                RepositoryInfo.CustomRepository != null ? " partial " : " ",
                RepositoryName,
                "RepositoryBase",
                RepositoryInfo.RepositoryInterface != null ? "," + RepositoryInfo.RepositoryInterface.Identifier.Text : "");
            return classDeclaration;
        }

        public virtual string GetFields()
        {
            return "";
        }

        public virtual string GetProperties()
        {
            return "";
        }

        public virtual string GetConstructors()
        {
            var constructor = string.Format("public {0}(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) {{ }}", RepositoryName);
            return constructor;
        }

        public virtual string GetMethods()
        {
            return "";
        }

        public virtual string GetFullCode()
        {
            var sb = new StringBuilder();

            // auto-generated header
            sb.AppendLine(GeneratorHelper.GetGeneratedDocumentHeader());

            // check analysis error
            if (RepositoryAnalysisError == null)
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
                    sb.AppendLine(GetConstructors());
                    sb.AppendLine(GetFields());
                    sb.AppendLine(GetProperties());
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
                sb.AppendLine(("Analysis ERROR: " + RepositoryAnalysisError).SurroundWithComments());
            }

            return sb.ToString();
        }

        public string RepositoryAnalysisError { get; set; }

      

        #endregion

        
    }
}