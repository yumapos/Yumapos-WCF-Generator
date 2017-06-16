using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCFGenerator.RepositoriesGeneration.Helpers;
using WCFGenerator.RepositoriesGeneration.Infrastructure;

namespace WCFGenerator.RepositoriesGeneration.Core
{
    internal class RepositoryCodeGenerator : RepositoryCodeGeneratorAbstract
    {
        #region Query field names

        private readonly string _selectAllQuery = "SelectAllQuery";
        private readonly string _selectByQuery = "SelectByQuery";
        private readonly string _selectIntoTemp = "SelectIntoTempTable";

        private readonly string _insertQuery = "InsertQuery";

        private readonly string _updateQuery = "UpdateQuery";
        private readonly string _updateQueryBy = "UpdateQueryBy";

        private readonly string _deleteQueryBy = "DeleteQueryBy";
        private readonly string _whereQueryBy = "WhereQueryBy";
        private readonly string _andWithIsDeletedFilter = "AndWithIsDeletedFilter";
        private readonly string _whereWithIsDeletedFilter = "WhereWithIsDeletedFilter";
        private string _andWithSliceDateFilter = "AndWithSliceDateFilter";
        private readonly string _join = "Join";
        private readonly string _pk = "Pk";
        private string _insertOrUpdateQuery = "InsertOrUpdateQuery";

        private const string DataAccessControllerField = "_dataAccessController";
        private const string DateTimeServiceField = "_dateTimeService";

        #endregion

        #region Overrides of RepositoryCodeGeneratorAbstract

        public override string GetConstructors()
        {
            if(RepositoryInfo.IsInsertModified)
            {
                var constructorParamers = new List<string>
                {
                    RepositoryInfo.DataAccessServiceTypeName + " dataAccessService",
                    RepositoryInfo.DataAccessControllerTypeName + " dataAccessController",
                    RepositoryInfo.DateTimeServiceTypeName + " dateTimeService"
                };
                var parameters = string.Join(",\r\n", constructorParamers);

                var sb = new StringBuilder();

                sb.Append("public " + RepositoryName + "(");
                sb.Append(parameters);
                sb.AppendLine(") : base(dataAccessService)");
                sb.AppendLine("{");
                sb.AppendLine(DataAccessControllerField + " = " + "dataAccessController;");
                sb.AppendLine(DateTimeServiceField + " = " + "dateTimeService;");
                sb.AppendLine("}");

                return sb.ToString();
            }
            return base.GetConstructors();
        }

        public override string GetClassDeclaration()
        {
            return "public partial class " + RepositoryName + " : " + RepositoryInfo.RepositoryBaseTypeName + ", " + RepositoryInfo.RepositoryInterfaceName;
        }

        public override string GetUsings()
        {
            var sb = new StringBuilder(base.GetUsings());

            foreach (var nameSpace in RepositoryInfo.RequiredNamespaces[RepositoryType.Cache])
            {
                sb.AppendLine("using " + nameSpace + ";");
            }

            return sb.ToString();
        }

