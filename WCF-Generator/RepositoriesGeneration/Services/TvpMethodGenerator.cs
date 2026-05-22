using System.Text;
using WCFGenerator.RepositoriesGeneration.Helpers;
using WCFGenerator.RepositoriesGeneration.Infrastructure;
using WCFGenerator.RepositoriesGeneration.Infrastructure.Tvp;

namespace WCFGenerator.RepositoriesGeneration.Services;

internal class TvpMethodGenerator
{
    private readonly RepositoryInfo _repoInfo;
    private readonly bool _isVersionRepo;
    private readonly TvpMetadata _tvp;

    public TvpMethodGenerator(RepositoryInfo repoInfo, bool isVersionRepo)
    {
        _repoInfo = repoInfo;
        _isVersionRepo = isVersionRepo;
        _tvp = repoInfo.TvpMetadata;
    }

    public string GenerateInsertManyViaTvp(bool requiresImplementation = true)
    {
        if (_tvp == null)
        {
            return "Cannot generate insert many via TVP";
        }

        if (!string.IsNullOrEmpty(_tvp.AnalysisError))
        {
            return _tvp.AnalysisError;
        }
        if (!_tvp.OrderedColumns.Any())
            return string.Empty;

        var sb = new StringBuilder();
        var className = _repoInfo.ClassName;
        var elementName = className.FirstSymbolToLower();
        var parameterName = $"{elementName}List";
        var methodParameter = $"IEnumerable<{_repoInfo.ClassFullName}> {parameterName}";

        GenerateMethod(sb, false, methodParameter, parameterName, _repoInfo.JoinRepositoryInfo?.TvpMetadata);
        GenerateMethod(sb, true, methodParameter, parameterName, _repoInfo.JoinRepositoryInfo?.TvpMetadata);
        GenerateSplitByTransactions(sb, false, methodParameter, parameterName, _repoInfo.JoinRepositoryInfo?.TvpMetadata);
        GenerateSplitByTransactions(sb, true, methodParameter, parameterName, _repoInfo.JoinRepositoryInfo?.TvpMetadata);

        var code = sb.ToString();
        return requiresImplementation ? code : code.SurroundWithComments();
    }

    private void GenerateMethod(StringBuilder sb, bool isAsync, string methodParameter, string parameterName, TvpMetadata joinedTableTvp)
    {
        GenerateInsertManyMethod(sb, isAsync, methodParameter, parameterName, joinedTableTvp, useBatching: false);
    }

    private void GenerateSplitByTransactions(StringBuilder sb, bool isAsync, string methodParameter, string parameterName, TvpMetadata joinedTableTvp)
    {
        GenerateInsertManyMethod(sb, isAsync, methodParameter, parameterName, joinedTableTvp, useBatching: true);
    }

