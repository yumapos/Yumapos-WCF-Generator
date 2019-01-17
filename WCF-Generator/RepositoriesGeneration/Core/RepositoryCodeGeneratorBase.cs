using System.Linq;
using System.Text;
using WCFGenerator.RepositoriesGeneration.Helpers;
using WCFGenerator.RepositoriesGeneration.Infrastructure;

namespace WCFGenerator.RepositoriesGeneration.Core
{
    internal abstract class RepositoryCodeGeneratorBase : RepositoryCodeGeneratorAbstract
    {
        private readonly string _insertManyQueryTemplate = "InsertManyQueryTemplate";
        private readonly string _insertManyValuesTemplete = "InsertManyValuesTemplate";
        private readonly string _noCheckConstraint = "NoCheckConstraint";
        private readonly string _checkConstraint = "CheckConstraint";

        public override string GetFields()
        {
            // Common info for generate sql scriptes
            var sqlInfo = ScriptGenerator.GetTableInfo(RepositoryInfo.RepositorySqlInfo);

            var sb = new StringBuilder();

            var insertManyQueryTemplate = ScriptGenerator.GenerateInsertManyQueryTemplate(sqlInfo).SurroundWithQuotes();
            var insertManyValuesTemplate = ScriptGenerator.GenerateInsertManyValuesTemplate(sqlInfo).SurroundWithQuotes();
            var noCheckConstraint = ScriptGenerator.GenerateNoCheckConstraint(sqlInfo).SurroundWithQuotes();
            var checkConstraint = ScriptGenerator.GenerateCheckConstraint(sqlInfo).SurroundWithQuotes();

            sb.AppendLine("private const string " + _insertManyQueryTemplate + " = @" + insertManyQueryTemplate + ";");
            sb.AppendLine("private const string " + _insertManyValuesTemplete + " = @" + insertManyValuesTemplate + ";");
            sb.AppendLine("private const string " + _noCheckConstraint + " = @" + noCheckConstraint + ";");
            sb.AppendLine("private const string " + _checkConstraint + " = @" + checkConstraint + ";");

            return sb.ToString();
        }