        public override string GetFields()
        {
            var sb = new StringBuilder();

            InitScriptGenerator(RepositoryInfo.DatabaseType);

            // Common info for generate sql scriptes
            var sqlInfo = ScriptGenerator.GetTableInfo(RepositoryInfo.RepositorySqlInfo);

            var fields = ScriptGenerator.GenerateFields(sqlInfo);
            var selectAllQuery = ScriptGenerator.GenerateSelectAll(sqlInfo).SurroundWithQuotes();
            var selectByQuery = ScriptGenerator.GenerateSelectBy(sqlInfo, null).SurroundWithQuotes();
            var insertQuery = ScriptGenerator.GenerateInsert(sqlInfo).SurroundWithQuotes();
            var updateBy = ScriptGenerator.GenerateUpdate(sqlInfo).SurroundWithQuotes();
            var deleteBy = ScriptGenerator.GenerateRemove(sqlInfo).SurroundWithQuotes();
            var insertOrUpdate = ScriptGenerator.GenerateInsertOrUpdate(sqlInfo).SurroundWithQuotes();

            sb.AppendLine("private const string Fields = @" + fields.SurroundWithQuotes() + ";");
            sb.AppendLine("private const string " + _selectAllQuery + " = @" + selectAllQuery + ";");
            sb.AppendLine("private const string " + _selectByQuery + " = @" + selectByQuery + ";");
            sb.AppendLine("private const string " + _insertQuery + " = @" + insertQuery + ";");
            sb.AppendLine("private const string " + _updateQueryBy + " = @" + updateBy + ";");
            sb.AppendLine("private const string " + _deleteQueryBy + " = @" + deleteBy + ";");
            if(RepositoryInfo.JoinRepositoryInfo == null)
            {
                sb.AppendLine("private const string " + _insertOrUpdateQuery + " = @" + insertOrUpdate + ";");
            }

            if(RepositoryInfo.JoinRepositoryInfo != null)
            {
                var updateJoin = ScriptGenerator.GenerateUpdateJoin(sqlInfo).SurroundWithQuotes();
                sb.AppendLine("private const string " + _updateQuery + _join + " = " + updateJoin + ";");

                var selectIntoTemp = ScriptGenerator.GenerateInsertToTemp(sqlInfo).SurroundWithQuotes();
                sb.AppendLine("private const string " + _selectIntoTemp + " = @" + selectIntoTemp + ";");
            }

            foreach (var method in RepositoryInfo.PossibleKeysForMethods)
            {
                var key = method.Key;
                var parametrs = method.Parameters.Where(x => x.NeedGeneratePeriod == false).Select(p => p.Name).ToList();
                var datesParams = method.Parameters.Where(x => x.NeedGeneratePeriod).Select(p => p.Name);
                var sql = datesParams.Any() ? ScriptGenerator.GenerateWhere(parametrs, datesParams, sqlInfo).SurroundWithQuotes() : ScriptGenerator.GenerateWhere(parametrs, sqlInfo).SurroundWithQuotes();
                sb.AppendLine("private const string " + _whereQueryBy + key + " = " + sql + ";");
            }

            // where by join PK
            if (RepositoryInfo.JoinRepositoryInfo != null)
            {
                var sqlJoin = ScriptGenerator.GenerateWhereJoinPk(sqlInfo).SurroundWithQuotes();
                sb.AppendLine("private const string " + _whereQueryBy + _join + _pk + " = " + sqlJoin + ";");
            }
            // Is deleted filter
            if(RepositoryInfo.IsDeletedExist)
            {
                var specialOption = RepositoryInfo.SpecialOptionsIsDeleted.Parameters.First().Name;
                var filter = ScriptGenerator.GenerateAnd(specialOption, sqlInfo.JoinTableName ?? sqlInfo.TableName);
                sb.AppendLine("private const string " + _andWithIsDeletedFilter + " = " + filter.SurroundWithQuotes() + ";");
                var isDeletedOnlyFilter = ScriptGenerator.GenerateWhere(new List<string> {specialOption}, sqlInfo);
                sb.AppendLine("private const string " + _whereWithIsDeletedFilter + " = " + isDeletedOnlyFilter.SurroundWithQuotes() + ";");
            }

            if (RepositoryInfo.IsInsertModified)
            {
                sb.AppendLine();
                sb.AppendLine("private " + RepositoryInfo.DataAccessControllerTypeName + " " + DataAccessControllerField + ";");
                sb.AppendLine("private " + RepositoryInfo.DateTimeServiceTypeName + " " + DateTimeServiceField + ";");
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

            if(!string.IsNullOrEmpty(getAllMethods))
            {
                sb.AppendLine(getAllMethods);
            }

            // RepositoryMethod.GetBy - for primary key
            var getByPrimaryKeyMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.GetBy && m.FilterInfo.FilterType == FilterType.PrimaryKey)
                .Aggregate("", (s, method) => s + GenerateGetByPrimaryKey(method));

            if(!string.IsNullOrEmpty(getByPrimaryKeyMethods))
            {
                sb.AppendLine(getByPrimaryKeyMethods);
            }

            // RepositoryMethod.GetBy - for filter key
            var getByFilterKeyMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.GetBy && m.FilterInfo.FilterType == FilterType.FilterKey)
                .Aggregate("", (s, method) => s + GenerateGetByFilterKey(method));

