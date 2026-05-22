using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCFGenerator.RepositoriesGeneration.Enums;
using WCFGenerator.RepositoriesGeneration.Helpers;
using WCFGenerator.RepositoriesGeneration.Infrastructure;
using WCFGenerator.RepositoriesGeneration.Services;

namespace WCFGenerator.RepositoriesGeneration.Core
{
    internal abstract class RepositoryCodeGeneratorBase : RepositoryCodeGeneratorAbstract
    {
        protected readonly string InsertManyQueryTemplateField = "InsertManyQueryTemplate";
        protected readonly string InsertManyValuesTemplateField = "InsertManyValuesTemplate";
        protected readonly string InsertManyJoinedValuesTempleteField = "InsertManyJoinedValuesTemplate";

        public override string GetFields()
        {
            // Common info for generate sql scriptes
            var sqlInfo = ScriptGenerator.GetTableInfo(RepositoryInfo.RepositorySqlInfo);

            var sb = new StringBuilder();

            var insertManyQueryTemplate = ScriptGenerator.GenerateInsertManyQueryTemplate(sqlInfo, RepositoryType).SurroundWithQuotes();
            var insertManyValuesTemplate = ScriptGenerator.GenerateInsertManyValuesTemplate(sqlInfo).SurroundWithQuotes();

            sb.AppendLine("private const string " + InsertManyQueryTemplateField + " = @" + insertManyQueryTemplate + ";");
            sb.AppendLine("private const string " + InsertManyValuesTemplateField + " = @" + insertManyValuesTemplate + ";");

            if (RepositoryInfo.JoinRepositoryInfo != null)
            {
                var insertManyJoinedValuesTemplate = ScriptGenerator.GenerateInsertManyJoinedValuesTemplate(sqlInfo).SurroundWithQuotes();

                sb.AppendLine("private const string " + InsertManyJoinedValuesTempleteField + " = @" + insertManyJoinedValuesTemplate + ";");
            }

            return sb.ToString();
        }

        protected string GenerateInsertMany(bool requiresImplementation = true)
        {
            var generationMethod = RepositoryInfo.InsertManyMethod;
            var dbType = RepositoryInfo.DatabaseType;
            var sb = new StringBuilder();
            var insertManyWrapped = WrapCallInsertMany(generationMethod);
            insertManyWrapped = requiresImplementation ? insertManyWrapped : insertManyWrapped.SurroundWithComments();
            sb.Append(insertManyWrapped);
            sb.AppendLine();
            sb.Append(GenerateInsertManyViaRows(requiresImplementation && generationMethod == InsertManyMethod.ViaRows));
            sb.AppendLine();
            if (dbType == DatabaseType.MSSql)
            {
                sb.AppendLine(GenerateInsertManyViaTvp(requiresImplementation && generationMethod == InsertManyMethod.ViaTvp));
            }
            return sb.ToString();
        }

