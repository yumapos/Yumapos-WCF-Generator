using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCFGenerator.RepositoriesGeneration.Helpers;

namespace WCFGenerator.RepositoriesGeneration.Core.SQL
{
    internal class SQLPostgresScriptGenerator : ISqlScriptGenerator
    {
        private const string _tempTable = "@temp";
        private const string _sliceDateColumnName = "modified";
        private const string _versionTableAlias1 = "version_table1";
        private const string _joinVersionTableAlias1 = "join_version_table1";

        public string GenerateTableName(string tableName)
        {
            tableName = tableName.Trim();

            var nametrue = string.Join(".", tableName.Split('.').Select(part => part.Trim('[', ']')));

            return nametrue;
        }

        #region Cache table

        public string GenerateFields(SqlInfo info)
        {
            return Fields(info.TableColumns, info.TableName);
        }

        public string GenerateValues(SqlInfo info)
        {
            return Values(info.TableColumns);
        }

        public string GenerateSelectAll(SqlInfo info)
        {
            return GenerateSelectBy(info, null) + " " + WhereTenantRelated(info.TableName, info.TenantRelated) + " ";
        }

        public string GenerateSelectBy(SqlInfo info, int? topNumber = null)
        {
            if (info.JoinTableColumns != null && !string.IsNullOrEmpty(info.JoinTableName))
            {
                // return select from table and join table
                return Select(info.TableColumns, info.TableName, topNumber) + ","
                       + Fields(info.JoinTableColumns, info.JoinTableName) + " "
                       + From(info.TableName) + " "
                       + InnerJoin(info.TableName, info.PrimaryKeyNames.First(), info.JoinTableName, info.JoinPrimaryKeyNames.First()) + " ";//TODO FIX TO MANY KEYS
            }
            // return select from table
            return Select(info.TableColumns, info.TableName, topNumber) + " " + From(info.TableName) + " ";
        }

        public string GenerateInsert(SqlInfo info)
        {
            // Skip PK if identity = true
            var columns = info.TableColumns
                .Except(info.IdentityColumns)
                .Concat(info.HiddenTableColumns)
                .ToList();

            var insertSql = !info.ReturnPrimaryKey
                ? Insert(columns, info.TableName)
                : Insert(columns, info.TableName, info.PrimaryKeyNames.First());

            // Return if have not joined table 
            if (info.JoinTableColumns == null)
            {
                return insertSql;
            }

            // Skip PK if identity of joined table = true
            var columnsJoned = info.JoinTableColumns
                .Except(info.IdentityColumnsJoined)
                .Concat(info.HiddenTableColumns)
                .ToList();

            var valuesJoined = columnsJoned.Select(c => c == info.JoinVersionKeyName ? info.VersionKeyName : c == info.JoinPrimaryKeyNames.First() ? info.PrimaryKeyNames.First() : c).ToList();


            var insertJoinedTable = InsertWithJoined(columnsJoned, valuesJoined, info.JoinTableName, info.JoinPrimaryKeyNames.First(), info.PrimaryKeyType, columns, columns, info.TableName, info.PrimaryKeyNames.First());

            // return inset into table and join table
            return insertJoinedTable;
        }

        public string GenerateInsertToTemp(SqlInfo info)
        {
            return "DECLARE " + _tempTable + " TABLE (ItemId uniqueidentifier);" +
                                                        "INSERT INTO " + _tempTable + " " +
                                                        "SELECT " + Field(info.TableName, info.PrimaryKeyNames.First()) + " FROM " + info.TableName + " ";//TODO FIX TO MANY KEYS
        }

        public string GenerateWhere(IEnumerable<string> selectedFilters, SqlInfo info)
        {
            return Where(selectedFilters, info.TableName) + AndTenantRelated(info.TableName, info.TenantRelated) + " ";
        }

        public string GenerateWhereJoinPk(SqlInfo info)
        {
            return Where(new[] { info.JoinPrimaryKeyNames.First() }, info.JoinTableName) + AndTenantRelated(info.JoinTableName, info.TenantRelated) + " "; //TODO FIX TO MANY KEYS
        }


