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

        private string _selectAllQuery = "SelectQuery";
        private string _insertQuery = "InsertQuery";
        private string _updateQueryBy = "UpdateQueryBy";
        private string _deleteQueryBy = "DeleteQueryBy";
        private string _whereQueryBy = "WhereQueryBy";
        private string _andWithfilterData = "AndWithFilterData";

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

            sb.AppendLine("private const string " + _selectAllQuery + " = " + SqlScriptGenerator.GenerateSelectAll(RepositoryInfo.Elements, RepositoryInfo.TableName).SurroundWithQuotes() + ";");
            sb.AppendLine("private const string " + _insertQuery + " = " + SqlScriptGenerator.GenerateInsert(RepositoryInfo.Elements, RepositoryInfo.TableName).SurroundWithQuotes() + ";");

            foreach (var method in RepositoryInfo.FilterInfos)
            {
                var key = method.Key;
                var parametrs = method.Parameters.Select(p => p.Name).ToList();
                sb.AppendLine("private const string " + _whereQueryBy + key + " = " + SqlScriptGenerator.GenerateWhere(parametrs, RepositoryInfo.TableName).SurroundWithQuotes() + ";");
                sb.AppendLine("private const string " + _updateQueryBy + key + " = " + SqlScriptGenerator.GenerateUpdate(parametrs, RepositoryInfo.TableName).SurroundWithQuotes() + ";");
                sb.AppendLine("private const string " + _deleteQueryBy + key + " = " + SqlScriptGenerator.GenerateRemove(parametrs, RepositoryInfo.TableName).SurroundWithQuotes() + ";");
            }

            var specialOptions = RepositoryInfo.SpecialOptions.Parameters.Select(p => p.Name).ToList();
            sb.AppendLine("private const string " + _andWithfilterData + " = " + SqlScriptGenerator.GenerateWhere(specialOptions, RepositoryInfo.TableName).SurroundWithQuotes() + ";");

            return sb.ToString();
        }

        public override string GetMethods()
        {
            var sb = new StringBuilder();

            // RepositoryMethod.GetAll
            var getAllMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.GetAll)
                .Aggregate("", (s, method) => GenerateGetAll(method));

            if (!string.IsNullOrEmpty(getAllMethods))
                sb.AppendLine(getAllMethods);

            // RepositoryMethod.GetBy
            var getByMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.GetBy)
                .Aggregate("", (s, method) => GenerateGetBy(method));

            if (!string.IsNullOrEmpty(getByMethods))
                sb.AppendLine(getByMethods);

            // RepositoryMethod.Insert
            var insertMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.Insert)
                .Aggregate("", (s, method) => GenerateInsert(method));

            if (!string.IsNullOrEmpty(insertMethods))
                sb.AppendLine(insertMethods);

            // RepositoryMethod.UpdateBy
            var updateByMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.UpdateBy)
                .Aggregate("", (s, method) => GenerateUpdate(method));

            if (!string.IsNullOrEmpty(updateByMethods))
                sb.AppendLine(updateByMethods);

            // RepositoryMethod.RemoveBy
            var removeByMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.RemoveBy)
                .Aggregate("", (s, method) => GenerateRemove(method));

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

            #region Synchronous method

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

            #endregion

            #region Asynchronous method

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

            #endregion

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

            var queryName = _updateQueryBy + method.Key;
            var whereQueryName = _whereQueryBy + method.Key;

            // Synchronous method
            sb.AppendLine("public void UpdateBy" + method.Key + "(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine("var sql = " + queryName + " + " + whereQueryName + "; ");
            sb.AppendLine("DataAccessService.PersistObject(" + parameterName + ", sql);");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task UpdateBy" + method.Key + "Async(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine("var sql = " + queryName + " + " + whereQueryName + "; ");
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
            
            var queryName = _deleteQueryBy + method.Key;
            var whereQueryName = _whereQueryBy + method.Key;

            // Synchronous method
            sb.AppendLine("public void RemoveBy" + method.Key + "(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine("var sql = " + queryName + " + " + whereQueryName + "; ");
            sb.AppendLine("DataAccessService.PersistObject("+ parameterName + ", sql);");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task RemoveBy" + method.Key + "Async(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine("var sql = " + queryName + " + " + whereQueryName + "; ");
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
            sb.AppendLine("var sql = " + queryName + " + " + whereQueryName + "; ");
            sb.AppendLine("DataAccessService.PersistObject<" + RepositoryInfo.ClassFullName + ">(sql, new {" + parameter2Name + "});");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task RemoveBy" + method.Key + "Async(" + methodParameter2 + ")");
            sb.AppendLine("{");
            sb.AppendLine("var sql = " + queryName + " + " + whereQueryName + "; ");
            sb.AppendLine("await DataAccessService.PersistObjectAsync<" + RepositoryInfo.ClassFullName + ">(sql, new {" + parameter2Name + "});");
            sb.AppendLine("}");
            sb.AppendLine("");

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        #endregion
    }
}