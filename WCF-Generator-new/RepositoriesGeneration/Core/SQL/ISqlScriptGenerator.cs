using System.Collections.Generic;
using WCFGenerator.RepositoriesGeneration.Infrastructure;

namespace WCFGenerator.RepositoriesGeneration.Core.SQL
{
    internal interface ISqlScriptGenerator
    {
        string GenerateFields(SqlInfo info);

        string GenerateValues(SqlInfo info);

        string GenerateSelectAll(SqlInfo info);

        string GenerateSelectBy(SqlInfo info, int? topNumber = null);

        string GenerateInsert(SqlInfo info);

        string GenerateInsertManyQueryTemplate(SqlInfo info, RepositoryType repositoryType);

        string GenerateInsertManyValuesTemplate(SqlInfo info);

        string GenerateInsertManyJoinedValuesTemplate(SqlInfo info);

        string GenerateInsertToTemp(SqlInfo info);

        string GenerateWhere(IEnumerable<ParameterInfo> parameters, SqlInfo info);

        string GenerateWhere(IEnumerable<string> selectedFilters, SqlInfo info);

        string GenerateWhere(IEnumerable<string> commonFilters, IEnumerable<string> datesFilters, SqlInfo info);

        string GenerateWhereJoinPk(SqlInfo info);

        string GenerateAnd(string selectedFilter, string ownerTable, string condition = "=");

        string GenerateOrderBySliceDate(SqlInfo info);

        string GenerateUpdate(SqlInfo info);

        string GenerateUpdateJoin(SqlInfo info);

        string GenerateRemove(SqlInfo info);

        string GenerateInsertToVersionTable(SqlInfo info);

        string GenerateSelectByToVersionTable(SqlInfo info);

        string GenerateSelectByKeyAndSliceDateToVersionTable(SqlInfo info);

        string GenerateWhereVersions(IEnumerable<string> selectedFilters, SqlInfo info);

        string GenerateWhereVersionsWithAlias(IEnumerable<string> selectedFilters, SqlInfo info);

        string GenerateAndVersionsWithAlias(string selectedFilter, SqlInfo info, string condition = "=");

        string GenerateWhereJoinPkVersion(SqlInfo info);

        string GenerateInsertOrUpdate(List<ParameterInfo> primaryKeys, SqlInfo info);

        string GenerateNoCheckConstraint(SqlInfo sqlInfo, RepositoryType repositoryType);

        string GenerateCheckConstraint(SqlInfo sqlInfo, RepositoryType repositoryType);

        SqlInfo GetTableInfo(SqlInfo repositoryInfo);

        string GenerateUpdateMany(SqlInfo sqlInfo);

        string GenerateUpdateManyJoined(SqlInfo sqlInfo);
    }
}