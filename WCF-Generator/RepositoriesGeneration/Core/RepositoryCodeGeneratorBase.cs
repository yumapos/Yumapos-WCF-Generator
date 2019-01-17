using System.Linq;
using System.Text;
using WCFGenerator.RepositoriesGeneration.Helpers;
using WCFGenerator.RepositoriesGeneration.Infrastructure;

namespace WCFGenerator.RepositoriesGeneration.Core
{
    internal abstract class RepositoryCodeGeneratorBase : RepositoryCodeGeneratorAbstract
    {
        protected readonly string InsertManyQueryTemplateField = "InsertManyQueryTemplate";
        protected readonly string InsertManyValuesTempleteField = "InsertManyValuesTemplate";
        protected readonly string NoCheckConstraintQueryField = "NoCheckConstraintQuery";
        protected readonly string CheckConstraintQueryField = "CheckConstraintQuery";
        protected const string ClearCacheField = "ClearCache";
        protected const string ClearCacheQueryField = "ClearCacheQuery";
        protected const string CheckConstraintAfterInsertManyField = "CheckConstraintAfterInsertMany";

        public override string GetFields()
        {
            // Common info for generate sql scriptes
            var sqlInfo = ScriptGenerator.GetTableInfo(RepositoryInfo.RepositorySqlInfo);

            var sb = new StringBuilder();

            var insertManyQueryTemplate = ScriptGenerator.GenerateInsertManyQueryTemplate(sqlInfo, RepositoryType).SurroundWithQuotes();
            var insertManyValuesTemplate = ScriptGenerator.GenerateInsertManyValuesTemplate(sqlInfo).SurroundWithQuotes();
            var noCheckConstraint = ScriptGenerator.GenerateNoCheckConstraint(sqlInfo, RepositoryType).SurroundWithQuotes();
            var checkConstraint = ScriptGenerator.GenerateCheckConstraint(sqlInfo, RepositoryType).SurroundWithQuotes();

            sb.AppendLine("private const string " + InsertManyQueryTemplateField + " = @" + insertManyQueryTemplate + ";");
            sb.AppendLine("private const string " + InsertManyValuesTempleteField + " = @" + insertManyValuesTemplate + ";");
            sb.AppendLine("private const string " + NoCheckConstraintQueryField + " = @" + noCheckConstraint + ";");
            sb.AppendLine("private const string " + CheckConstraintQueryField + " = @" + checkConstraint + ";");
            sb.AppendLine("private const string " + ClearCacheQueryField + " = @\"DBCC DROPCLEANBUFFERS; DBCC FREEPROCCACHE;\";");

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

            sb.AppendLine($"if({CheckConstraintAfterInsertManyField})");
            sb.AppendLine("{");
            sb.AppendLine($"{DataAccessServiceBaseRepositoryField}.Execute({NoCheckConstraintQueryField});");
            sb.AppendLine("}");

            sb.AppendLine();

            sb.AppendLine("foreach (var items in itemsPerRequest)");
            sb.AppendLine("{");

            if (RepositoryInfo.IsTenantRelated)
            {
                sb.AppendLine($"parameters.Add($\"TenantId\", {DataAccessControllerBaseRepositoryField}.Tenant.TenantId);");
            }

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
                        if (c.IsBool)
                        {
                            return $"{name} ? 1 : 0";
                        }
                        if (c.IsEnum)
                        {
                            return $"(int){name}";
                        }

                        return name;
                    }
                    if (c.IsBool)
                    {
                        return $"({name} != null ? ({name}.Value ? 1 : 0).ToString() : null) ?? \"NULL\"";
                    }
                    if (c.IsEnum)
                    {
                        return $"((int?){name})?.ToString() ?? \"NULL\"";
                    }

                    return $"{name}?.ToString() ?? \"NULL\"";

                })
                .ToList();

            sb.AppendLine("values.AppendLine(index != 0 ? \",\":\"\");");

            sb.AppendLine(values.Any()
                ? $"values.AppendFormat({InsertManyValuesTempleteField}, {string.Join(",", values)}, index);"
                : $"values.AppendFormat({InsertManyValuesTempleteField}, index);");

            sb.AppendLine("}");

            sb.AppendLine($"query.AppendFormat({InsertManyQueryTemplateField}, values.Replace(\"'NULL'\",\"NULL\").ToString());");
            sb.AppendLine($"if({ClearCacheField})");
            sb.AppendLine("{");
            sb.AppendLine($"{DataAccessServiceBaseRepositoryField}.Execute({ClearCacheQueryField});");
            sb.AppendLine("}");
            sb.AppendLine($"{DataAccessServiceBaseRepositoryField}.Execute(query.ToString(), parameters);");
            sb.AppendLine("parameters.Clear();");
            sb.AppendLine("values.Clear();");
            sb.AppendLine("query.Clear();");

            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine($"if({CheckConstraintAfterInsertManyField})");
            sb.AppendLine("{");
            sb.AppendLine($"{DataAccessServiceBaseRepositoryField}.Execute({CheckConstraintQueryField});");
            sb.AppendLine("}");
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
            sb.AppendLine($"if({CheckConstraintAfterInsertManyField})");
            sb.AppendLine("{");
            sb.AppendLine($"await {DataAccessServiceBaseRepositoryField}.ExecuteAsync({NoCheckConstraintQueryField});");
            sb.AppendLine("}");
            
            sb.AppendLine();

            sb.AppendLine("foreach (var items in itemsPerRequest)");
            sb.AppendLine("{");

            if (RepositoryInfo.IsTenantRelated)
            {
                sb.AppendLine($"parameters.Add($\"TenantId\", {DataAccessControllerBaseRepositoryField}.Tenant.TenantId);");
            }

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
                ? $"values.AppendFormat({InsertManyValuesTempleteField}, {string.Join(",", values)}, index);"
                : $"values.AppendFormat({InsertManyValuesTempleteField}, index);");

            sb.AppendLine("}");

            sb.AppendLine($"query.AppendFormat({InsertManyQueryTemplateField}, values.Replace(\"'NULL'\",\"NULL\").ToString());");
            sb.AppendLine($"await Task.Delay(10);");
            sb.AppendLine($"if({ClearCacheField})");
            sb.AppendLine("{");
            sb.AppendLine($"await {DataAccessServiceBaseRepositoryField}.ExecuteAsync({ClearCacheQueryField});");
            sb.AppendLine("}");

            sb.AppendLine($"await {DataAccessServiceBaseRepositoryField}.ExecuteAsync(query.ToString(), parameters);");
            sb.AppendLine("parameters.Clear();");
            sb.AppendLine("values.Clear();");
            sb.AppendLine("query.Clear();");

            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine($"await Task.Delay(10);");

            sb.AppendLine($"if({CheckConstraintAfterInsertManyField})");
            sb.AppendLine("{");
            sb.AppendLine($"await {DataAccessServiceBaseRepositoryField}.ExecuteAsync({CheckConstraintQueryField});");
            sb.AppendLine("}");

            sb.AppendLine();

            sb.AppendLine("}");
            sb.AppendLine();

            return requiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }
    }
}