        protected string GenerateInsertMany(bool requiresImplementation = true)
        {
            var sb = new StringBuilder();

            var elementName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var parameterName = $"{elementName}List";
            var methodParameter = $"IEnumerable<{RepositoryInfo.ClassFullName}> {parameterName}";
            var columns = RepositoryInfo.Elements.Concat(RepositoryInfo.JoinRepositoryInfo?.Elements ?? Enumerable.Empty<PropertyInfo>()).ToList();
            var valuesAsParametesCount = RepositoryInfo.Elements.Count(p => p.IsParameter) + RepositoryInfo.HiddenElements.Count(p => p.IsParameter);

            // Synchronous method
            sb.AppendLine("public void InsertMany(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine($"if({parameterName}==null) throw new ArgumentException(nameof({parameterName}));");
            sb.AppendLine();
            sb.AppendLine($"if(!{parameterName}.Any()) return;");
            sb.AppendLine();


            if (valuesAsParametesCount > 1)
            {
                sb.AppendLine($"var maxInsertManyRowsWithParameters = {MaxRepositoryParamsBaseRepositoryField} / {valuesAsParametesCount};");

                sb.AppendLine($@"var maxInsertManyRows = maxInsertManyRowsWithParameters < {MaxInsertManyRowsBaseRepositoryField} 
                                                        ? maxInsertManyRowsWithParameters
                                                        : {MaxInsertManyRowsBaseRepositoryField};");
            }
            else
            {
                sb.AppendLine($"var maxInsertManyRows = {MaxInsertManyRowsBaseRepositoryField};");
            }

            sb.AppendLine($"var values = new System.Text.StringBuilder();");
            sb.AppendLine($"var query = new System.Text.StringBuilder();");
            sb.AppendLine($"var parameters = new Dictionary<string, object>();");
            sb.AppendLine();
            sb.AppendLine($@"var itemsPerRequest = {parameterName}.Select((x, i) => new {{Index = i,Value = x}})
                .GroupBy(x => x.Index / maxInsertManyRows)
                .Select(x => x.Select((v, i) => new {{ Index = i, Value = v.Value }}).ToList())
                .ToList(); ");
            sb.AppendLine();

            sb.AppendLine($"{DataAccessServiceBaseRepositoryField}.Execute({_noCheckConstraint});");
            sb.AppendLine();

            if (RepositoryInfo.IsTenantRelated)
            {
                sb.AppendLine($"parameters.Add($\"TenantId\", {DataAccessControllerBaseRepositoryField}.Tenant.TenantId);");
            }

            sb.AppendLine("foreach (var items in itemsPerRequest)");
            sb.AppendLine("{");
            sb.AppendLine($"foreach (var item in items)");
            sb.AppendLine("{");
            sb.AppendLine($"var {elementName} = item.Value;");
            sb.AppendLine("var index = item.Index; ");

            var columnsAsParameters = columns.Where(c => c.IsParameter).ToList();

            foreach (var column in columnsAsParameters)
            {
                sb.AppendLine($"parameters.Add($\"{column.Name}{{index}}\", {elementName}.{column.Name});");
            }

            var values = columns.Where(c => !c.IsParameter)
                .Select(c =>
                {
                    var name = $"{elementName}.{c.Name}";

                    if (!c.IsNullable)
                    {
                        return c.IsDataType 
                            ? $"$\"'{{{name}}}'\"" 
                            : name;
                    }
                    return c.IsDataType
                        ? $"{name} != null ? $\"'{{{name}}}'\" : \"NULL\""
                        : $"{name}?.ToString() ?? \"NULL\"";

                })
                .ToList();

            sb.AppendLine("values.AppendLine(index != 0 ? \",\":\"\");");

            sb.AppendLine(values.Any()
                ? $"values.AppendFormat({_insertManyValuesTemplete}, {string.Join(",", values)}, index);"
                : $"values.AppendFormat({_insertManyValuesTemplete}, index);");

            sb.AppendLine("}");

            sb.AppendLine($"query.AppendFormat({_insertManyQueryTemplate}, values.ToString());");
            sb.AppendLine($"{DataAccessServiceBaseRepositoryField}.Execute(query.ToString(), parameters);");

            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine($"{DataAccessServiceBaseRepositoryField}.Execute({_checkConstraint});");
            sb.AppendLine();

            sb.AppendLine("}");
            sb.AppendLine();

            // Asynchronous method
            sb.AppendLine("public async Task InsertManyAsync(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine($"if({parameterName}==null) throw new ArgumentException(nameof({parameterName}));");
            sb.AppendLine();
            sb.AppendLine($"if(!{parameterName}.Any()) return;");
            sb.AppendLine();
            if (valuesAsParametesCount > 1)
            {
                sb.AppendLine($"var maxInsertManyRowsWithParameters = {MaxRepositoryParamsBaseRepositoryField} / {valuesAsParametesCount};");

                sb.AppendLine($@"var maxInsertManyRows = maxInsertManyRowsWithParameters < {MaxInsertManyRowsBaseRepositoryField} 
                                                        ? maxInsertManyRowsWithParameters
                                                        : {MaxInsertManyRowsBaseRepositoryField};");
            }
            else
            {
                sb.AppendLine($"var maxInsertManyRows = {MaxInsertManyRowsBaseRepositoryField};");
            }

            sb.AppendLine($"var values = new System.Text.StringBuilder();");
            sb.AppendLine($"var query = new System.Text.StringBuilder();");
            sb.AppendLine($"var parameters = new Dictionary<string, object>();");
            sb.AppendLine();
            sb.AppendLine($@"var itemsPerRequest = {parameterName}.Select((x, i) => new {{Index = i,Value = x}})
                .GroupBy(x => x.Index / maxInsertManyRows)
                .Select(x => x.Select((v, i) => new {{ Index = i, Value = v.Value }}).ToList())
                .ToList(); ");
            sb.AppendLine();

            sb.AppendLine($"await Task.Delay(10);");
            sb.AppendLine($"await {DataAccessServiceBaseRepositoryField}.ExecuteAsync({_noCheckConstraint});");
            sb.AppendLine();

            if (RepositoryInfo.IsTenantRelated)
            {
                sb.AppendLine($"parameters.Add($\"TenantId\", {DataAccessControllerBaseRepositoryField}.Tenant.TenantId);");
            }

            sb.AppendLine("foreach (var items in itemsPerRequest)");
            sb.AppendLine("{");
            sb.AppendLine($"foreach (var item in items)");
            sb.AppendLine("{");
            sb.AppendLine($"var {elementName} = item.Value;");
            sb.AppendLine("var index = item.Index; ");

            foreach (var column in columnsAsParameters)
            {
                sb.AppendLine($"parameters.Add($\"{column.Name}{{index}}\", {elementName}.{column.Name});");
            }

            sb.AppendLine("values.AppendLine(index != 0 ? \",\":\"\");");

            sb.AppendLine(values.Any()
                ? $"values.AppendFormat({_insertManyValuesTemplete}, {string.Join(",", values)}, index);"
                : $"values.AppendFormat({_insertManyValuesTemplete}, index);");

            sb.AppendLine("}");

            sb.AppendLine($"query.AppendFormat({_insertManyQueryTemplate}, values.ToString());");
            sb.AppendLine($"await Task.Delay(10);");
            sb.AppendLine($"await {DataAccessServiceBaseRepositoryField}.ExecuteAsync(query.ToString(), parameters);");

            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine($"await Task.Delay(10);");
            sb.AppendLine($"await {DataAccessServiceBaseRepositoryField}.ExecuteAsync({_checkConstraint});");
            sb.AppendLine();

            sb.AppendLine("}");
            sb.AppendLine();

            return requiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }
    }
}