        public string GenerateAnd(string selectedFilter, string ownerTable, string condition = "=")
        {
            return And(new[] { selectedFilter }, ownerTable, condition);
        }

        public string GenerateOrderBySliceDate(SqlInfo info)
        {
            return OrderBy("modified", info.JoinTableName, true);
        }

        public string GenerateUpdate(SqlInfo info)
        {
            var columns = info.TableColumns.Where(c => info.IdentityColumns.All(pk => pk != c)).ToList();
            return Update(info.TableName) + " "
                    + Set(columns, info.TableName) + " "
                    + From(info.TableName) + " ";
        }

        public string GenerateUpdateWithoutTable(SqlInfo info)
        {
            var columns = info.TableColumns.Where(c => info.IdentityColumns.All(pk => pk != c)).ToList();
            return Update(info.TableName) + " "
                    + Set(columns, info.TableName) + " ";
        }

        public string GenerateUpdateJoin(SqlInfo info)
        {
            // use pk from inherite model
            var values = info.JoinTableColumns
                .Except(info.IdentityColumnsJoined)
                .Select(c => new KeyValuePair<string, string>(c, c == info.JoinVersionKeyName ? info.VersionKeyName : c == info.JoinPrimaryKeyNames.First() ? info.PrimaryKeyNames.First() : c));//TODO FIX TO MANY KEYS

            return Update(info.JoinTableName) + " "
                   + Set(values, info.JoinTableName) + " "
                   + From(info.JoinTableName) + " ";
        }

        public string GenerateRemove(SqlInfo info)
        {
            // base type of repository and "isDeleted" flag - set IsDeleted = true
            if (info.IsDeleted && info.VersionTableName == null)
            {
                if (info.JoinTableColumns != null && !string.IsNullOrEmpty(info.JoinTableName))
                    return
                        Update(info.JoinTableName) + " SET is_deleted = TRUE ";

                return Update(info.TableName) + " SET is_deleted = TRUE ";
            }

            // base or version repository without "isDeleted" flag
            if (info.JoinTableColumns != null && !string.IsNullOrEmpty(info.JoinTableName))
                return Delete(info.TableName) + " WHERE " + Field(info.TableName, info.PrimaryKeyNames.First()) + " = ANY (SELECT ItemId FROM " + _tempTable + ");" +//TODO FIX TO MANY KEYS
                        Delete(info.JoinTableName) + " WHERE " + Field(info.JoinTableName, info.JoinPrimaryKeyNames.First()) + " = ANY (SELECT ItemId FROM " + _tempTable + ");" + " ";//TODO FIX TO MANY KEYS

            return Delete(info.TableName) + " ";
        }

        public string GenerateInsertOrUpdate(SqlInfo info)
        {
            var insert = GenerateInsert(info);
            var conflict = " ON CONFLICT (" + string.Join(",", info.PrimaryKeyNames.Select(PostgresColumnsHelper.Convert)) + ") DO ";
            var update = GenerateUpdateWithoutTable(info);
            return insert + conflict + update;
        }

        #endregion

        #region VERSION TABLE

        public string GenerateInsertToVersionTable(SqlInfo info)
        {
            var columns = info.TableColumns.Concat(info.HiddenTableColumns).ToList();
            var classProperties = Fields(columns, info.TableName);
            var classValues = Values(columns);

            if (info.JoinTableName != null)
            {
                var joinFields = info.JoinTableColumns.Concat(info.HiddenTableColumns).ToList();
                var joinClassProperties = Fields(joinFields, info.JoinTableName);
                // use versionId & primary key from inherite model
                var values = joinFields.Select(c => c == info.JoinVersionKeyName ? info.VersionKeyName : c == info.JoinPrimaryKeyNames.First() ? info.PrimaryKeyNames.First() : c).ToList(); //TODO FIX TO MANY KEYS
                var joinClassValues = Values(values);

                return "INSERT INTO " + info.JoinVersionTableName + "(" + joinClassProperties + ")\r\n" +
                       "VALUES (" + joinClassValues + ")\r\n" +
                   "INSERT INTO " + info.VersionTableName + "(" + classProperties + ")\r\n" +
                   "VALUES (" + classValues + ")";
            }

            if (!info.IsManyToMany)
            {
                return "INSERT INTO " + info.VersionTableName + "(" + classProperties + ")\r\n" +
                        "VALUES (" + classValues + ")";
            }

            return "INSERT INTO " + info.VersionTableName + "(" + classProperties + ")\r\n" +
                       "VALUES (" + classValues + ")";
        }

