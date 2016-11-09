using System;
using System.Linq;
using System.Text;
using WCFGenerator.RepositoriesGeneration.Core.SQL;
using WCFGenerator.RepositoriesGeneration.Heplers;
using WCFGenerator.RepositoriesGeneration.Infrastructure;

namespace WCFGenerator.RepositoriesGeneration.Core
{
    internal class CodeClassGeneratorVersionsRepository : BaseCodeClassGeneratorRepository
    {
        public static string RepositoryKind = "Version";
        private string _insertQueryName = "InsertQuery";
        private string _whereQueryBy = "WhereQueryBy";
        private string _whereQueryByWithAlias = "WhereQueryByWithAlias";
        private string _selectByQuery = "SelectBy";
        private string _selectByKeyAndSliceDateQuery = "SelectByKeyAndSliceDateQuery";
        private string _andWithIsDeletedFilter = "AndWithIsDeletedFilter";
        private string _andWithSliceDateFilter = "AndWithSliceDateFilter";
        private string _andWithIsDeletedFilterWithAlias = "AndWithIsDeletedFilterWithAlias";

        private string _join = "Join";
        private string _pk = "Pk";

        #region Overrides of BaseCodeClassGeneratorRepository

        public override string RepositoryName
        {
            get { return RepositoryInfo.ClassName + RepositoryKind + RepositoryInfo.RepositorySuffix; }
        }

        public override string RepositoryKindName
        {
            get { return RepositoryKind; }
        }

        public override RepositoryType RepositoryType
        {
            get { return RepositoryType.Version; }
        }

