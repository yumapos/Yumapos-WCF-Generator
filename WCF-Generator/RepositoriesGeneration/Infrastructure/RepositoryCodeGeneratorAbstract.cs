using System;
using System.Text;
using WCFGenerator.RepositoriesGeneration.Core.SQL;
using WCFGenerator.RepositoriesGeneration.Enums;
using WCFGenerator.RepositoriesGeneration.Helpers;

namespace WCFGenerator.RepositoriesGeneration.Infrastructure
{
    internal abstract class RepositoryCodeGeneratorAbstract : ICodeClassGeneratorRepository
    {
        protected const string DataAccessControllerBaseRepositoryField = "DataAccessController";
        protected const string DataAccessServiceBaseRepositoryField = "DataAccessService";
        protected const string MaxRepositoryParamsBaseRepositoryField = "MaxRepositoryParams";
        protected const string MaxInsertManyRowsBaseRepositoryField = "MaxInsertManyRows";
        protected const string MaxUpdateManyRowsBaseRepositoryField = "MaxUpdateManyRows";
        

        #region Properties

        /// <summary>
        ///     Repository model information
        /// </summary>
        public RepositoryInfo RepositoryInfo { get; set; }
        
        /// <summary>
        ///     Repository model information
        /// </summary>
        public ISqlScriptGenerator ScriptGenerator { get; set; }

        /// <summary>
        ///     Allowed repository type
        /// </summary>
        public virtual RepositoryType RepositoryType
        {
            get { return RepositoryType.General; }
        }

        public virtual string RepositoryKindName
        {
            get { return ""; }
        }

        #endregion

        #region Implementation of ICodeClassGeneratorRepository

        public virtual string RepositoryName
        {
            get { return RepositoryInfo.RepositoryName; }
        }

        public virtual string FileName
        {
            get { return RepositoryInfo.ClassName + RepositoryKindName + RepositoryInfo.RepositorySuffix + ".g.cs"; }
        }

        public virtual string GetUsings()
        {
            var sb = new StringBuilder();

            foreach (var nameSpace in RepositoryInfo.RequiredNamespaces[RepositoryType.General])
            {
                sb.AppendLine("using "+ nameSpace + ";");
            }

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
            var constructor = $"public {RepositoryName}({RepositoryInfo.DataAccessServiceTypeName} dataAccessService, {RepositoryInfo.DataAccessControllerTypeName} dataAccessController) : base(dataAccessService, dataAccessController) {{ }}";
            return constructor;
        }

        public virtual string GetMethods()
        {
            return "";
        }

        public virtual string GetFullCode()
        {
            var sb = new StringBuilder();

            // check analysis error
            if (RepositoryInfo.CanBeGenerated)
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
                    sb.AppendLine(GetFields().TrimEnd('\n', '\r'));
                    sb.AppendLine(GetProperties().TrimEnd('\n', '\r'));
                    if((RepositoryType != RepositoryType.Cache && !RepositoryInfo.IsConstructorImplemented) || (RepositoryType == RepositoryType.Cache && !RepositoryInfo.IsCacheRepositoryConstructorImplemented))
                    {
                        sb.AppendLine(GetConstructors());
                    }
                    sb.AppendLine(GetMethods().TrimEnd('\n', '\r'));
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
                sb.AppendLine(("Analysis ERRORS: " + string.Join("\n\r", RepositoryInfo.RepositoryAnalysisError)).SurroundWithComments());
            }

            return sb.ToString();
        }

        public void InitScriptGenerator(DatabaseType type)
        {
            ScriptGenerator = type == DatabaseType.MSSql ? (ISqlScriptGenerator) new SqlScriptGenerator() : new SQLPostgresScriptGenerator();
        }

        #endregion
    }
}