            if(!string.IsNullOrEmpty(getByFilterKeyMethods))
            {
                sb.AppendLine(getByFilterKeyMethods);
            }

            // RepositoryMethod.GetBy - for version key
            var getByVersionKeyMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.GetBy && m.FilterInfo.FilterType == FilterType.VersionKey)
                .Aggregate("", (s, method) => s + GenerateGetByVersionKey(method));

            if(!string.IsNullOrEmpty(getByVersionKeyMethods))
            {
                sb.AppendLine(getByVersionKeyMethods);
            }

            // RepositoryMethod.Insert
            string insertMethods;
            if(RepositoryInfo.IsInsertModified)
            {
                insertMethods = RepositoryInfo.MethodImplementationInfo
                    .Where(m => m.Method == RepositoryMethod.Insert)
                    .Aggregate("", (s, method) => s + GenerateInsertWithModified(method));
            }
            else
            {
                insertMethods = RepositoryInfo.MethodImplementationInfo
                    .Where(m => m.Method == RepositoryMethod.Insert)
                    .Aggregate("", (s, method) => s + GenerateInsert(method));
            }

            if(!string.IsNullOrEmpty(insertMethods))
            {
                sb.AppendLine(insertMethods);
            }

            var filtersWithoutDateTimes = new List<MethodImplementationInfo>();
            foreach (var impl in RepositoryInfo.MethodImplementationInfo)
            {
                if(impl.FilterInfo != null && impl.FilterInfo.Parameters.Count(t => t.NeedGeneratePeriod) == 0)
                {
                    filtersWithoutDateTimes.Add(impl);
                }
            }
            // RepositoryMethod.UpdateBy
            var updateByMethods = filtersWithoutDateTimes
                .Where(m => m.Method == RepositoryMethod.UpdateBy && m.FilterInfo.FilterType != FilterType.VersionKey)
                .Aggregate("", (s, method) => s + GenerateUpdate(method));

            if(!string.IsNullOrEmpty(updateByMethods))
            {
                sb.AppendLine(updateByMethods);
            }

            // RepositoryMethod.RemoveBy
            var removeByMethods = filtersWithoutDateTimes
                .Where(m => m.Method == RepositoryMethod.RemoveBy && m.FilterInfo.FilterType != FilterType.VersionKey)
                .Aggregate("", (s, method) => s + GenerateRemove(method));

            if(!string.IsNullOrEmpty(removeByMethods))
            {
                sb.AppendLine(removeByMethods);
            }

            if (RepositoryInfo.JoinRepositoryInfo == null)
            {
                // RepositoryMethod.InsertOrUpdate
                var upsertMethods = RepositoryInfo.MethodImplementationInfo
                    .Where(m => m.Method == RepositoryMethod.InsertOrUpdate)
                    .Aggregate("", (s, method) => s + GenerateInsertOrUpdate(method));

                if (!string.IsNullOrEmpty(upsertMethods))
                {
                    sb.AppendLine(upsertMethods);
                }
            }

