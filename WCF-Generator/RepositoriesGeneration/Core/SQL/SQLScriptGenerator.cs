using System.Collections.Generic;
using System.Linq;
using System.Text;
using VersionedRepositoryGeneration.Generator.Heplers;

namespace WCFGenerator.RepositoriesGeneration.Core.SQL
{
    internal class SqlScriptGenerator
    {

        private const string _tempTable = "@Temp";
        private const string _tempId = "@TempPKItemId";
        private const string _columns = "{columns}";
        private const string _values = "{values}";

        #region Cache table

        public static string GenerateFields(SqlInfo info)
        {
            return Fields(info.TableColumns, info.TableName) + _columns;
        }

        public static string GenerateValues(SqlInfo info)
        {
            return Values(info.TableColumns) + _values;
        }

        public static string GenerateSelectAll(SqlInfo info)
        {
            return GenerateSelectBy(info) + " " + WhereTenantRelated(info.TableName, info.TenantRelated) + " ";
        }

        public static string GenerateSelectBy(SqlInfo info)
        {
            if (info.JoinTableColumns != null && !string.IsNullOrEmpty(info.JoinTableName))
            {
                // return select from table and join table
                return Select(info.TableColumns, info.TableName) + ","
                       + Fields(info.JoinTableColumns, info.JoinTableName) + " "
                       + From(info.TableName) + " "
                       + InnerJoin(info.TableName, info.PrimaryKeyName, info.JoinTableName, info.JoinPrimaryKeyName) + " ";
            }
            // return select from table
            return Select(info.TableColumns, info.TableName) + " " + From(info.TableName) + " ";
        }

        public static string GenerateInsert(SqlInfo info)
        {
            if (info.JoinTableColumns != null && !string.IsNullOrEmpty(info.JoinTableName))
            {
                var columns = info.TableColumns.ToList();
                var columnsJoned = info.JoinTableColumns.ToList();
                var outputKey = info.ReturnPrimarayKey && string.IsNullOrEmpty(info.PrimaryKeyName) ? "" : "OUTPUT INSERTED." + info.PrimaryKeyName;

                // return inset into table and join table
                return "DECLARE " + _tempTable + " TABLE (ItemId uniqueidentifier); DECLARE @TempPKItemId uniqueidentifier;"
                       + "INSERT INTO " + info.JoinTableName + "(" + Fields(columnsJoned, info.JoinTableName) + _columns + ") OUTPUT INSERTED." + info.JoinPrimaryKeyName + " VALUES(" + Values(columnsJoned) + _values + ") "
                       + "SELECT " + _tempId + " = " + info.JoinPrimaryKeyName + " FROM " + _tempTable + " "
                       + "INSERT INTO " + info.TableName + "(" + Fields(columns, info.TableName) + _columns + ") " + outputKey + " VALUES(" + _tempId +","+ Values(columns.Where(c => c != info.PrimaryKeyName)) + _values + ") ";
            }
            // return select from table
            return Insert(info.TableColumns, info.TableName, info.ReturnPrimarayKey ? info.PrimaryKeyName : null) + " ";
        }

        public static string GenerateInsertToTemp(SqlInfo info)
        {
            return "DECLARE " + _tempTable + " TABLE (ItemId uniqueidentifier);" +
                                                        "INSERT INTO "+ _tempTable + " " +
                                                        "SELECT " + Field(info.TableName, info.PrimaryKeyName) + " FROM " + GenerateTableName(info.TableName) + " ";
        }

        public static string GenerateWhere(IEnumerable<string> selectedFilters, SqlInfo info)
        {
            return Where(selectedFilters, info.TableName) + AndTenantRelated(info.TableName, info.TenantRelated) + " ";
        }

        public static string GenerateWhere(string selectedFilter, SqlInfo info)
        {
            return Where(new [] { selectedFilter}, info.TableName) + AndTenantRelated(info.TableName, info.TenantRelated) + " ";
        }


        public static string GenerateAnd(IEnumerable<string> selectedFilters, string ownerTable)
        {
            return And(selectedFilters, ownerTable);
        }

        public static string GenerateUpdate(SqlInfo info)
        {
            return Update(info.TableName) + " " 
                    + Set(info.TableColumns, info.TableName) + " " 
                    + From(info.TableName) + " ";
        }

        public static string GenerateUpdateJoin(SqlInfo info)
        {
            return Update(info.JoinTableName) + " "
                   + Set(info.JoinTableColumns, info.TableName) + " "
                   + From(info.JoinTableName) + " "
                   + InnerJoin(info.TableName, info.PrimaryKeyName, info.JoinTableName, info.JoinPrimaryKeyName) + " ";
        }

        public static string GenerateRemove(SqlInfo info)
        {
            if (info.JoinTableColumns != null && !string.IsNullOrEmpty(info.JoinTableName))
                return Delete(info.TableName) + " WHERE " + Field(info.TableName, info.PrimaryKeyName) + " IN (SELECT ItemId FROM " + _tempTable + ");" +
                   Delete(info.JoinTableName) + " WHERE " + Field(info.JoinTableName, info.JoinPrimaryKeyName) + " IN (SELECT ItemId FROM " + _tempTable + ");" + " ";

            return Delete(info.TableName) + " ";
        }

        #endregion

        #region VERSION TABLE