        private string WrapCallInsertMany(InsertManyMethod method)
        {
            var suffix = method == InsertManyMethod.ViaRows ? "ViaRows" : "ViaTvp";
            var elementName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var parameterName = $"{elementName}List";
            var methodParameter = $"IEnumerable<{RepositoryInfo.ClassFullName}> {parameterName}";

            return $$"""
                     public void InsertMany({{methodParameter}})
                     {
                         InsertMany{{suffix}}({{parameterName}});
                     }

                     public async Task InsertManyAsync({{methodParameter}})
                     {
                         await InsertMany{{suffix}}Async({{parameterName}});
                     }

                     public void InsertManySplitByTransactions({{methodParameter}})
                     {
                         InsertMany{{suffix}}SplitByTransactions({{parameterName}});
                     }

                     public async Task InsertManySplitByTransactionsAsync({{methodParameter}})
                     {
                         await InsertMany{{suffix}}SplitByTransactionsAsync({{parameterName}});
                     }
                     """;
        }
        protected string GenerateInsertManyViaRows(bool requiresImplementation = true)
        {
            var joined = RepositoryInfo.JoinRepositoryInfo != null;

            var sb = new StringBuilder();

            var elementName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var parameterName = $"{elementName}List";
            var methodParameter = $"IEnumerable<{RepositoryInfo.ClassFullName}> {parameterName}";
            var allColumns = RepositoryInfo.Elements.Concat(RepositoryInfo.JoinRepositoryInfo?.Elements ?? Enumerable.Empty<PropertyInfo>()).ToList();
            var valuesAsParametesCount = RepositoryInfo.Elements.Count(p => p.IsParameter) + RepositoryInfo.HiddenElements.Count(p => p.IsParameter);
            var columnsAsParameters = allColumns.Where(c => c.IsParameter).ToList();
            var values = ExtractValuesAsString(elementName, RepositoryInfo.Elements.Where(c => !c.IsParameter));
            var joinedValues = ExtractValuesAsString(elementName, RepositoryInfo.JoinRepositoryInfo?.Elements?.Where(c => !c.IsParameter));

            Action<bool, bool> generator = (isAsync, splitByTransactions) =>
            {
                var methodName = splitByTransactions
                    ? "InsertManyViaRowsSplitByTransactions"
                    : "InsertManyViaRows";

                if (isAsync)
                {
                    sb.AppendLine($"public async Task {methodName}Async({methodParameter})");
                }
                else
                {
                    sb.AppendLine($"public void {methodName}({methodParameter})");
                }
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

                if (isAsync)
                {
                    sb.AppendLine();
                    sb.AppendLine($"await Task.Delay(10);");
                }

                sb.AppendLine();
                sb.AppendLine("foreach (var items in itemsPerRequest)");
                sb.AppendLine("{");

                if (splitByTransactions)
                {
                    sb.AppendLine("query.AppendLine(\"BEGIN TRANSACTION;\");");
                }

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
                    ? $"values.AppendFormat({InsertManyValuesTemplateField}, index, {string.Join(",", values)});"
                    : $"values.AppendFormat({InsertManyValuesTemplateField}, index);");


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

                if (splitByTransactions)
                {
                    sb.AppendLine("query.AppendLine(\"COMMIT TRANSACTION;\");");
                }

                if (isAsync)
                {
                    sb.AppendLine();
                    sb.AppendLine($"await Task.Delay(10);");
                    sb.AppendLine($"await {DataAccessServiceBaseRepositoryField}.ExecuteAsync(query.ToString(), parameters);");
                }
                else
                {
                    sb.AppendLine($"{DataAccessServiceBaseRepositoryField}.Execute(query.ToString(), parameters);");
                }

                sb.AppendLine("parameters.Clear();");
                sb.AppendLine("values.Clear();");
                if (joined)
                {
                    sb.AppendLine("joinedValues.Clear();");
                }
                sb.AppendLine("query.Clear();");

                sb.AppendLine("}");
                if (isAsync)
                {
                    sb.AppendLine();
                    sb.AppendLine($"await Task.Delay(10);");
                }

                sb.AppendLine();

                sb.AppendLine("}");

            };

            // synchronous method
            sb.AppendLine("");
            generator(false, false);
            sb.AppendLine("");
            generator(false, true);

            // Asynchronous method
            sb.AppendLine("");
            generator(true, false);
            sb.AppendLine("");
            generator(true, true);

            return requiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        protected string GenerateInsertManyViaTvp(bool requiresImplementation = true)
        {
            var isVersionRepo = RepositoryType == RepositoryType.Version;
            var tvpGen = new TvpMethodGenerator(RepositoryInfo, isVersionRepo);
            var insert = tvpGen.GenerateInsertManyViaTvp(true);
            return requiresImplementation ? insert : insert.SurroundWithComments();
        }
        
        protected List<string> ExtractValuesAsString(string entityName, IEnumerable<PropertyInfo> properties)
        {
            if (properties == null) return null;
            return properties.Select(c =>
                {
                    var name = $"{entityName}.{c.Name}";

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
        }

      
    }
}