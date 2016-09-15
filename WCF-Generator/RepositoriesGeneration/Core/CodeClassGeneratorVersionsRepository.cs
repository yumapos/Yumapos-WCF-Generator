using System;
using System.Linq;
using System.Text;
using VersionedRepositoryGeneration.Generator.Heplers;
using VersionedRepositoryGeneration.Generator.Infrastructure;
using WCFGenerator.RepositoriesGeneration.Core.SQL;

namespace VersionedRepositoryGeneration.Generator.Core
{
    internal class CodeClassGeneratorVersionsRepository : BaseCodeClassGeneratorRepository
    {
        public static string RepositoryKind = "Version";
        private string _insertQueryName = "InsertQuery";
        private string _whereQueryBy = "WhereQueryBy";
        private string _selectByQuery = "SelectByQuery";
        private string _andWithfilterData = "AndWithFilterData";



        #region Overrides of BaseCodeClassGeneratorRepository

        public override string RepositoryName
        {
            get { return RepositoryInfo.ClassName + RepositoryKind + RepositoryInfo.RepositorySuffix; }
        }

        public override RepositoryType RepositoryType
        {
            get { return RepositoryType.Version; }
        }

        public override string GetClassDeclaration()
        {
            return "internal class " + RepositoryName + " : RepositoryBase";
        }

        public override string GetUsings()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine(base.GetUsings());
            return sb.ToString();
        }

        #region Overrides of BaseCodeClassGeneratorRepository

        public override string GetFields()
        {
            // Common info for generate sql scriptes
            var sqlInfo = RepositoryInfo.RepositorySqlInfo;
            
            var insertQuery = SqlScriptGenerator.GenerateInsertToVersionTable(sqlInfo).SurroundWithQuotes();
            
            var sb = new StringBuilder();

            sb.AppendLine("private const string " + _insertQueryName + " = @" + insertQuery + ";");

            if(!RepositoryInfo.IsManyToMany)
            {
                // Version filter
                var selectByQuery = SqlScriptGenerator.GenerateSelectBy(sqlInfo).SurroundWithQuotes();
                var vk = RepositoryInfo.PossibleKeysForMethods.First(info =>info.FilterType == FilterType.VersionKey);
                var key = vk.Key;
                var parametrs = vk.Parameters.Select(p => p.Name).ToList();
                var sql = SqlScriptGenerator.GenerateWhere(parametrs, sqlInfo).SurroundWithQuotes();

                sb.AppendLine("private const string " + _selectByQuery + " = @" + selectByQuery + ";");
                sb.AppendLine("private const string " + _whereQueryBy + key + " = @" + sql + ";");

                // Is deleted filter
                if (RepositoryInfo.IsDeletedExist)
                {
                    var specialOption = RepositoryInfo.SpecialOptions.Parameters.Select(p => p.Name);
                    var andFilter = SqlScriptGenerator.GenerateAnd(specialOption, sqlInfo.JoinTableName ?? sqlInfo.TableName).SurroundWithQuotes();
                    sb.AppendLine("private const string " + _andWithfilterData + " = " + andFilter + ";");
                }
            }

            return sb.ToString();
        }

        #endregion

        public override string GetMethods()
        {
            var sb = new StringBuilder();

            // Insert
            var insert = GenerateInsert();
            if (!string.IsNullOrEmpty(insert))
                sb.AppendLine(insert);

            if(!RepositoryInfo.IsManyToMany)
            {
                // RepositoryMethod.GetBy - for version key
                var getByVersionKeyMethods = RepositoryInfo.MethodImplementationInfo
                    .Where(m => m.Method == RepositoryMethod.GetBy && m.FilterInfo.FilterType == FilterType.VersionKey)
                    .Aggregate("", (s, method) => s + GenerateGetByVersionKey(method));

                if (!string.IsNullOrEmpty(getByVersionKeyMethods))
                    sb.AppendLine(getByVersionKeyMethods);
            }

            return sb.ToString();

        }