    private void GenerateInsertManyMethod(StringBuilder sb, bool isAsync, string methodParameter, string parameterName, TvpMetadata joinedTableTvp, bool useBatching)
    {
        var columns = _tvp.OrderedColumns;
        var tvpTypeName = _tvp.DataTableName;
        var joinColumns = joinedTableTvp?.OrderedColumns;
        var joinedTableExists = joinColumns != null;
        var methodNameSuffix = useBatching ? "SplitByTransactions" : "";
        var methodName = $"InsertManyViaTvp{methodNameSuffix}{(isAsync ? "Async" : "")}";
        var returnType = isAsync ? "async Task" : "void";
        var dataAccessServiceField = "DataAccessService";

        sb.AppendLine($"private {returnType} {methodName}({methodParameter})");
        sb.AppendLine("{");
        sb.AppendLine($"if ({parameterName} == null) throw new ArgumentNullException(nameof({parameterName}));");
        sb.AppendLine($"var list = {parameterName}.ToList();");
        sb.AppendLine($"if (!list.Any()) return;");
        sb.AppendLine();

        if (useBatching)
        {
            sb.AppendLine("const int batchSize = 1000;");
            sb.AppendLine();
        }

        if (joinedTableExists)
        {
            sb.AppendLine("var joinedDataTable = new System.Data.DataTable();");
            foreach (var col in joinColumns)
            {
                sb.AppendLine($"joinedDataTable.Columns.Add(\"{col.DataTableColumnName}\", typeof({col.DoClrUnderlying}));");
                if (col.IsNullable)
                {
                    sb.AppendLine($"joinedDataTable.Columns[\"{col.DataTableColumnName}\"].AllowDBNull = true;");
                }
            }
        }

        // Создание основной DataTable
        sb.AppendLine("var dataTable = new System.Data.DataTable();");
        foreach (var col in columns)
        {
            sb.AppendLine($"dataTable.Columns.Add(\"{col.DataTableColumnName}\", typeof({col.DoClrUnderlying}));");
        }
        sb.AppendLine();

        // Выбор способа итерации: пакетная или обычная
        var loopHeader = useBatching
            ? "foreach (var batch in list.Chunk(batchSize))"
            : "foreach (var item in list)";

        sb.AppendLine($"{loopHeader}");
        sb.AppendLine("{");

        if (useBatching)
        {
            sb.AppendLine("foreach (var item in batch)");
            sb.AppendLine("{");
        }

        // Заполнение строки основной таблицы
        sb.AppendLine("var row = dataTable.NewRow();");

        foreach (var col in columns)
        {
            var propName = col.DoPropertyName;
            if (propName.Equals("tenantId", StringComparison.OrdinalIgnoreCase))
            {
                sb.AppendLine($"row[\"{col.DataTableColumnName}\"] = DataAccessController.Tenant.TenantId;");
            }
            else if (col.IsNullable)
            {
                sb.AppendLine($"row[\"{col.DataTableColumnName}\"] = item.{propName} == null ? DBNull.Value : item.{propName};");
            }
            else
            {
                sb.AppendLine($"row[\"{col.DataTableColumnName}\"] = item.{propName};");
            }
        }

        sb.AppendLine("dataTable.Rows.Add(row);");

        // Заполнение строки присоединяемой таблицы (если есть)
        if (joinedTableExists)
        {
            sb.AppendLine("var joinedRow = joinedDataTable.NewRow();");

            foreach (var col in joinColumns)
            {
                var propName = col.DoPropertyName;
                if (propName.Equals("tenantId", StringComparison.OrdinalIgnoreCase))
                {
                    sb.AppendLine($"joinedRow[\"{col.DataTableColumnName}\"] = DataAccessController.Tenant.TenantId;");
                }
                else if (col.IsNullable)
                {
                    sb.AppendLine($"joinedRow[\"{col.DataTableColumnName}\"] = item.{propName} == null ? DBNull.Value : item.{propName};");
                }
                else
                {
                    sb.AppendLine($"joinedRow[\"{col.DataTableColumnName}\"] = item.{propName};");
                }
            }
            sb.AppendLine("joinedDataTable.Rows.Add(joinedRow);");
        }

        sb.AppendLine("}");
        if (!useBatching)
        {
            sb.AppendLine();
            AppendSqlAndExecute(sb, isAsync, columns, joinColumns, joinedTableTvp, tvpTypeName, dataAccessServiceField);
            sb.AppendLine("}");
            sb.AppendLine();
            return;
        }

        //for batching
        sb.AppendLine();
        AppendSqlAndExecute(sb, isAsync, columns, joinColumns, joinedTableTvp, tvpTypeName, dataAccessServiceField);

        // Очистка таблиц
        if (joinedTableExists)
        {
            sb.AppendLine("joinedDataTable.Clear();");
        }
        sb.AppendLine("dataTable.Clear();");

        if (isAsync)
        {
            sb.AppendLine("await Task.Delay(10);");
        }

        sb.AppendLine("}");
        sb.AppendLine("}");
        sb.AppendLine();
    }

    private void AppendSqlAndExecute(StringBuilder sb, bool isAsync, IReadOnlyCollection<TvpColumnInfo> columns, IReadOnlyCollection<TvpColumnInfo> joinColumns, TvpMetadata joinedTableTvp, string tvpTypeName, string dataAccessServiceField)
    {
        var joinedTableExists = joinColumns != null;
        sb.AppendLine("var parameters = new Dictionary<string, object>();");
        sb.AppendLine($"{dataAccessServiceField}.AddTableParameter(dataTable, \"{tvpTypeName}\", \"tvp\", parameters);");
        if (joinedTableExists)
        {
            sb.AppendLine($"{dataAccessServiceField}.AddTableParameter(joinedDataTable, \"{joinedTableTvp.DataTableName}\", \"joinedTvp\", parameters);");
        }

        sb.AppendLine("var sql = @\"");
        if (joinedTableExists)
        {
            var joinedTableName = _isVersionRepo ? $"[{joinedTableTvp.VersionSchema}].[{joinedTableTvp.VersionTableName}]" : $"[{joinedTableTvp.Schema}].[{joinedTableTvp.TableName}]";
            sb.AppendLine($"INSERT INTO {joinedTableName} ({string.Join(", ", joinColumns.Select(c => $"{joinedTableName}.[{c.DataTableColumnName}]"))})");
            sb.AppendLine($"SELECT {string.Join(", ", joinColumns.Select(c => $"[{c.DataTableColumnName}]"))} FROM @joinedTvp;");
            sb.AppendLine();
        }

        var tableName = _isVersionRepo ? $"[{_tvp.VersionSchema}].[{_tvp.VersionTableName}]" : $"[{_tvp.Schema}].[{_tvp.TableName}]";
        sb.AppendLine($"INSERT INTO {tableName} ({string.Join(", ", columns.Select(c => $"{tableName}.[{c.DataTableColumnName}]"))}) ");
        sb.AppendLine($"SELECT {string.Join(", ", columns.Select(c => $"[{c.DataTableColumnName}]"))} FROM @tvp;\";");
        sb.AppendLine();

        if (isAsync)
        {
            sb.AppendLine($"await {dataAccessServiceField}.ExecuteAsync(sql, parameters);");
        }
        else
        {
            sb.AppendLine($"{dataAccessServiceField}.Execute(sql, parameters);");
        }
    }
}