        public string GenerateSelectByToVersionTable(SqlInfo info)
        {
            if (info.JoinTableColumns != null && !string.IsNullOrEmpty(info.JoinTableName))
            {
                // return select from table and join table
                return Select(info.TableColumns, info.VersionTableName) + ","
                       + Fields(info.JoinTableColumns, info.JoinVersionTableName) + " "
                       + From(info.VersionTableName) + " "
                       + InnerJoin(info.VersionTableName, info.PrimaryKeyNames.First(), info.JoinVersionTableName, info.JoinPrimaryKeyNames.First()) + " ";//TODO FIX TO MANY KEYS
            }
            // return select from table
            return Select(info.TableColumns, info.VersionTableName) + " " + From(info.VersionTableName) + " ";
        }

        public string GenerateSelectByKeyAndSliceDateToVersionTable(SqlInfo info)
        {
            var versionTableAlias = "version_table";

            var joined = info.JoinTableColumns != null && !string.IsNullOrEmpty(info.JoinTableName);

            if (joined)
            {
                var selectMaxModifiedJoin = @"(SELECT " + string.Join(",", info.PrimaryKeyNames.Select(pk => Field(_versionTableAlias1, pk))) + ", MAX(" + Field(_joinVersionTableAlias1, _sliceDateColumnName) + ") as " + _sliceDateColumnName + " "
                                            + From(info.VersionTableName + " " + _versionTableAlias1) + " "
                                            + InnerJoin(info.JoinTableName + " " + _joinVersionTableAlias1, Field(_versionTableAlias1, info.VersionKeyName) + " = " + Field(_joinVersionTableAlias1, info.JoinVersionKeyName)) + " "
                                            + "{filter} "
                                            + " GROUP BY " + string.Join(",", info.PrimaryKeyNames.Select(pk => Field(_versionTableAlias1, pk))) + ") " + versionTableAlias;

                // join all columns from main table
                var comparePrimaryKey = string.Join(" AND ", info.PrimaryKeyNames.Select(pk => Field(versionTableAlias, pk) + " = " + Field(info.JoinVersionTableName, info.JoinPrimaryKeyNames.First())));
                var compareSliceDate = Field(versionTableAlias, _sliceDateColumnName) + " = " + Field(info.JoinVersionTableName, _sliceDateColumnName);
                var join1 = InnerJoin(info.JoinVersionTableName, comparePrimaryKey + " AND " + compareSliceDate);

                // join all columns from joinned table
                var compareVersionId = Field(info.JoinVersionTableName, info.JoinVersionKeyName) + " = " + Field(info.VersionTableName, info.VersionKeyName);
                var join2 = InnerJoin(info.VersionTableName, compareVersionId);
                return Select(info.TableColumns, info.VersionTableName) + " FROM " + selectMaxModifiedJoin + " " + join1 + " " + join2;
            }
            else
            {
                var selectMaxModified = @"(SELECT " + string.Join(",", info.PrimaryKeyNames.Select(pk => Field(_versionTableAlias1, pk))) + ", MAX(" + Field(_versionTableAlias1, _sliceDateColumnName) + ") as " + _sliceDateColumnName + " "
                                        + From(info.VersionTableName, _versionTableAlias1) + " "
                                        + " {filter} "
                                        + " GROUP BY " + string.Join(",", info.PrimaryKeyNames.Select(pk => Field(_versionTableAlias1, pk))) + ") " + versionTableAlias;

                // join all columns from main table
                var comparePrimaryKey = string.Join(" AND ", info.PrimaryKeyNames.Select(pk => Field(versionTableAlias, pk) + " = " + Field(info.VersionTableName, pk)));
                var compareSliceDate = Field(versionTableAlias, _sliceDateColumnName) + " = " + Field(info.VersionTableName, _sliceDateColumnName);
                var join1 = InnerJoin(info.VersionTableName, comparePrimaryKey + " AND " + compareSliceDate);

                return Select(info.TableColumns, info.VersionTableName) + " FROM " + selectMaxModified + " " + join1;
            }
        }