        public override string GetClassDeclaration()
        {
            return "internal class " + RepositoryName + " : RepositoryBase";
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
                var selectBy = (SqlScriptGenerator.GenerateSelectByToVersionTable(sqlInfo) + " {filter} ").SurroundWithQuotes();
                var selectByKeyAndSliceDateQuery = SqlScriptGenerator.GenerateSelectByKeyAndSliceDateToVersionTable(sqlInfo).SurroundWithQuotes();
                
                sb.AppendLine("private const string " + _selectByQuery + " = @" + selectBy + ";");
                sb.AppendLine("private const string " + _selectByKeyAndSliceDateQuery + " = @" + selectByKeyAndSliceDateQuery + ";");

                foreach (var method in RepositoryInfo.PossibleKeysForMethods)
                {
                    var key = method.Key;
                    var parametrs = method.Parameters.Select(p => p.Name).ToList();
                    var sqlV = SqlScriptGenerator.GenerateWhereVersions(parametrs, sqlInfo).SurroundWithQuotes();
                    var sqlA = SqlScriptGenerator.GenerateWhereVersionsWithAlias(parametrs, sqlInfo).SurroundWithQuotes();

                    sb.AppendLine("private const string " + _whereQueryBy + key + " = " + sqlV + ";");
                    sb.AppendLine("private const string " + _whereQueryByWithAlias + key + " = " + sqlA + ";");

                }
                // where by join PK
                if (RepositoryInfo.JoinRepositoryInfo != null)
                {
                    var sqlJoin = SqlScriptGenerator.GenerateWhereJoinPkVersion(sqlInfo).SurroundWithQuotes();
                    sb.AppendLine("private const string " + _whereQueryBy + _join + _pk + " = " + sqlJoin + ";");
                }

                // Is deleted filter
                if (RepositoryInfo.IsDeletedExist)
                {
                    var specialOption = RepositoryInfo.SpecialOptionsIsDeleted.Parameters.First().Name;
                    var andFilterV = SqlScriptGenerator.GenerateAnd(specialOption, sqlInfo.JoinVersionTableName?? sqlInfo.VersionTableName ).SurroundWithQuotes();
                    var andFilterA = SqlScriptGenerator.GenerateAndVersionsWithAlias(specialOption, sqlInfo).SurroundWithQuotes();
                    sb.AppendLine("private const string " + _andWithIsDeletedFilter + " = " + andFilterV + ";");
                    sb.AppendLine("private const string " + _andWithIsDeletedFilterWithAlias + " = " + andFilterA + ";");
                }

                // Is sliceDate filter (modified)
                if (RepositoryInfo.IsModifiedExist)
                {
                    var specialOption = RepositoryInfo.SpecialOptionsModified.Parameters.First().Name;
                    var filter = SqlScriptGenerator.GenerateAndVersionsWithAlias(specialOption, sqlInfo, "<=");
                    sb.AppendLine("private const string " + _andWithSliceDateFilter + " = " + filter.SurroundWithQuotes() + ";");
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

            if (!RepositoryInfo.IsManyToMany)
            {
                // RepositoryMethod.GetBy - for primary key
                var getByPrimaryKeyMethods = RepositoryInfo.MethodImplementationInfo
                    .Where(m => m.Method == RepositoryMethod.GetBy && m.FilterInfo.FilterType == FilterType.PrimaryKey)
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

        private string GenerateGetByVersionKey(MethodImplementationInfo method)
        {
            var code = GenerateGetByKey(method);
            return method.RequiresImplementation ? code : code.SurroundWithComments();
        }

        private string GenerateGetByKey(MethodImplementationInfo method)
        {
            var returnType = method.ReturnType.IsEnumerable() ? "IEnumerable<" + RepositoryInfo.ClassFullName + ">" : RepositoryInfo.ClassFullName;
            var returnFunc = method.ReturnType.IsEnumerable() ? "return result.ToList();" : "return result.FirstOrDefault();";
            var filter = method.FilterInfo;
            var isVersionKeyFilter = filter.FilterType == FilterType.VersionKey;
            var filterBySliceDate = !isVersionKeyFilter && RepositoryInfo.IsModifiedExist;
            var filterByIsDeleted = RepositoryInfo.IsModifiedExist;

            var sqlWhere = (!isVersionKeyFilter ? _whereQueryByWithAlias : _whereQueryBy) + filter.Key;

            var selectQuery = isVersionKeyFilter ? _selectByQuery : _selectByKeyAndSliceDateQuery;
            var isDeletedFilter = isVersionKeyFilter ? _andWithIsDeletedFilter : _andWithIsDeletedFilterWithAlias;

            var parameters = filter.Parameters.Select(k => k.TypeName + " " + k.Name.FirstSymbolToLower()).ToList();
            var parameterNames = filter.Parameters.Select(k => k.Name.FirstSymbolToLower()).ToList();

            if (filterBySliceDate)
            {
                var specialParameterModified = RepositoryInfo.SpecialOptionsModified.Parameters.First();
                var specialMethodParameterModified = specialParameterModified.TypeName + " " + specialParameterModified.Name.FirstSymbolToLower();
                var specialMethodParameterModifiedName = specialParameterModified.Name.FirstSymbolToLower();

                parameters.Add(specialMethodParameterModified);
                parameterNames.Add(specialMethodParameterModifiedName);
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
            sb.AppendLine("var filter = " + sqlWhere + ";");
            if (filterByIsDeleted)
            {
                var parameter = RepositoryInfo.SpecialOptionsIsDeleted.Parameters.First().Name.FirstSymbolToLower();
                sb.AppendLine("if (" + parameter + ".HasValue)");
                sb.AppendLine("{");
                sb.AppendLine("filter = filter + " + isDeletedFilter + ";");
                sb.AppendLine("}");
            }
            if (filterBySliceDate)
            {
                sb.AppendLine("filter = filter + " + _andWithSliceDateFilter + ";");
            }
            sb.AppendLine("var sql = " + selectQuery + ".Replace(\"{filter}\", filter);");
            sb.AppendLine("var result = DataAccessService.Get<" + RepositoryInfo.ClassFullName + ">(sql, parameters);");
            sb.AppendLine(returnFunc);
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task<" + returnType + "> GetBy" + filter.Key + "Async" + "(" + methodParameters + ")");
            sb.AppendLine("{");

            sb.AppendLine("object parameters = new {" + methodParameterNames + "};");
            sb.AppendLine("var filter = " + sqlWhere + ";");
            if (filterByIsDeleted)
            {
                var parameter = RepositoryInfo.SpecialOptionsIsDeleted.Parameters.First().Name.FirstSymbolToLower();
                sb.AppendLine("if (" + parameter + ".HasValue)");
                sb.AppendLine("{");
                sb.AppendLine("filter = filter + " + isDeletedFilter + ";");
                sb.AppendLine("}");
            }
            if (filterBySliceDate)
            {
                sb.AppendLine("filter = filter + " + _andWithSliceDateFilter + ";");
            }
            sb.AppendLine("var sql = " + selectQuery + ".Replace(\"{filter}\", filter);");
            sb.AppendLine("var result = (await DataAccessService.GetAsync<" + RepositoryInfo.ClassFullName + ">(sql, parameters));");
            sb.AppendLine(returnFunc);
            sb.AppendLine("}");
            sb.AppendLine("");

            return sb.ToString();
        }

        public override string GetFullCode()
        {
            var sb = new StringBuilder();

            // auto-generated header
            sb.AppendLine(CodeHelper.GeneratedDocumentHeader);

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