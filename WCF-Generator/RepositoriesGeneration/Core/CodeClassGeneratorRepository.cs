using System.Linq;
using System.Text;
using VersionedRepositoryGeneration.Generator.Core.SQL;
using VersionedRepositoryGeneration.Generator.Heplers;
using VersionedRepositoryGeneration.Generator.Infrastructure;

namespace VersionedRepositoryGeneration.Generator.Core
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
        private string _andWithfilterData = "AndWithFilterData";
        private string _andWithfilterDataJoin = "AndWithFilterDataJoin";
        private string _join = "Join";
        private string _andWithTenant = "AndWithTenant";
        private string _andWithTenantJoin = "AndWithTenantJoin";


        #endregion

        #region Overrides of BaseCodeClassGeneratorRepository

        public override string GetUsings()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine(base.GetUsings());

            return sb.ToString();
        }

        public override string GetClassDeclaration()
        {
            return "public partial class " + RepositoryName + " : RepositoryBase," + RepositoryInfo.RepositoryInterfaceName;
        }

        public override string GetFields()
        {
            var sb = new StringBuilder();

            // Common info for generate sql scriptes
            var sqlInfo = new SqlInfo()
            {
                TableColumns = RepositoryInfo.Elements,
                TableName = RepositoryInfo.TableName,
                PrimaryKeyName = RepositoryInfo.PrimaryKeyName, // TODO more then one key ?
                JoinTableColumns = RepositoryInfo.JoinRepositoryInfo != null ? RepositoryInfo.JoinRepositoryInfo.Elements : null,
                JoinTableName = RepositoryInfo.JoinRepositoryInfo != null ? RepositoryInfo.JoinRepositoryInfo.TableName : null,
                JoinPrimaryKeyName = RepositoryInfo.JoinRepositoryInfo != null ? RepositoryInfo.JoinRepositoryInfo.PrimaryKeyName : null, // TODO more then one key ?
                TenantRelated = RepositoryInfo.IsTenantRelated
            };

            var fields = SqlScriptGenerator.GenerateFields(sqlInfo);
            var values = SqlScriptGenerator.GenerateValues(sqlInfo);
            var selectAllQuery = SqlScriptGenerator.GenerateSelectAll(sqlInfo).SurroundWithQuotes();
            var selectByQuery = SqlScriptGenerator.GenerateSelectBy(sqlInfo).SurroundWithQuotes();
            var insertQuery = SqlScriptGenerator.GenerateInsert(sqlInfo).SurroundWithQuotes();
            var updateBy = SqlScriptGenerator.GenerateUpdate(sqlInfo).SurroundWithQuotes();
            var deleteBy = SqlScriptGenerator.GenerateRemove(sqlInfo).SurroundWithQuotes();
            var selectIntoTemp = SqlScriptGenerator.GenerateInsertToTemp(sqlInfo).SurroundWithQuotes();

            sb.AppendLine("private const string Fields = " + fields.SurroundWithQuotes() + ";");
            sb.AppendLine("private const string Values = " + values.SurroundWithQuotes() + ";");
            sb.AppendLine("private const string " + _selectAllQuery + " = " + selectAllQuery + ";");
            sb.AppendLine("private const string " + _selectByQuery + " = " + selectByQuery + ";");
            sb.AppendLine("private const string " + _insertQuery + " = " + insertQuery + ";");
            sb.AppendLine("private const string " + _updateQueryBy + " = " + updateBy + ";");
            sb.AppendLine("private const string " + _deleteQueryBy + " = " + deleteBy + ";");
            sb.AppendLine("private const string " + _selectIntoTemp + " = " + selectIntoTemp + ";");

            if(RepositoryInfo.JoinRepositoryInfo != null)
            {
                var updateJoin = SqlScriptGenerator.GenerateUpdateJoin(sqlInfo).SurroundWithQuotes();
                sb.AppendLine("private const string " + _updateQuery + _join + " = " + updateJoin + ";");
            }

            foreach (var method in RepositoryInfo.PossibleKeysForMethods)
            {
                var key = method.Key;
                var parametrs = method.Parameters.Select(p => p.Name).ToList();
                var sql = SqlScriptGenerator.GenerateWhere(parametrs, sqlInfo).SurroundWithQuotes();
                sb.AppendLine("private const string " + _whereQueryBy + key + " = " + sql + ";");
            }

            var specialOptions = RepositoryInfo.SpecialOptions.Parameters.Select(p => p.Name).ToList();
            var andFilter = SqlScriptGenerator.GenerateWhere(specialOptions, sqlInfo).SurroundWithQuotes();
            sb.AppendLine("private const string " + _andWithfilterData + " = " + andFilter + ";");

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

            // RepositoryMethod.GetBy
            var getByMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.GetBy)
                .Aggregate("", (s, method) => s + GenerateGetBy(method));

            if (!string.IsNullOrEmpty(getByMethods))
                sb.AppendLine(getByMethods);

            // RepositoryMethod.Insert
            var insertMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.Insert)
                .Aggregate("", (s, method) => s + GenerateInsert(method));

            if (!string.IsNullOrEmpty(insertMethods))
                sb.AppendLine(insertMethods);

            // RepositoryMethod.UpdateBy
            var updateByMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.UpdateBy)
                .Aggregate("", (s, method) => s + GenerateUpdate(method));

            if (!string.IsNullOrEmpty(updateByMethods))
                sb.AppendLine(updateByMethods);

            // RepositoryMethod.RemoveBy
            var removeByMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.RemoveBy)
                .Aggregate("", (s, method) => s + GenerateRemove(method));

            if (!string.IsNullOrEmpty(removeByMethods))
                sb.AppendLine(removeByMethods);

            return sb.ToString();
        }

        #endregion

        #region Private

        private string GenerateGetAll(MethodImplementationInfo method)
        {
            var sb = new StringBuilder();

            var specialParameter = RepositoryInfo.SpecialOptions.Parameters.First();
            var specialMethodParameter = specialParameter.TypeName + "? " + specialParameter.Name.FirstSymbolToLower() + " = " + specialParameter.DefaultValue;
            var specialSqlParameter = specialParameter.Name.FirstSymbolToLower();

            // Synchronous method
            sb.AppendLine("public IEnumerable<" + RepositoryInfo.ClassFullName + "> GetAll(" + specialMethodParameter + ")");
            sb.AppendLine("{");

            sb.AppendLine("object parameters = new {" + specialSqlParameter + "};");
            sb.AppendLine("var sql = " + _selectAllQuery + ";");
            sb.AppendLine("if (" + specialSqlParameter + ".HasValue)");
            sb.AppendLine("{");
            sb.AppendLine("sql = sql + " + _andWithfilterData + ";");
            sb.AppendLine("}");

            sb.AppendLine("var result = DataAccessService.Get<" + RepositoryInfo.ClassFullName + ">(sql, parameters).ToList();");
            sb.AppendLine("return result.ToList();");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task<IEnumerable<" + RepositoryInfo.ClassFullName + ">> GetAllAsync(" + specialMethodParameter + ")");
            sb.AppendLine("{");

            sb.AppendLine("object parameters = new {" + specialSqlParameter + "};");
            sb.AppendLine("var sql = " + _selectAllQuery + ";");
            sb.AppendLine("if (" + specialSqlParameter + ".HasValue)");
            sb.AppendLine("{");
            sb.AppendLine("sql = sql + " + _andWithfilterData + ";");
            sb.AppendLine("}");

            sb.AppendLine("var result = (await DataAccessService.GetAsync<" + RepositoryInfo.ClassFullName + ">(sql, parameters));");
            sb.AppendLine("return result.ToList();");

            sb.AppendLine("}");

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateGetBy(MethodImplementationInfo method)
        {
            var sb = new StringBuilder();

            var methodParameters = string.Join(", ", method.Parameters.Select(k => k.TypeName + " " + k.Name.FirstSymbolToLower()));
            var sqlParameters = string.Join(", ", method.Parameters.Select(k => k.Name.FirstSymbolToLower()));
            var sqlWhere = string.Join(" + ", method.Parameters.Select(k => _whereQueryBy + k.Name));

            var specialParameter = RepositoryInfo.SpecialOptions.Parameters.First();
            var specialMethodParameter = specialParameter.TypeName + "? " + specialParameter.Name.FirstSymbolToLower() + " = " + specialParameter.DefaultValue;
            var specialSqlParameter = specialParameter.Name.FirstSymbolToLower();

            #region Synchronous method

            sb.AppendLine("public IEnumerable<" + RepositoryInfo.ClassFullName + "> GetBy" + method.Key + "(" + methodParameters + ", " + specialMethodParameter + ")");
            sb.AppendLine("{");

            sb.AppendLine("object parameters = new {" + sqlParameters + "," + specialSqlParameter + "};");
            sb.AppendLine("var sql = " + _selectAllQuery + " + " + sqlWhere + ";");
            sb.AppendLine("if (" + specialSqlParameter + ".HasValue)");
            sb.AppendLine("{");
            sb.AppendLine("sql = sql + " + _andWithfilterData + ";");
            sb.AppendLine("}");

            sb.AppendLine("var result = DataAccessService.Get<" + RepositoryInfo.ClassFullName + ">(sql, new {" + sqlParameters + "});");
            sb.AppendLine("return result.ToList();");
            sb.AppendLine("}");

            #endregion

            #region Asynchronous method

            sb.AppendLine("public async Task<IEnumerable<" + RepositoryInfo.ClassFullName + ">> GetBy" + method.Key + "Async" + "(" + methodParameters + ", " + specialMethodParameter + ")");
            sb.AppendLine("{");

            sb.AppendLine("object parameters = new {" + sqlParameters + "," + specialSqlParameter + "};");
            sb.AppendLine("var sql = " + _selectAllQuery + " + " + sqlWhere + ";");
            sb.AppendLine("if (" + specialSqlParameter + ".HasValue)");
            sb.AppendLine("{");
            sb.AppendLine("sql = sql + " + _andWithfilterData + ";");
            sb.AppendLine("}");

            sb.AppendLine("var result = (await DataAccessService.GetAsync<" + RepositoryInfo.ClassFullName + ">(sql, new {" + sqlParameters + "}));");
            sb.AppendLine("return result.ToList();");
            sb.AppendLine("}");
            sb.AppendLine("");

            #endregion





            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateInsert(MethodImplementationInfo method)
        {
            var sb = new StringBuilder();

            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var methodParameter = RepositoryInfo.ClassFullName + " " + parameterName;

            var queryName = _insertQuery;

            #region Synchronous method

            sb.AppendLine("public void Insert(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine("DataAccessService.InsertObject(" + parameterName + "," + queryName + ");");
            sb.AppendLine("}");

            #endregion

            #region Asynchronous method

            sb.AppendLine("public async Task InsertAsync(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine("await DataAccessService.InsertObjectAsync(" + parameterName + "," + queryName + ");");
            sb.AppendLine("}");

            #endregion

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateUpdate(MethodImplementationInfo method)
        {
            var sb = new StringBuilder();

            // Update by entity (use filter key)
            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var methodParameter = RepositoryInfo.ClassFullName + " " + parameterName;

            var whereQueryName = _whereQueryBy + method.Key;

            // Synchronous method
            sb.AppendLine("public void UpdateBy" + method.Key + "(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine("var sql = " + _updateQueryBy + " + " + whereQueryName + "; ");
            sb.AppendLine("DataAccessService.PersistObject(" + parameterName + ", sql);");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task UpdateBy" + method.Key + "Async(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine("var sql = " + _updateQueryBy + " + " + whereQueryName + "; ");
            sb.AppendLine("await DataAccessService.PersistObjectAsync(" + parameterName + ", sql);");
            sb.AppendLine("}");
            sb.AppendLine("");


            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateRemove(MethodImplementationInfo method)
        {
            var sb = new StringBuilder();

            // Remove by entity (use filter key)
            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var methodParameter = RepositoryInfo.ClassFullName + " " + parameterName;
            
            var whereQueryName = _whereQueryBy + method.Key;

            // Synchronous method
            sb.AppendLine("public void RemoveBy" + method.Key + "(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine("var sql = " + _deleteQueryBy + " + " + whereQueryName + "; ");
            sb.AppendLine("DataAccessService.PersistObject("+ parameterName + ", sql);");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task RemoveBy" + method.Key + "Async(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine("var sql = " + _deleteQueryBy + " + " + whereQueryName + "; ");
            sb.AppendLine("await DataAccessService.PersistObjectAsync(" + parameterName + ", sql);");
            sb.AppendLine("}");
            sb.AppendLine("");

            // Remove by filter key
            var parameter2 = method.Parameters.First();
            var parameter2Name = parameter2.Name.FirstSymbolToLower();
            var methodParameter2 = parameter2.TypeName + " " + parameter2Name;

            // Synchronous method
            sb.AppendLine("public void RemoveBy" + method.Key + "(" + methodParameter2 + ")");
            sb.AppendLine("{");
            sb.AppendLine("var sql = " + _deleteQueryBy + " + " + whereQueryName + "; ");
            sb.AppendLine("DataAccessService.PersistObject<" + RepositoryInfo.ClassFullName + ">(sql, new {" + parameter2Name + "});");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task RemoveBy" + method.Key + "Async(" + methodParameter2 + ")");
            sb.AppendLine("{");
            sb.AppendLine("var sql = " + _deleteQueryBy + " + " + whereQueryName + "; ");
            sb.AppendLine("await DataAccessService.PersistObjectAsync<" + RepositoryInfo.ClassFullName + ">(sql, new {" + parameter2Name + "});");
            sb.AppendLine("}");
            sb.AppendLine("");

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        #endregion
    }
}