        public string GenerateWhereVersions(IEnumerable<string> selectedFilters, SqlInfo info)
        {
            return Where(selectedFilters, info.VersionTableName) + AndTenantRelated(info.VersionTableName, info.TenantRelated) + " ";
        }

        public string GenerateWhereVersionsWithAlias(IEnumerable<string> selectedFilters, SqlInfo info)
        {
            return Where(selectedFilters, _versionTableAlias1) + AndTenantRelated(_versionTableAlias1, info.TenantRelated) + " ";
        }

        public string GenerateAndVersionsWithAlias(string selectedFilter, SqlInfo info, string condition = "=")
        {
            var tableAlias = info.JoinVersionTableName != null ? _joinVersionTableAlias1 : _versionTableAlias1;
            return And(new[] { selectedFilter }, tableAlias, condition);
        }

        public string GenerateWhereJoinPkVersion(SqlInfo info)
        {
            return Where(new[] { info.JoinPrimaryKeyNames.First() }, info.JoinVersionTableName) + AndTenantRelated(info.JoinVersionTableName, info.TenantRelated) + " "; //TODO FIX TO MANY KEYS
        }

        #endregion

        #region SQL operators

        private static string Fields(IEnumerable<string> columns, string table)
        {
            columns = columns.Select(PostgresColumnsHelper.Convert);
            return string.Join(",", columns.Select(e => Field(table, e)));
        }
        private static string Values(IEnumerable<string> columns)
        {
            return string.Join(",", columns.Select(e => "@" + e));
        }

        private static string Select(IEnumerable<string> columns, string table, int? topNumber = null)
        {
            return "SELECT " + (topNumber.HasValue ? " TOP " + topNumber : "") + Fields(columns, table);
        }

        private static string Insert(IList<string> columns, string ownerTableName)
        {
            return "INSERT INTO " + ownerTableName + "(" + Fields(columns, ownerTableName) + ")  VALUES(" + Values(columns) + ") ";
        }

        private static string Insert(IList<string> columns, string ownerTableName, string returnInserted)
        {
            var outputKey = "OUTPUT INSERTED." + returnInserted;
            return "INSERT INTO " + ownerTableName + "(" + Fields(columns, ownerTableName) + ") " + outputKey + " VALUES(" + Values(columns) + ") ";
        }

        private static string InsertWithJoined(List<string> joinedTableColumns, List<string> joinedTableValues, string joinedTableName, string joinedPkColumn, string joinedPkType, List<string> tableColumns, List<string> tableValues, string tableName, string pkColumn)
        {
            var tableForSave = "TempTable";
            var declareTable = "DECLARE @" + tableForSave + " TABLE (" + joinedPkColumn + " " + joinedPkType + ");";
            var insertToJoined = "INSERT INTO " + joinedTableName + "(" + Fields(joinedTableColumns, joinedTableName) + ") " + "OUTPUT INSERTED." + PostgresColumnsHelper.Convert(joinedPkColumn) + (string.IsNullOrEmpty(tableForSave) ? "" : " INTO @" + tableForSave) + " VALUES(" + Values(joinedTableValues) + ");";

            var tempValue = "TempId";
            var insertedValue = "DECLARE @" + tempValue + " " + joinedPkType + "; SELECT @" + tempValue + " = " + joinedPkColumn + " FROM @" + tableForSave + ";";

            var insert = "INSERT INTO " + tableName + "(" + Fields(tableColumns, tableName) + ") " + "OUTPUT INSERTED." + PostgresColumnsHelper.Convert(pkColumn) + (string.IsNullOrEmpty(tableForSave) ? "" : " INTO @" + tableForSave) + " VALUES(" + Values(tableValues.Select(v => v == pkColumn ? tempValue : v)) + ");";
            var selectId = "SELECT " + joinedPkColumn + " FROM @" + tableForSave + ";";
            return declareTable + insertToJoined + insertedValue + insert + selectId;
        }

