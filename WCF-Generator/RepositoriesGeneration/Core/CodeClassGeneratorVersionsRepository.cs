using System;
using System.Text;
using VersionedRepositoryGeneration.Generator.Core.SQL;
using VersionedRepositoryGeneration.Generator.Heplers;
using VersionedRepositoryGeneration.Generator.Infrastructure;

namespace VersionedRepositoryGeneration.Generator.Core
{
    internal class CodeClassGeneratorVersionsRepository : BaseCodeClassGeneratorRepository
    {

        #region Overrides of BaseCodeClassGeneratorRepository

        public override string RepositoryName
        {
            get { return RepositoryInfo.ClassName + "Version" + RepositoryInfo.RepositorySuffix; }
        }

        #region Overrides of BaseCodeClassGeneratorRepository

        public override string GetClassDeclaration()
        {
            return "public partial class " + RepositoryName + " : RepositoryBase";
        }

        #endregion

        public override string GetUsings()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using YumaPos.Server.Data.Sql;");
            return sb.ToString();
        }

        public override string GetMethods()
        {
            var sb = new StringBuilder();

            #region Asynchronous method

            sb.AppendLine("public Guid Insert(" + RepositoryInfo.ClassFullName + " " + RepositoryInfo.ParameterName + ")");
            sb.AppendLine("{");

            sb.AppendLine("var InsertQuery = @" + SqlScriptGenerator.GenerateInsertToVersionTable(RepositoryInfo, RepositoryName).SurroundWithQuotes() + ";");
            sb.AppendLine("return (Guid)DataAccessService.InsertObject(" + RepositoryInfo.ParameterName + ", InsertQuery);");

            sb.AppendLine("}");

            #endregion

            #region Asynchronous method

            sb.AppendLine("public Task<Guid> InsertAsync(" + RepositoryInfo.ClassFullName + " " + RepositoryInfo.ParameterName + ")");
            sb.AppendLine("{");

            sb.AppendLine("var InsertQuery = @" + SqlScriptGenerator.GenerateInsertToVersionTable(RepositoryInfo, RepositoryName).SurroundWithQuotes() + ";");
            sb.AppendLine("return await (Guid)DataAccessService.InsertObjectAsync(" + RepositoryInfo.ParameterName + ", InsertQuery);");

            sb.AppendLine("}");

            #endregion

            return sb.ToString();
        }


        public override string GetFullCode()
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

        #endregion
    }
}