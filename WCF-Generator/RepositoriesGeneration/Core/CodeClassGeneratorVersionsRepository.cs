using System;
using System.Text;
using VersionedRepositoryGeneration.Generator.Core.SQL;
using VersionedRepositoryGeneration.Generator.Heplers;
using VersionedRepositoryGeneration.Generator.Infrastructure;

namespace VersionedRepositoryGeneration.Generator.Core
{
    internal class CodeClassGeneratorVersionsRepository : BaseCodeClassGeneratorRepository
    {
        public static string RepositoryKind = "Version";

        private string _insertQueryName = "InsertQuery";

        #region Overrides of BaseCodeClassGeneratorRepository

        public override string RepositoryName
        {
            get { return RepositoryInfo.ClassName + RepositoryKind + RepositoryInfo.RepositorySuffix; }
        }

        public override string GetClassDeclaration()
        {
            return "internal class " + RepositoryName + " : RepositoryBase";
        }

        public override string GetUsings()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine(base.GetUsings());
            return sb.ToString();
        }

        #region Overrides of BaseCodeClassGeneratorRepository

        public override string GetFields()
        {
            return "private const string " + _insertQueryName + " = @" + SqlScriptGenerator.GenerateInsertToVersionTable(RepositoryInfo, RepositoryName).SurroundWithQuotes() + ";";
        }

        #endregion

        public override string GetMethods()
        {
            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();

            var sb = new StringBuilder();

            #region Asynchronous method

            sb.AppendLine("public Guid Insert(" + RepositoryInfo.ClassFullName + " " + parameterName + ")");
            sb.AppendLine("{");
            sb.AppendLine("var res = DataAccessService.InsertObject(" + parameterName + ", " + _insertQueryName + ");");
            sb.AppendLine("return (Guid)res;");

            sb.AppendLine("}");

            #endregion

            #region Asynchronous method

            sb.AppendLine("public async Task<Guid> InsertAsync(" + RepositoryInfo.ClassFullName + " " + parameterName + ")");
            sb.AppendLine("{");
            sb.AppendLine("var res = await DataAccessService.InsertObjectAsync(" + parameterName + ", " + _insertQueryName + ");");
            sb.AppendLine("return (Guid)res;");

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
                    sb.AppendLine(GetFields());
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