        private string GenerateInsert()
        {
            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();

            var sb = new StringBuilder();

            // Asynchronous method
            sb.AppendLine("public void Insert(" + RepositoryInfo.ClassFullName + " " + parameterName + ")");
            sb.AppendLine("{");
            sb.AppendLine("DataAccessService.InsertObject(" + parameterName + ", " + _insertQueryName + ");");
            sb.AppendLine("}");

            // Synchronous method
            sb.AppendLine("public async Task InsertAsync(" + RepositoryInfo.ClassFullName + " " + parameterName + ")");
            sb.AppendLine("{");
            sb.AppendLine("await DataAccessService.InsertObjectAsync(" + parameterName + ", " + _insertQueryName + ");");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateGetByVersionKey(MethodImplementationInfo method)
        {
            var filter = method.FilterInfo;

            var methodParameters = string.Join(", ", filter.Parameters.Select(k => k.TypeName + " " + k.Name.FirstSymbolToLower()));
            var sqlParameters = string.Join(", ", filter.Parameters.Select(k => k.Name.FirstSymbolToLower()));
            var sqlWhere = _whereQueryBy + filter.Key;

            var specialParameter = RepositoryInfo.SpecialOptions.Parameters.First();
            var specialMethodParameter = specialParameter.TypeName + "? " + specialParameter.Name.FirstSymbolToLower() + " = " + specialParameter.DefaultValue;
            var specialSqlParameter = specialParameter.Name.FirstSymbolToLower();

            var sb = new StringBuilder();

            if (RepositoryInfo.IsDeletedExist)
            {
                // Synchronous method
                sb.AppendLine("public " + RepositoryInfo.ClassFullName + " GetBy" + filter.Key + "(" + methodParameters + ", " + specialMethodParameter + ")");
                sb.AppendLine("{");

                sb.AppendLine("object parameters = new {" + sqlParameters + "," + specialSqlParameter + "};");
                sb.AppendLine("var sql = " + _selectByQuery + " + " + sqlWhere + ";");
                sb.AppendLine("if (" + specialSqlParameter + ".HasValue)");
                sb.AppendLine("{");
                sb.AppendLine("sql = sql + " + _andWithfilterData + ";");
                sb.AppendLine("}");

                sb.AppendLine("var result = DataAccessService.Get<" + RepositoryInfo.ClassFullName + ">(sql, parameters);");
                sb.AppendLine("return result.FirstOrDefault();");
                sb.AppendLine("}");

                // Asynchronous method
                sb.AppendLine("public async Task<" + RepositoryInfo.ClassFullName + "> GetBy" + filter.Key + "Async" + "(" + methodParameters + ", " + specialMethodParameter + ")");
                sb.AppendLine("{");

                sb.AppendLine("object parameters = new {" + sqlParameters + "," + specialSqlParameter + "};");
                sb.AppendLine("var sql = " + _selectByQuery + " + " + sqlWhere + ";");
                sb.AppendLine("if (" + specialSqlParameter + ".HasValue)");
                sb.AppendLine("{");
                sb.AppendLine("sql = sql + " + _andWithfilterData + ";");
                sb.AppendLine("}");

                sb.AppendLine("var result = (await DataAccessService.GetAsync<" + RepositoryInfo.ClassFullName + ">(sql, parameters));");
                sb.AppendLine("return result.FirstOrDefault();");
                sb.AppendLine("}");
                sb.AppendLine("");
            }
            else
            {
                // Synchronous method
                sb.AppendLine("public " + RepositoryInfo.ClassFullName + " GetBy" + filter.Key + "(" + methodParameters + ")");
                sb.AppendLine("{");

                sb.AppendLine("object parameters = new {" + sqlParameters + "};");
                sb.AppendLine("var sql = " + _selectByQuery + " + " + sqlWhere + ";");

                sb.AppendLine("var result = DataAccessService.Get<" + RepositoryInfo.ClassFullName + ">(sql, parameters);");
                sb.AppendLine("return result.FirstOrDefault();");
                sb.AppendLine("}");

                // Asynchronous method
                sb.AppendLine("public async Task<" + RepositoryInfo.ClassFullName + "> GetBy" + filter.Key + "Async" + "(" + methodParameters + ")");
                sb.AppendLine("{");

                sb.AppendLine("object parameters = new {" + sqlParameters + "};");
                sb.AppendLine("var sql = " + _selectByQuery + " + " + sqlWhere + ";");

                sb.AppendLine("var result = (await DataAccessService.GetAsync<" + RepositoryInfo.ClassFullName + ">(sql, parameters));");
                sb.AppendLine("return result.FirstOrDefault();");
                sb.AppendLine("}");
                sb.AppendLine("");
            }

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
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