using System.Linq;
using System.Text;
using WCFGenerator.RepositoriesGeneration.Core.SQL;
using WCFGenerator.RepositoriesGeneration.Heplers;
using WCFGenerator.RepositoriesGeneration.Infrastructure;

namespace WCFGenerator.RepositoriesGeneration.Core
{
    internal class CodeClassGeneratorRepository : BaseCodeClassGeneratorRepository
    {
        #region Query field names

        private string _selectAllQuery = "SelectAllQuery";
        private string _selectByQuery = "SelectByQuery";
        private string _selectIntoTemp = "SelectIntoTempTable";

        private string _insertQuery = "InsertQuery";

        private string _updateQuery = "UpdateQuery";
        private string _updateQueryBy = "UpdateQueryBy";

        private string _deleteQueryBy = "DeleteQueryBy";
        private string _whereQueryBy = "WhereQueryBy";
        private string _andWithIsDeletedFilter = "AndWithIsDeletedFilter";
        private string _andWithSliceDateFilter = "AndWithSliceDateFilter";
        private string _join = "Join";
        private string _pk = "Pk";

        #endregion

        #region Overrides of BaseCodeClassGeneratorRepository

        public override string GetClassDeclaration()
        {
            return "public partial class " + RepositoryName + " : " + RepositoryInfo.RepositoryBaseTypeName + ", " + RepositoryInfo.RepositoryInterfaceName;
        }

        #region Overrides of BaseCodeClassGeneratorRepository

        public override string GetUsings()
        {
            var sb = new StringBuilder(base.GetUsings());

            foreach (var nameSpace in RepositoryInfo.RequiredNamespaces[RepositoryType.Cache])
            {
                sb.AppendLine("using "+ nameSpace + ";");
            }

            return sb.ToString();
        }

        #endregion

        public override string GetFields()
        {
            var sb = new StringBuilder();

            // Common info for generate sql scriptes
            var sqlInfo = RepositoryInfo.RepositorySqlInfo;

            var fields = SqlScriptGenerator.GenerateFields(sqlInfo);
            var values = SqlScriptGenerator.GenerateValues(sqlInfo);
            var selectAllQuery = SqlScriptGenerator.GenerateSelectAll(sqlInfo).SurroundWithQuotes();
            var selectByQuery = SqlScriptGenerator.GenerateSelectBy(sqlInfo, null).SurroundWithQuotes();
            var insertQuery = SqlScriptGenerator.GenerateInsert(sqlInfo).SurroundWithQuotes();
            var updateBy = SqlScriptGenerator.GenerateUpdate(sqlInfo).SurroundWithQuotes();
            var deleteBy = SqlScriptGenerator.GenerateRemove(sqlInfo).SurroundWithQuotes();

            sb.AppendLine("private const string Fields = @" + fields.SurroundWithQuotes() + ";");
            //sb.AppendLine("private const string Values = @" + values.SurroundWithQuotes() + ";");
            sb.AppendLine("private const string " + _selectAllQuery + " = @" + selectAllQuery + ";");
            sb.AppendLine("private const string " + _selectByQuery + " = @" + selectByQuery + ";");
            sb.AppendLine("private const string " + _insertQuery + " = @" + insertQuery + ";");
            sb.AppendLine("private const string " + _updateQueryBy + " = @" + updateBy + ";");
            sb.AppendLine("private const string " + _deleteQueryBy + " = @" + deleteBy + ";");


            if (RepositoryInfo.JoinRepositoryInfo != null)
            {
                var updateJoin = SqlScriptGenerator.GenerateUpdateJoin(sqlInfo).SurroundWithQuotes();
                sb.AppendLine("private const string " + _updateQuery + _join + " = " + updateJoin + ";");

                var selectIntoTemp = SqlScriptGenerator.GenerateInsertToTemp(sqlInfo).SurroundWithQuotes();
                sb.AppendLine("private const string " + _selectIntoTemp + " = @" + selectIntoTemp + ";");
            }

            foreach (var method in RepositoryInfo.PossibleKeysForMethods)
            {
                var key = method.Key;
                var parametrs = method.Parameters.Select(p => p.Name).ToList();
                var sql = SqlScriptGenerator.GenerateWhere(parametrs, sqlInfo).SurroundWithQuotes();
                sb.AppendLine("private const string " + _whereQueryBy + key + " = " + sql + ";");
                
            }
            // where by join PK
            if (RepositoryInfo.JoinRepositoryInfo!=null)
            {
                var sqlJoin = SqlScriptGenerator.GenerateWhereJoinPk(sqlInfo).SurroundWithQuotes();
                sb.AppendLine("private const string " + _whereQueryBy + _join + _pk + " = " + sqlJoin + ";");
            }
            // Is deleted filter
            if (RepositoryInfo.IsDeletedExist)
            {
                var specialOption = RepositoryInfo.SpecialOptionsIsDeleted.Parameters.First().Name;
                var filter = SqlScriptGenerator.GenerateAnd(specialOption, sqlInfo.JoinTableName ?? sqlInfo.TableName);
                sb.AppendLine("private const string " + _andWithIsDeletedFilter + " = " + filter.SurroundWithQuotes() + ";");
            }

            return sb.ToString();
        }