            return sb.ToString();
        }

        #endregion

        #region Generation

        public string GenerateGetAll(MethodImplementationInfo method)
        {
            var addIsDeletedFilter = RepositoryInfo.IsDeletedExist;
            var parameters = "";
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

            if(addIsDeletedFilter)
            {
                var parameter = RepositoryInfo.SpecialOptionsIsDeleted.Parameters.First().Name.FirstSymbolToLower();
                sb.AppendLine("object parameters = new {" + parameterNames + "};");
                sb.AppendLine("if (" + parameter + ".HasValue)");
                sb.AppendLine("{");
                sb.AppendLine("sql = sql + " + (RepositoryInfo.IsTenantRelated ? _andWithIsDeletedFilter : _whereWithIsDeletedFilter) + ";");
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
                sb.AppendLine("sql = sql + " + (RepositoryInfo.IsTenantRelated ? _andWithIsDeletedFilter : _whereWithIsDeletedFilter) + ";");
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

        public string GenerateGetByVersionKey(MethodImplementationInfo method)
        {
            var code = GenerateGetByKey(method);
            return method.RequiresImplementation ? code : code.SurroundWithComments();
        }

        public string GenerateGetByPrimaryKey(MethodImplementationInfo method)
        {
            var code = GenerateGetByKey(method);
            return method.RequiresImplementation ? code : code.SurroundWithComments();
        }

        public string GenerateGetByFilterKey(MethodImplementationInfo method)
        {
            var code = GenerateGetByKey(method);
            return method.RequiresImplementation ? code : code.SurroundWithComments();
        }

        public string GenerateGetByKey(MethodImplementationInfo method)
        {
            var returnType = method.ReturnType.IsEnumerable() ? "IEnumerable<" + RepositoryInfo.ClassFullName + ">" : RepositoryInfo.ClassFullName;
            var returnFunc = method.ReturnType.IsEnumerable() ? "return result.ToList();" : "return result.FirstOrDefault();";
            var filter = method.FilterInfo;
            var filterByIsDeleted = RepositoryInfo.IsDeletedExist;
            var sqlWhere = _whereQueryBy + filter.Key;
            var selectQuery = _selectByQuery;

            var parameters = filter.Parameters
                .Where(x => x.NeedGeneratePeriod == false)
                .Select(k => k.TypeName + " " + k.Name.FirstSymbolToLower()).ToList();
            var parameterNames = filter.Parameters
                .Where(x => x.NeedGeneratePeriod == false)
                .Select(k => k.Name.FirstSymbolToLower()).ToList();

            var dateTimeParams = filter.Parameters.Where(x => x.NeedGeneratePeriod);

            if(dateTimeParams.Any())
            {
                var updatedDateTimesParams = new List<string>();
                var updatedDateTimesParamsNames = new List<string>();

                var periodParams = dateTimeParams.ToList();

                for (var i = 0; i < periodParams.Count; i++)
                {
                    updatedDateTimesParams.Add(periodParams[i].TypeName + " start" + periodParams[i].Name);
                    updatedDateTimesParamsNames.Add("start" + periodParams[i].Name);
                    updatedDateTimesParams.Add(periodParams[i].TypeName + " end" + periodParams[i].Name);
                    updatedDateTimesParamsNames.Add("end" + periodParams[i].Name);
                }

                parameters = parameters.Union(updatedDateTimesParams).ToList();
                parameterNames = parameterNames.Union(updatedDateTimesParamsNames).ToList();
            }

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
            if(filterByIsDeleted)
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
            if(filterByIsDeleted)
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

        public string GenerateInsert(MethodImplementationInfo method)
        {
            var sb = new StringBuilder();

            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var methodParameter = RepositoryInfo.ClassFullName + " " + parameterName;

            var queryName = _insertQuery;

            // If should not return identifier
            if(RepositoryInfo.PrimaryKeys.Count != 1 || !RepositoryInfo.Identity)
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

        public string GenerateInsertWithModified(MethodImplementationInfo method)
        {
            var sb = new StringBuilder();
            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var methodParameter = RepositoryInfo.ClassFullName + " " + parameterName;

            var queryName = _insertQuery;

            // If should not return identifier
            if (RepositoryInfo.PrimaryKeys.Count != 1 || !RepositoryInfo.Identity)
            {
                // Synchronous method
                sb.AppendLine("public void Insert(" + methodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine(parameterName + ".Modified = " + DateTimeServiceField + ".CurrentDateTimeOffset;");
                sb.AppendLine(parameterName + ".ModifiedBy = " + DataAccessControllerField + ".EmployeeId.Value;");
                sb.AppendLine("DataAccessService.InsertObject(" + parameterName + "," + queryName + ");");
                sb.AppendLine("}");

                // Asynchronous method
                sb.AppendLine("public async Task InsertAsync(" + methodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine(parameterName + ".Modified = " + DateTimeServiceField + ".CurrentDateTimeOffset;");
                sb.AppendLine(parameterName + ".ModifiedBy = " + DataAccessControllerField + ".EmployeeId.Value;");
                sb.AppendLine("await DataAccessService.InsertObjectAsync(" + parameterName + "," + queryName + ");");
                sb.AppendLine("}");
            }
            else
            {
                var returnType = RepositoryInfo.PrimaryKeys.First().TypeName;

                // Synchronous method
                sb.AppendLine("public " + returnType + " Insert(" + methodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine(parameterName + ".Modified = " + DateTimeServiceField + ".CurrentDateTimeOffset;");
                sb.AppendLine(parameterName + ".ModifiedBy = " + DataAccessControllerField + ".EmployeeId.Value;");
                sb.AppendLine("var res = DataAccessService.InsertObject(" + parameterName + "," + queryName + ");");
                sb.AppendLine("return (" + returnType + ")res;");
                sb.AppendLine("}");

                // Asynchronous method
                sb.AppendLine("public async Task<" + returnType + "> InsertAsync(" + methodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine(parameterName + ".Modified = " + DateTimeServiceField + ".CurrentDateTimeOffset;");
                sb.AppendLine(parameterName + ".ModifiedBy = " + DataAccessControllerField + ".EmployeeId.Value;");
                sb.AppendLine("var res = await DataAccessService.InsertObjectAsync(" + parameterName + "," + queryName + ");");
                sb.AppendLine("return (" + returnType + ")res;");
                sb.AppendLine("}");
            }

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        public string GenerateUpdate(MethodImplementationInfo method)
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
            if(RepositoryInfo.JoinRepositoryInfo == null)
            {
                sb.AppendLine("var sql = " + _updateQueryBy + " + " + whereQueryName + "; ");
            }
            else
            {
                sb.AppendLine("var sql = " + _updateQueryBy + " + " + whereQueryName + " + " + _updateQuery + _join + " + " + whereQueryByJoinPk + "; ");
            }
            sb.AppendLine("DataAccessService.PersistObject(" + parameterName + ", sql);");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task UpdateBy" + filter.Key + "Async(" + methodParameter + ")");
            sb.AppendLine("{");
            if(RepositoryInfo.JoinRepositoryInfo == null)
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

        public string GenerateRemove(MethodImplementationInfo method)
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
            if(RepositoryInfo.JoinRepositoryInfo == null)
            {
                sb.AppendLine("var sql = " + _deleteQueryBy + " + " + whereQueryName + "; ");
            }
            else
            {
                sb.AppendLine("var sql = " + _selectIntoTemp + " + " + whereQueryName + " + " + _deleteQueryBy + "; ");
            }
            sb.AppendLine("DataAccessService.PersistObject(" + parameterName + ", sql);");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task RemoveBy" + filter.Key + "Async(" + methodParameter + ")");
            sb.AppendLine("{");
            if(RepositoryInfo.JoinRepositoryInfo == null)
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
            if(RepositoryInfo.JoinRepositoryInfo == null)
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
            if(RepositoryInfo.JoinRepositoryInfo == null)
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

        private string GenerateInsertOrUpdate(MethodImplementationInfo method)
        {
            var sb = new StringBuilder();

            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var methodParameter = RepositoryInfo.ClassFullName + " " + parameterName;

            var queryName = _insertOrUpdateQuery;

            // If should not return identifier

            // Synchronous method
            sb.AppendLine("public void InsertOrUpdate(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine("DataAccessService.ExecuteScalar(" + queryName + "," + parameterName + ");");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task InsertOrUpdateAsync(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine("await DataAccessService.ExecuteScalarAsync<" + RepositoryInfo.ClassFullName + " >(" + queryName + "," + parameterName + ");");
            sb.AppendLine("}");

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        #endregion
    }
}