        public static string GenerateInsertToVersionTable(SqlInfo info)
        {
            var classProperties = Fields(info.TableColumns, info.TableName);
            var classValues = Values(info.TableColumns);

            if (info.JoinTableName != null)
            {
                var joinClassProperties = Fields(info.JoinTableColumns, info.JoinTableName);
                var joinClassValues = Values(info.JoinTableColumns);

                return "DECLARE @TempPKTable TABLE (" + info.VersionKeyName + " " + info.VersionKeyType + ");\n" +
                       "DECLARE @TempPK" + info.VersionKeyName + " " + info.VersionKeyType + ";\n" +
                       "INSERT INTO " + info.JoinTableName + "(" + joinClassProperties + ")\n" +
                       "OUTPUT INSERTED." + info.VersionKeyName + " INTO @TempPKTable\n" +
                       "VALUES (" + joinClassValues + ")\n" +
                   "SELECT @TempPK" + info.VersionKeyName + " = " + info.VersionKeyName + " FROM @TempPKTable\n" +
                   "INSERT INTO " + info.VersionTableName + "(" + classProperties + ")\n" +
                   "VALUES (" + classValues + ")\n" +
                   "SELECT " + info.VersionKeyName + " FROM @TempPKTable\n";
            }
            if(!info.IsManyToMany)
            {
                return "INSERT INTO " + info.VersionTableName + "(" + classProperties + ")\n" +
                       "OUTPUT INSERTED." + info.VersionKeyName + "VALUES (" + classValues + ")";
            }

            return "INSERT INTO " + info.VersionTableName + "(" + classProperties + ")\n" +
                       "VALUES (" + classValues + ")";
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

        private static string Select(IEnumerable<string> columns, string table)
        {
            return "SELECT " + Fields(columns, table);
        }

        private static string Insert(IEnumerable<string> tableColumns, string ownerTableName, string primaryKey = null)
        {
            var columns = tableColumns.ToList();
            var outputKey = string.IsNullOrEmpty(primaryKey) ? "" : "OUTPUT INSERTED." + primaryKey;
            return "INSERT INTO " + ownerTableName + "(" + Fields(columns, ownerTableName) + _columns + ") " + outputKey + " VALUES(" + Values(columns) + _values +")";
        }

        private static string Update(string tableName)
        {
            return "UPDATE " + GenerateTableName(tableName);
        }
        
        private static string Set(IEnumerable<string> parameters, string ownerTableName)
        {
            var sb = new StringBuilder();

            sb.Append("SET ");
            sb.Append(string.Join(",", parameters.Select(i => ownerTableName + ".[" + i + "] = @" + i)));

            return sb.ToString();
        }

        private static string Delete(string tableName)
        {
            return "DELETE FROM " + GenerateTableName(tableName);
        }

        private static string From(string tableName)
        {
            return "FROM " + GenerateTableName(tableName);
        }
        private static string InnerJoin(string tableName, string tablePrimaryKey, string joinTableName, string joinTablePrimaryKey)
        {
            return "INNER JOIN " + GenerateTableName(joinTableName) + " ON " + GenerateTableName(tableName) + ".[" + tablePrimaryKey + "] = " + GenerateTableName(joinTableName) + ".[" + joinTablePrimaryKey + "]";
        }

        private static string Where(IEnumerable<string> parameters, string ownerTableName)
        {
            var sb = new StringBuilder();

            sb.Append("WHERE ");
            sb.Append(string.Join(" AND ", parameters.Select(i => ownerTableName + ".[" + i + "] = @" + i)));

            return sb.ToString();
        }

        private static string And(IEnumerable<string> parameters, string ownerTableName)
        {
            var sb = new StringBuilder();

            sb.Append("AND ");
            sb.Append(string.Join(" AND ", parameters.Select(i => ownerTableName + ".[" + i + "] = @" + i)));

            return sb.ToString();
        }

        private static string AndTenantRelated(string tableName, bool isTenantRelated)
        {
            return isTenantRelated ? "{andTenantId:" + GenerateTableName(tableName) +"}" : "";
        }

        private static string WhereTenantRelated(string tableName, bool isTenantRelated)
        {
            return isTenantRelated ? "{whereTenantId:" + GenerateTableName(tableName) + "}": "";
        }

        #endregion

        #region Private

        private static readonly Dictionary<string, string> TableNameCache = new Dictionary<string, string>();

        private static string GenerateTableName(string tableName)
        {
            tableName = tableName.Trim();
            string cachetableName;

            TableNameCache.TryGetValue(tableName, out cachetableName);

            if (cachetableName != null)
            {
                return cachetableName;
            }

            var nametrue = string.Join(".", tableName.Trim('[', ']').Split('.').Select(part => "[" + part + "]"));

            TableNameCache.Add(tableName, nametrue);

            return nametrue;
        }

        private static string Field(string tableName, string fieldName)
        {
            return GenerateTableName(tableName) + ".[" + fieldName + "]";
        }

        #endregion

    }

    public struct SqlInfo
    {
        public string TableName;
        public IEnumerable<string> TableColumns;
        public string VersionKeyType;
        public bool ReturnPrimarayKey;
        public string JoinTableName;
        public IEnumerable<string> JoinTableColumns;
        public string JoinPrimaryKeyName;
        public bool TenantRelated;
        public string PrimaryKeyName;
        public string VersionKeyName;
        public string VersionTableName;
        public bool IsManyToMany;
    }
}