        public override string GetMethods()
        {
            var sb = new StringBuilder();

            // RepositoryMethod.GetAll
            var getAllMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.GetAll)
                .Aggregate("", (s, method) => s + GenerateGetAll(method));

            if (!string.IsNullOrEmpty(getAllMethods))
                sb.AppendLine(getAllMethods);

            // RepositoryMethod.GetBy - for primary key
            var getByPrimaryKeyMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.GetBy && (m.FilterInfo.FilterType == FilterType.PrimaryKey))
                .Aggregate("", (s, method) => s + GenerateGetByPrimaryKey(method));

            if (!string.IsNullOrEmpty(getByPrimaryKeyMethods))
                sb.AppendLine(getByPrimaryKeyMethods);

            // RepositoryMethod.GetBy - for filter key
            var getByFilterKeyMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.GetBy && m.FilterInfo.FilterType == FilterType.FilterKey)
                .Aggregate("", (s, method) => s + GenerateGetByFilterKey(method));

            if (!string.IsNullOrEmpty(getByFilterKeyMethods))
                sb.AppendLine(getByFilterKeyMethods);

            // RepositoryMethod.GetBy - for version key
            var getByVersionKeyMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.GetBy && m.FilterInfo.FilterType == FilterType.VersionKey)
                .Aggregate("", (s, method) => s + GenerateGetByVersionKey(method));

            if (!string.IsNullOrEmpty(getByVersionKeyMethods))
                sb.AppendLine(getByVersionKeyMethods);

            // RepositoryMethod.Insert
            var insertMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.Insert)
                .Aggregate("", (s, method) => s + GenerateInsert(method));

            if (!string.IsNullOrEmpty(insertMethods))
                sb.AppendLine(insertMethods);

            // RepositoryMethod.UpdateBy
            var updateByMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.UpdateBy && m.FilterInfo.FilterType != FilterType.VersionKey)
                .Aggregate("", (s, method) => s + GenerateUpdate(method));

            if (!string.IsNullOrEmpty(updateByMethods))
                sb.AppendLine(updateByMethods);

            // RepositoryMethod.RemoveBy
            var removeByMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.RemoveBy && m.FilterInfo.FilterType != FilterType.VersionKey)
                .Aggregate("", (s, method) => s + GenerateRemove(method));

            if (!string.IsNullOrEmpty(removeByMethods))
                sb.AppendLine(removeByMethods);

            return sb.ToString();
        }

        #endregion

        #region Private

        private string GenerateGetAll(MethodImplementationInfo method)
        {
            var addIsDeletedFilter = RepositoryInfo.IsDeletedExist;
            var parameters =  "";
            var parameterNames = "";

            if(addIsDeletedFilter)
            {
                var specialParameterIsDeleted = RepositoryInfo.SpecialOptionsIsDeleted.Parameters.First();
                parameters = specialParameterIsDeleted.TypeName + "? " + specialParameterIsDeleted.Name.FirstSymbolToLower() + " = " + specialParameterIsDeleted.DefaultValue;
                parameterNames = specialParameterIsDeleted.Name.FirstSymbolToLower();
            }

            var returnType = "IEnumerable<" + RepositoryInfo.ClassFullName + ">";

            var sb = new StringBuilder();

            // Synchronous method
            sb.AppendLine("public " + returnType + " GetAll(" + parameters + ")");
            sb.AppendLine("{");
            sb.AppendLine("var sql = " + _selectAllQuery + ";");

            if (addIsDeletedFilter)
            {
                var parameter = RepositoryInfo.SpecialOptionsIsDeleted.Parameters.First().Name.FirstSymbolToLower();
                sb.AppendLine("object parameters = new {" + parameterNames + "};");
                sb.AppendLine("if (" + parameter + ".HasValue)");
                sb.AppendLine("{");
                sb.AppendLine("sql = sql + " + _andWithIsDeletedFilter + ";");
                sb.AppendLine("}");
            }
            else
            {
                sb.AppendLine("object parameters = null;");
            }

            sb.AppendLine("var result = DataAccessService.Get<" + RepositoryInfo.ClassFullName + ">(sql, parameters).ToList();");
            sb.AppendLine("return result.ToList();");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task<" + returnType + "> GetAllAsync(" + parameters + ")");
            sb.AppendLine("{");
            sb.AppendLine("var sql = " + _selectAllQuery + ";");

            if(addIsDeletedFilter)
            {
                var parameter = RepositoryInfo.SpecialOptionsIsDeleted.Parameters.First().Name.FirstSymbolToLower();
                sb.AppendLine("object parameters = new {" + parameterNames + "};");
                sb.AppendLine("if (" + parameter + ".HasValue)");
                sb.AppendLine("{");
                sb.AppendLine("sql = sql + " + _andWithIsDeletedFilter + ";");
                sb.AppendLine("}");
            }
            else
            {
                sb.AppendLine("object parameters = null;");
            }

            sb.AppendLine("var result = (await DataAccessService.GetAsync<" + RepositoryInfo.ClassFullName + ">(sql, parameters));");
            sb.AppendLine("return result.ToList();");
            sb.AppendLine("}");

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateGetByVersionKey(MethodImplementationInfo method)
        {
            var code = GenerateGetByKey(method);
            return method.RequiresImplementation ? code : code.SurroundWithComments();
        }

        private string GenerateGetByPrimaryKey(MethodImplementationInfo method)
        {
            var code = GenerateGetByKey(method);
            return method.RequiresImplementation ? code : code.SurroundWithComments();
        }

        private string GenerateGetByFilterKey(MethodImplementationInfo method)
        {
            var code = GenerateGetByKey(method);
            return method.RequiresImplementation ? code : code.SurroundWithComments();
        }

        private string GenerateGetByKey(MethodImplementationInfo method)
        {
            var returnType = method.ReturnType.IsEnumerable() ? "IEnumerable<" + RepositoryInfo.ClassFullName + ">" : RepositoryInfo.ClassFullName;
            var returnFunc = method.ReturnType.IsEnumerable() ? "return result.ToList();" : "return result.FirstOrDefault();";
            var filter = method.FilterInfo;
            var filterByIsDeleted = RepositoryInfo.IsDeletedExist;
            var sqlWhere = _whereQueryBy + filter.Key;
            var selectQuery = _selectByQuery;

            var parameters = filter.Parameters.Select(k => k.TypeName + " " + k.Name.FirstSymbolToLower()).ToList();
            var parameterNames = filter.Parameters.Select(k => k.Name.FirstSymbolToLower()).ToList();

            // last parameter - because have default value
            if (filterByIsDeleted)
            {
                var specialParameterIsDeleted = RepositoryInfo.SpecialOptionsIsDeleted.Parameters.First();
                var specialMethodParameterIsDeleted = specialParameterIsDeleted.TypeName + "? " + specialParameterIsDeleted.Name.FirstSymbolToLower() + " = " + specialParameterIsDeleted.DefaultValue;
                var specialMethodParameterIsDeletedName = specialParameterIsDeleted.Name.FirstSymbolToLower();

                parameters.Add(specialMethodParameterIsDeleted);
                parameterNames.Add(specialMethodParameterIsDeletedName);
            }

            var methodParameters = string.Join(", ", parameters);
            var methodParameterNames = string.Join(", ", parameterNames);


            var sb = new StringBuilder();

            // Synchronous method
            sb.AppendLine("public " + returnType + " GetBy" + filter.Key + "(" + methodParameters + ")");
            sb.AppendLine("{");

            sb.AppendLine("object parameters = new {" + methodParameterNames + "};");
            sb.AppendLine("var sql = " + selectQuery + " + " + sqlWhere + ";");
            if (filterByIsDeleted)
            {
                var parameter = RepositoryInfo.SpecialOptionsIsDeleted.Parameters.First().Name.FirstSymbolToLower();
                sb.AppendLine("if (" + parameter + ".HasValue)");
                sb.AppendLine("{");
                sb.AppendLine("sql = sql + " + _andWithIsDeletedFilter + ";");
                sb.AppendLine("}");
            }
            sb.AppendLine("var result = DataAccessService.Get<" + RepositoryInfo.ClassFullName + ">(sql, parameters);");
            sb.AppendLine(returnFunc);
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task<" + returnType + "> GetBy" + filter.Key + "Async" + "(" + methodParameters + ")");
            sb.AppendLine("{");

            sb.AppendLine("object parameters = new {" + methodParameterNames + "};");
            sb.AppendLine("var sql = " + selectQuery + " + " + sqlWhere + ";");
            if (filterByIsDeleted)
            {
                var parameter = RepositoryInfo.SpecialOptionsIsDeleted.Parameters.First().Name.FirstSymbolToLower();
                sb.AppendLine("if (" + parameter + ".HasValue)");
                sb.AppendLine("{");
                sb.AppendLine("sql = sql + " + _andWithIsDeletedFilter + ";");
                sb.AppendLine("}");
            }
            sb.AppendLine("var result = (await DataAccessService.GetAsync<" + RepositoryInfo.ClassFullName + ">(sql, parameters));");
            sb.AppendLine(returnFunc);
            sb.AppendLine("}");
            sb.AppendLine("");

            return sb.ToString();
        }

        private string GenerateInsert(MethodImplementationInfo method)
        {
            var sb = new StringBuilder();

            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var methodParameter = RepositoryInfo.ClassFullName + " " + parameterName;

            var queryName = _insertQuery;

            // If should not return identifier
            if (RepositoryInfo.PrimaryKeys.Count != 1)
            {
                // Synchronous method
                sb.AppendLine("public void Insert(" + methodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine("DataAccessService.InsertObject(" + parameterName + "," + queryName + ");");
                sb.AppendLine("}");

                // Asynchronous method
                sb.AppendLine("public async Task InsertAsync(" + methodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine("await DataAccessService.InsertObjectAsync(" + parameterName + "," + queryName + ");");
                sb.AppendLine("}");
            }
            else
            {
                var returnType = RepositoryInfo.PrimaryKeys.First().TypeName;

                // Synchronous method
                sb.AppendLine("public " + returnType + " Insert(" + methodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine("var res = DataAccessService.InsertObject(" + parameterName + "," + queryName + ");");
                sb.AppendLine("return (" + returnType + ")res;");
                sb.AppendLine("}");

                // Asynchronous method
                sb.AppendLine("public async Task<" + returnType + "> InsertAsync(" + methodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine("var res = await DataAccessService.InsertObjectAsync(" + parameterName + "," + queryName + ");");
                sb.AppendLine("return (" + returnType + ")res;");
                sb.AppendLine("}");
            }

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateUpdate(MethodImplementationInfo method)
        {
            var filter = method.FilterInfo;

            var sb = new StringBuilder();

            // Update by entity (use filter key)
            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var methodParameter = RepositoryInfo.ClassFullName + " " + parameterName;

            var whereQueryName = _whereQueryBy + filter.Key;
            var whereQueryByJoinPk = _whereQueryBy + _join + _pk;

            // Synchronous method
            sb.AppendLine("public void UpdateBy" + filter.Key + "(" + methodParameter + ")");
            sb.AppendLine("{");
            if (RepositoryInfo.JoinRepositoryInfo == null)
            {
                sb.AppendLine("var sql = " + _updateQueryBy + " + " + whereQueryName + "; ");
            }
            else
            {
                sb.AppendLine("var sql = " + _updateQueryBy + " + " + whereQueryName +  " + " + _updateQuery + _join + " + " + whereQueryByJoinPk + "; ");
            }
            sb.AppendLine("DataAccessService.PersistObject(" + parameterName + ", sql);");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task UpdateBy" + filter.Key + "Async(" + methodParameter + ")");
            sb.AppendLine("{");
            if (RepositoryInfo.JoinRepositoryInfo == null)
            {
                sb.AppendLine("var sql = " + _updateQueryBy + " + " + whereQueryName + "; ");
            }
            else
            {
                sb.AppendLine("var sql = " + _updateQueryBy + " + " + whereQueryName + " + " + _updateQuery + _join + " + " + whereQueryByJoinPk + "; ");
            }
            sb.AppendLine("await DataAccessService.PersistObjectAsync(" + parameterName + ", sql);");
            sb.AppendLine("}");
            sb.AppendLine("");


            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateRemove(MethodImplementationInfo method)
        {
            var filter = method.FilterInfo;

            var sb = new StringBuilder();

            // Remove by entity (use filter key)
            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var methodParameter = RepositoryInfo.ClassFullName + " " + parameterName;
            
            var whereQueryName = _whereQueryBy + filter.Key;

            // Synchronous method
            sb.AppendLine("public void RemoveBy" + filter.Key + "(" + methodParameter + ")");
            sb.AppendLine("{");
            if (RepositoryInfo.JoinRepositoryInfo == null)
            {
                sb.AppendLine("var sql = " + _deleteQueryBy + " + " + whereQueryName + "; ");
            }
            else
            {
                sb.AppendLine("var sql = " + _selectIntoTemp + " + " + whereQueryName + " + " + _deleteQueryBy + "; ");
            }
            sb.AppendLine("DataAccessService.PersistObject("+ parameterName + ", sql);");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task RemoveBy" + filter.Key + "Async(" + methodParameter + ")");
            sb.AppendLine("{");
            if (RepositoryInfo.JoinRepositoryInfo == null)
            {
                sb.AppendLine("var sql = " + _deleteQueryBy + " + " + whereQueryName + "; ");
            }
            else
            {
                sb.AppendLine("var sql = " + _selectIntoTemp + " + " + whereQueryName + " + " + _deleteQueryBy + "; ");
            }
            sb.AppendLine("await DataAccessService.PersistObjectAsync(" + parameterName + ", sql);");
            sb.AppendLine("}");
            sb.AppendLine("");

            // Remove by filter key
            var methodParameters = string.Join(", ", filter.Parameters.Select(k => k.TypeName + " " + k.Name.FirstSymbolToLower()));
            var sqlParameters = string.Join(", ", filter.Parameters.Select(k => k.Name.FirstSymbolToLower()));

            // Synchronous method
            sb.AppendLine("public void RemoveBy" + filter.Key + "(" + methodParameters + ")");
            sb.AppendLine("{");
            sb.AppendLine("object parameters = new {" + sqlParameters + "};");
            if (RepositoryInfo.JoinRepositoryInfo == null)
            {
                sb.AppendLine("var sql = " + _deleteQueryBy + " + " + whereQueryName + "; ");
            }
            else
            {
                sb.AppendLine("var sql = " + _selectIntoTemp + " + " + whereQueryName + " + " + _deleteQueryBy + "; ");
            }
            sb.AppendLine("DataAccessService.PersistObject<" + RepositoryInfo.ClassFullName + ">(sql, parameters);");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task RemoveBy" + filter.Key + "Async(" + methodParameters + ")");
            sb.AppendLine("{");
            sb.AppendLine("object parameters = new {" + sqlParameters + "};");
            if (RepositoryInfo.JoinRepositoryInfo == null)
            {
                sb.AppendLine("var sql = " + _deleteQueryBy + " + " + whereQueryName + "; ");
            }
            else
            {
                sb.AppendLine("var sql = " + _selectIntoTemp + " + " + whereQueryName + " + " + _deleteQueryBy + "; ");
            }
            sb.AppendLine("await DataAccessService.PersistObjectAsync<" + RepositoryInfo.ClassFullName + ">(sql, parameters);");
            sb.AppendLine("}");
            sb.AppendLine("");

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        #endregion
    }
}