        private static string Update(string tableName)
        {
            return "UPDATE " + tableName;
        }

        private static string Set(IEnumerable<string> parameters, string ownerTableName)
        {
            var sb = new StringBuilder();

            sb.Append("SET ");
            sb.Append(string.Join(",", parameters.Select(i => ownerTableName + "." + PostgresColumnsHelper.Convert(i) + " = @" + i)));

            return sb.ToString();
        }

        private static string Set(IEnumerable<KeyValuePair<string, string>> parameters, string ownerTableName)
        {
            var sb = new StringBuilder();

            sb.Append("SET ");
            sb.Append(string.Join(",", parameters.Select(i => ownerTableName + "." + PostgresColumnsHelper.Convert(i.Key) + " = @" + i.Value)));

            return sb.ToString();
        }

        private static string Delete(string tableName)
        {
            return "DELETE FROM " + tableName;
        }

        private static string From(string tableName, string alias = null)
        {
            return "FROM " + tableName + (alias != null ? " " + alias : "");
        }
        private static string InnerJoin(string tableName, string tablePrimaryKey, string joinTableName, string joinTablePrimaryKey)
        {
            return "INNER JOIN " + joinTableName + " ON " + tableName + "." + PostgresColumnsHelper.Convert(tablePrimaryKey) + " = " + joinTableName + "." + PostgresColumnsHelper.Convert(joinTablePrimaryKey);
        }

        private static string InnerJoin(string joinTableName, string conditions)
        {
            return "INNER JOIN " + joinTableName + " ON " + conditions;
        }

        private static string Where(IEnumerable<string> parameters, string ownerTableName)
        {
            var sb = new StringBuilder();

            sb.Append("WHERE ");
            sb.Append(string.Join(" AND ", parameters.Select(i => ownerTableName + "." + PostgresColumnsHelper.Convert(i) + " = @" + i)));

            return sb.ToString();
        }

        private static string And(IEnumerable<string> parameters, string ownerTableName, string condition = "=")
        {
            var sb = new StringBuilder();

            sb.Append("AND ");
            sb.Append(string.Join(" AND ", parameters.Select(i => ownerTableName + "." + PostgresColumnsHelper.Convert(i) + " " + condition + " @" + i)));
            sb.Append(" ");

            return sb.ToString();
        }

        private static string AndTenantRelated(string tableName, bool isTenantRelated)
        {
            return isTenantRelated ? "{andTenantId:" + tableName + "}" : "";
        }

        private static string OrderBy(string columnName, string ownerTableName, bool desc)
        {
            return "ORDER BY " + ownerTableName + "." + PostgresColumnsHelper.Convert(columnName) + (desc ? " DESC" : "");
        }


        private static string WhereTenantRelated(string tableName, bool isTenantRelated)
        {
            return isTenantRelated ? "{whereTenantId:" + tableName + "}" : "";
        }

        #endregion

        #region Private

        private static string Field(string tableName, string fieldName)
        {
            return tableName + "." + fieldName;
        }

        public SqlInfo GetTableInfo(SqlInfo repositoryInfo)
        {
            repositoryInfo.TableName = GenerateTableName(repositoryInfo.TableName);
            repositoryInfo.VersionTableName = repositoryInfo.VersionTableName != null ? GenerateTableName(repositoryInfo.VersionTableName) : null;
            repositoryInfo.JoinTableName = repositoryInfo.JoinTableName != null ? GenerateTableName(repositoryInfo.JoinTableName) : null;
            repositoryInfo.JoinVersionTableName = repositoryInfo.JoinVersionTableName != null ? GenerateTableName(repositoryInfo.JoinVersionTableName) : null;
            repositoryInfo.VersionKeyType = repositoryInfo.VersionKeyType != null ? SystemToPostgreSqlTypeMapper.GetPostgreSqlType(repositoryInfo.VersionKeyType) : null;
            repositoryInfo.PrimaryKeyType = repositoryInfo.PrimaryKeyType != null ? SystemToPostgreSqlTypeMapper.GetPostgreSqlType(repositoryInfo.PrimaryKeyType) : null;

            return repositoryInfo;
        }

        #endregion
    }
}