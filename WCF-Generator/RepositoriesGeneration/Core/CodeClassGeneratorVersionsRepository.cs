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
            return "public class " + RepositoryName + " : RepositoryBase";
        }

        #endregion

        public override string GetUsings()
        {
            return "using System;";
        }

        public override string GetMethods()
        {
            var sb = new StringBuilder();

            sb.AppendLine("public Guid Insert(" + RepositoryInfo.ClassFullName + " " + RepositoryInfo.ParameterName + ")");
            sb.AppendLine("{");

            sb.AppendLine("var InsertQuery = @" + SqlScriptGenerator.GenerateInsertToVersionTable(RepositoryInfo, RepositoryName).SurroundWithQuotes() + ";");
            sb.AppendLine("return (Guid)DataAccessService.InsertObject(" + RepositoryInfo.ParameterName + ", InsertQuery);");

            sb.AppendLine("}");
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
                    sb.AppendLine(GetConstructors().SurroundWithRegion("Constructor"));
                    sb.AppendLine(GetMethods().SurroundWithRegion("Methods"));
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