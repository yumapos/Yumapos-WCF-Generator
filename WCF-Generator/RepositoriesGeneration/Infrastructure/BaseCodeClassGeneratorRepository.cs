using System;
using System.Text;
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

        /// <summary>
        ///     Allowed repository type
        /// </summary>
        public virtual RepositoryType RepositoryType
        {
            get { return RepositoryType.General; }
        }

        #endregion

        #region Implementation of ICodeClassGeneratorRepository

        public virtual string RepositoryName
        {
            get { return RepositoryInfo.RepositoryName; }
        }

        public virtual string GetUsings()
        {
            var sb = new StringBuilder();

            sb.AppendLine("using YumaPos.Server.Data.Sql;");

            return sb.ToString();
        }

        public virtual string GetNamespaceDeclaration()
        {
            return "namespace " + RepositoryInfo.RepositoryNamespace;
        }

        public virtual string GetClassDeclaration()
        {
            return "";
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
                    sb.AppendLine(GetFields());
                    sb.AppendLine(GetProperties());
                    if((RepositoryType != RepositoryType.Cache && !RepositoryInfo.IsConstructorImplemented) || (RepositoryType == RepositoryType.Cache && !RepositoryInfo.IsCacheRepositoryConstructorImplemented))
                    {
                        sb.AppendLine(GetConstructors());
                    }
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

    internal enum RepositoryType
    {
        General,
        Cache,
        Version,
        VersionService
    }


}