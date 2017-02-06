using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCFGenerator.RepositoriesGeneration.Core.SQL
{
    internal class SqlScriptGenerator
    {
        private const string _tempTable = "@Temp";
        private const string _sliceDateColumnName = "Modified";
        private const string _versionTableAlias1 = "versionTable1";
        private const string _joinVersionTableAlias1 = "joinVersionTable1";

        public static string GenerateTableName(string tableName)
        {
            tableName = tableName.Trim();

            var nametrue = string.Join(".", tableName.Split('.').Select(part => "[" + part.Trim('[', ']') + "]"));

            return nametrue;
        }

        #region Cache table

        public static string GenerateFields(SqlInfo info)
        {
            return Fields(info.TableColumns, info.TableName);
        }

        public static string GenerateValues(SqlInfo info)
        {
            return Values(info.TableColumns);
        }

        public static string GenerateSelectAll(SqlInfo info)
        {
            return GenerateSelectBy(info, null) + " " + WhereTenantRelated(info.TableName, info.TenantRelated) + " ";
        }

        public static string GenerateSelectBy(SqlInfo info, int? topNumber = null)
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

        public static string GenerateInsert(SqlInfo info)
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
            var insertJoinedTable = InsertWithJoined(columnsJoned, info.JoinTableName, info.JoinPrimaryKeyNames.First(), info.PrimaryKeyType, info.TableColumns, info.TableName, info.PrimaryKeyNames.First());

            // return inset into table and join table
            return insertJoinedTable;
        }

        public static string GenerateInsertToTemp(SqlInfo info)
        {
            return "DECLARE " + _tempTable + " TABLE (ItemId uniqueidentifier);" +
                                                        "INSERT INTO "+ _tempTable + " " +
                                                        "SELECT " + Field(info.TableName, info.PrimaryKeyNames.First()) + " FROM " + info.TableName + " ";//TODO FIX TO MANY KEYS
        }

        public static string GenerateWhere(IEnumerable<string> selectedFilters, SqlInfo info)
        {
            return Where(selectedFilters, info.TableName) + AndTenantRelated(info.TableName, info.TenantRelated) + " ";
        }

        public static string GenerateWhereJoinPk( SqlInfo info)
        {
            return Where(new [] { info.JoinPrimaryKeyNames.First() }, info.JoinTableName) + AndTenantRelated(info.JoinTableName, info.TenantRelated) + " "; //TODO FIX TO MANY KEYS
        }


        public static string GenerateAnd(string selectedFilter, string ownerTable, string condition = "=")
        {
            return And(new []{selectedFilter}, ownerTable, condition);
        }

        public static string GenerateOrderBySliceDate(SqlInfo info)
        {
            return OrderBy("Modified", info.JoinTableName, true);
        }

        public static string GenerateUpdate(SqlInfo info)
        {
            var columns = info.TableColumns.Where(c => info.IdentityColumns.All(pk => pk != c)).ToList();
            return Update(info.TableName) + " " 
                    + Set(columns, info.TableName) + " " 
                    + From(info.TableName) + " ";
        }

        public static string GenerateUpdateJoin(SqlInfo info)
        {
            // use pk from inherite model
            var values = info.JoinTableColumns.Select(c => new KeyValuePair<string,string>(c,c == info.JoinVersionKeyName ? info.VersionKeyName : c == info.JoinPrimaryKeyNames.First() ? info.PrimaryKeyNames.First() : c));//TODO FIX TO MANY KEYS

            return Update(info.JoinTableName) + " "
                   + Set(values, info.JoinTableName) + " "
                   + From(info.JoinTableName) + " ";
        }

        public static string GenerateRemove(SqlInfo info)
        {
            if (info.JoinTableColumns != null && !string.IsNullOrEmpty(info.JoinTableName))
                return Delete(info.TableName) + " WHERE " + Field(info.TableName, info.PrimaryKeyNames.First()) + " IN (SELECT ItemId FROM " + _tempTable + ");" +//TODO FIX TO MANY KEYS
                   Delete(info.JoinTableName) + " WHERE " + Field(info.JoinTableName, info.JoinPrimaryKeyNames.First()) + " IN (SELECT ItemId FROM " + _tempTable + ");" + " ";//TODO FIX TO MANY KEYS

            return Delete(info.TableName) + " ";
        }

        #endregion

        #region VERSION TABLE

        public static string GenerateInsertToVersionTable(SqlInfo info)
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

            if(!info.IsManyToMany)
            {
                return "INSERT INTO " + info.VersionTableName + "(" + classProperties + ")\r\n" +
                        "VALUES (" + classValues + ")";
            }

            return "INSERT INTO " + info.VersionTableName + "(" + classProperties + ")\r\n" +
                       "VALUES (" + classValues + ")";
        }

        public static string GenerateSelectByToVersionTable(SqlInfo info)
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

        public static string GenerateSelectByKeyAndSliceDateToVersionTable(SqlInfo info)
        {
            var versionTableAlias = "versionTable";
            
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

        public static string GenerateWhereVersions(IEnumerable<string> selectedFilters, SqlInfo info)
        {
            return Where(selectedFilters, info.VersionTableName) + AndTenantRelated(info.VersionTableName, info.TenantRelated) + " ";
        }

        public static string GenerateWhereVersionsWithAlias(IEnumerable<string> selectedFilters, SqlInfo info)
        {
            return Where(selectedFilters, _versionTableAlias1) + AndTenantRelated(_versionTableAlias1, info.TenantRelated) + " ";
        }

        public static string GenerateAndVersionsWithAlias(string selectedFilter, SqlInfo info, string condition = "=")
        {
            var tableAlias = info.JoinVersionTableName != null ? _joinVersionTableAlias1 : _versionTableAlias1;
            return And(new[] { selectedFilter }, tableAlias, condition);
        }

        public static string GenerateWhereJoinPkVersion(SqlInfo info)
        {
            return Where(new[] { info.JoinPrimaryKeyNames.First() }, info.JoinVersionTableName) + AndTenantRelated(info.JoinVersionTableName, info.TenantRelated) + " "; //TODO FIX TO MANY KEYS
        }

        #endregion

        #region SQL operators

        private static string Fields(IEnumerable<string> columns, string table)
        {
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

        private static string Insert(IEnumerable<string> tableColumns, string ownerTableName)
        {
            var columns = tableColumns.ToList();
            return "INSERT INTO " + ownerTableName + "(" + Fields(columns, ownerTableName) + ") VALUES(" + Values(columns) + ")";
        }

        private static string Insert(IEnumerable<string> tableColumns, string ownerTableName, string returnInserted)
        {
            var columns = tableColumns.ToList();
            var outputKey = "OUTPUT INSERTED." + returnInserted;
            return "INSERT INTO " + ownerTableName + "(" + Fields(columns, ownerTableName) + ") " + outputKey + " VALUES(" + Values(columns) + ")";
        }

        private static string InsertWithJoined(IEnumerable<string> joinedTableColumns, string joinedTableName, string joinedPkColumn, string joinedPkType, IEnumerable<string> tableColumns, string tableName, string pkColumn)
        {
            var joinedTableColumnsList = joinedTableColumns.ToList();

            var tableForSave = "Temp" + joinedPkColumn + "Table";
            var declareTable = "DECLARE @" + tableForSave + " TABLE (" + joinedPkColumn + " " + joinedPkType + ");";
            var outputKey = "OUTPUT INSERTED." + joinedPkColumn + (string.IsNullOrEmpty(tableForSave) ? "" : " INTO @" + tableForSave);
            var insertToJoined = "INSERT INTO " + joinedTableName + "(" + Fields(joinedTableColumnsList, joinedTableName) + ") " + outputKey + " VALUES(" + Values(joinedTableColumnsList) + ");";

            var tempValue = "Temp" + joinedPkColumn;
            var insertedValue = "DECLARE @" + tempValue + " " + joinedPkType + "; SELECT " + tempValue + " = "+ joinedPkColumn + " FROM @" + tableForSave + ";";

            var tableColumnsList = tableColumns.ToList();
            var values = Values(tableColumnsList.Select(c => c!= pkColumn ? c : tempValue));
            var insert = "INSERT INTO @" + tableName + "(" + Fields(tableColumnsList, joinedTableName) + ") " + outputKey + " VALUES(" + values + ");";
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
            sb.Append(string.Join(",", parameters.Select(i => ownerTableName + ".[" + i + "] = @" + i)));

            return sb.ToString();
        }

        private static string Set(IEnumerable<KeyValuePair<string, string>> parameters, string ownerTableName)
        {
            var sb = new StringBuilder();

            sb.Append("SET ");
            sb.Append(string.Join(",", parameters.Select(i => ownerTableName + ".[" + i.Key + "] = @" + i.Value)));

            return sb.ToString();
        }

        private static string Delete(string tableName)
        {
            return "DELETE FROM " + tableName;
        }

        private static string From(string tableName, string alias = null)
        {
            return "FROM " + tableName +  (alias!=null ? " " + alias : "");
        }
        private static string InnerJoin(string tableName, string tablePrimaryKey, string joinTableName, string joinTablePrimaryKey)
        {
            return "INNER JOIN " + joinTableName + " ON " + tableName + ".[" + tablePrimaryKey + "] = " + joinTableName + ".[" + joinTablePrimaryKey + "]";
        }

        private static string InnerJoin(string joinTableName, string conditions)
        {
            return "INNER JOIN " + joinTableName + " ON " + conditions;
        }

        private static string Where(IEnumerable<string> parameters, string ownerTableName)
        {
            var sb = new StringBuilder();

            sb.Append("WHERE ");
            sb.Append(string.Join(" AND ", parameters.Select(i => ownerTableName + ".[" + i + "] = @" + i)));

            return sb.ToString();
        }

        private static string And(IEnumerable<string> parameters, string ownerTableName, string condition = "=")
        {
            var sb = new StringBuilder();

            sb.Append("AND ");
            sb.Append(string.Join(" AND ", parameters.Select(i => ownerTableName + ".[" + i + "] "+ condition + " @" + i)));
            sb.Append(" ");

            return sb.ToString();
        }

        private static string AndTenantRelated(string tableName, bool isTenantRelated)
        {
            return isTenantRelated ? "{andTenantId:" + tableName +"}" : "";
        }

        private static string OrderBy(string columnName, string ownerTableName, bool desc)
        {
            return "ORDER BY " + ownerTableName + "." + columnName + (desc ? " DESC":"");
        }


        private static string WhereTenantRelated(string tableName, bool isTenantRelated)
        {
            return isTenantRelated ? "{whereTenantId:" + tableName + "}": "";
        }

        #endregion

        #region Private

        private static string Field(string tableName, string fieldName)
        {
            return tableName + ".[" + fieldName + "]";
        }

        #endregion
        
    }

    internal struct SqlInfo
    {
        public string TableName;
        public IList<string> TableColumns;
        public IList<string> HiddenTableColumns;
        public string VersionKeyType;
        public bool ReturnPrimaryKey;
        public string JoinTableName;
        public IList<string> JoinTableColumns;
        public IList<string> JoinPrimaryKeyNames;
        public bool TenantRelated;
        public IList<string> PrimaryKeyNames;
        public string VersionKeyName;
        public string VersionTableName;
        public string JoinVersionKeyName;
        public string JoinVersionTableName;
        public bool IsManyToMany;
        public IList<string> IdentityColumns;
        public IList<string> IdentityColumnsJoined;
        public string PrimaryKeyType;
        public bool Identity;
        public bool JoinIdentity;
    }
}