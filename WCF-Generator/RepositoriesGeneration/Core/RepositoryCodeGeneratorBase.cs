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
        protected readonly string InsertManyJoinedValuesTempleteField = "InsertManyJoinedValuesTemplate";

        public override string GetFields()
        {
            // Common info for generate sql scriptes
            var sqlInfo = ScriptGenerator.GetTableInfo(RepositoryInfo.RepositorySqlInfo);

            var sb = new StringBuilder();

            var insertManyQueryTemplate = ScriptGenerator.GenerateInsertManyQueryTemplate(sqlInfo, RepositoryType).SurroundWithQuotes();
            var insertManyValuesTemplate = ScriptGenerator.GenerateInsertManyValuesTemplate(sqlInfo).SurroundWithQuotes();

            sb.AppendLine("private const string " + InsertManyQueryTemplateField + " = @" + insertManyQueryTemplate + ";");
            sb.AppendLine("private const string " + InsertManyValuesTempleteField + " = @" + insertManyValuesTemplate + ";");

            if (RepositoryInfo.JoinRepositoryInfo != null)
            {
                var insertManyJoinedValuesTemplate = ScriptGenerator.GenerateInsertManyJoinedValuesTemplate(sqlInfo).SurroundWithQuotes();

                sb.AppendLine("private const string " + InsertManyJoinedValuesTempleteField + " = @" + insertManyJoinedValuesTemplate + ";");
            }

            return sb.ToString();
        }

        protected string GenerateInsertMany(bool requiresImplementation = true)
        {
            var joined = RepositoryInfo.JoinRepositoryInfo != null;

            var sb = new StringBuilder();

            var elementName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var parameterName = $"{elementName}List";
            var methodParameter = $"IEnumerable<{RepositoryInfo.ClassFullName}> {parameterName}";
            var allColumns = RepositoryInfo.Elements.Concat(RepositoryInfo.JoinRepositoryInfo?.Elements ?? Enumerable.Empty<PropertyInfo>()).ToList();
            var valuesAsParametesCount = RepositoryInfo.Elements.Count(p => p.IsParameter) + RepositoryInfo.HiddenElements.Count(p => p.IsParameter);

            if (joined)
            {
                valuesAsParametesCount += 2;// @TempTable, @TempId
            }

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

            if (joined)
            {
                sb.AppendLine($"var joinedValues = new System.Text.StringBuilder();");
            }

            sb.AppendLine($"var query = new System.Text.StringBuilder();");
            sb.AppendLine($"var parameters = new Dictionary<string, object>();");
            sb.AppendLine();
            sb.AppendLine($@"var itemsPerRequest = {parameterName}.Select((x, i) => new {{Index = i,Value = x}})
                .GroupBy(x => x.Index / maxInsertManyRows)
                .Select(x => x.Select((v, i) => new {{ Index = i, Value = v.Value }}).ToList())
                .ToList(); ");
            sb.AppendLine();

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

            var columnsAsParameters = allColumns.Where(c => c.IsParameter).ToList();

            foreach (var column in columnsAsParameters)
            {
                sb.AppendLine($"parameters.Add($\"{column.Name}{{index}}\", {elementName}.{column.Name});");
            }

            var values = RepositoryInfo.Elements.Where(c => !c.IsParameter)
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

                        if (c.CultureDependent)
                        {
                            return $"{name}.ToString(CultureInfo.InvariantCulture)";
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
                    if (c.CultureDependent)
                    {
                        return $"{name}?.ToString(CultureInfo.InvariantCulture) ?? \"NULL\"";
                    }

                    return $"{name}?.ToString() ?? \"NULL\"";

                })
                .ToList();


            var joinedValues = RepositoryInfo.JoinRepositoryInfo?.Elements?.Where(c => !c.IsParameter)
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

                        if (c.CultureDependent)
                        {
                            return $"{name}.ToString(CultureInfo.InvariantCulture)";
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
                    if (c.CultureDependent)
                    {
                        return $"{name}?.ToString(CultureInfo.InvariantCulture) ?? \"NULL\"";
                    }

                    return $"{name}?.ToString() ?? \"NULL\"";

                })
                .ToList();

            sb.AppendLine("values.AppendLine(index != 0 ? \",\":\"\");");

            sb.AppendLine(values.Any()
                ? $"values.AppendFormat({InsertManyValuesTempleteField}, index, {string.Join(",", values)});"
                : $"values.AppendFormat({InsertManyValuesTempleteField}, index);");

            if (joined)
            {
                sb.AppendLine("joinedValues.AppendLine(index != 0 ? \",\":\"\");");

                sb.AppendLine(values.Any()
                    ? $"joinedValues.AppendFormat({InsertManyJoinedValuesTempleteField}, index, {string.Join(",", joinedValues)});"
                    : $"joinedValues.AppendFormat({InsertManyJoinedValuesTempleteField}, index);");
            }

            sb.AppendLine("}");

            if (joined)
            {
                sb.AppendLine($"query.AppendFormat({InsertManyQueryTemplateField}, joinedValues.Replace(\"'NULL'\",\"NULL\").ToString(), values.Replace(\"'NULL'\",\"NULL\").ToString());");
            }
            else
            {
                sb.AppendLine($"query.AppendFormat({InsertManyQueryTemplateField}, values.Replace(\"'NULL'\",\"NULL\").ToString());");
            }

            sb.AppendLine($"{DataAccessServiceBaseRepositoryField}.Execute(query.ToString(), parameters);");
            sb.AppendLine("parameters.Clear();");
            sb.AppendLine("values.Clear();");
            if (joined)
            {
                sb.AppendLine("joinedValues.Clear();");
            }
            sb.AppendLine("query.Clear();");

            sb.AppendLine("}");
            sb.AppendLine();
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

            if (joined)
            {
                sb.AppendLine($"var joinedValues = new System.Text.StringBuilder();");
            }

            sb.AppendLine($"var query = new System.Text.StringBuilder();");
            sb.AppendLine($"var parameters = new Dictionary<string, object>();");
            sb.AppendLine();
            sb.AppendLine($@"var itemsPerRequest = {parameterName}.Select((x, i) => new {{Index = i,Value = x}})
                .GroupBy(x => x.Index / maxInsertManyRows)
                .Select(x => x.Select((v, i) => new {{ Index = i, Value = v.Value }}).ToList())
                .ToList(); ");
            sb.AppendLine();

            sb.AppendLine($"await Task.Delay(10);");
            
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
                ? $"values.AppendFormat({InsertManyValuesTempleteField}, index, {string.Join(",", values)});"
                : $"values.AppendFormat({InsertManyValuesTempleteField}, index);");


            if (joined)
            {
                sb.AppendLine("joinedValues.AppendLine(index != 0 ? \",\":\"\");");

                sb.AppendLine(values.Any()
                    ? $"joinedValues.AppendFormat({InsertManyJoinedValuesTempleteField}, index, {string.Join(",", joinedValues)});"
                    : $"joinedValues.AppendFormat({InsertManyJoinedValuesTempleteField}, index);");
            }

            sb.AppendLine("}");

            if (joined)
            {
                sb.AppendLine($"query.AppendFormat({InsertManyQueryTemplateField}, joinedValues.Replace(\"'NULL'\",\"NULL\").ToString(), values.Replace(\"'NULL'\",\"NULL\").ToString());");
            }
            else
            {
                sb.AppendLine($"query.AppendFormat({InsertManyQueryTemplateField}, values.Replace(\"'NULL'\",\"NULL\").ToString());");
            }

            sb.AppendLine($"await Task.Delay(10);");

            sb.AppendLine($"await {DataAccessServiceBaseRepositoryField}.ExecuteAsync(query.ToString(), parameters);");
            sb.AppendLine("parameters.Clear();");
            sb.AppendLine("values.Clear();");
            if (joined)
            {
                sb.AppendLine("joinedValues.Clear();");
            }
            sb.AppendLine("query.Clear();");

            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine($"await Task.Delay(10);");

            sb.AppendLine();

            sb.AppendLine("}");
            sb.AppendLine();

            return requiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }
    }
}