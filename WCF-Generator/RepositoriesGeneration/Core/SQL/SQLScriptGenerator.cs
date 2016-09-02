using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VersionedRepositoryGeneration.Generator.Heplers;
using VersionedRepositoryGeneration.Generator.Infrastructure;
using WCFGenerator.RepositoriesGeneration.Infrastructure;

namespace VersionedRepositoryGeneration.Generator.Core.SQL
{
    internal class SqlScriptGenerator
    {

        private const string _tempTable = "@TempValueDb";
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
            return GenerateSelectBy(info) + WhereTenantRelated(info.TableName, info.TenantRelated);
        }

        public static string GenerateSelectBy(SqlInfo info)
        {
            if (info.JoinTableColumns != null && !string.IsNullOrEmpty(info.JoinTableName))
            {
                // return select from table and join table
                return Select(info.TableColumns, info.TableName) + ","
                       + Fields(info.JoinTableColumns, info.JoinTableName) + " "
                       + From(info.TableName) + " "
                       + InnerJoin(info.TableName, info.PrimaryKeyName, info.JoinTableName, info.JoinPrimaryKeyName);// + " " + AndTenantRelated(info.TableName, info.TenantRelated);
            }
            // return select from table
            return Select(info.TableColumns, info.TableName) + " " + From(info.TableName); //+ " " + WhereTenantRelated(info.TableName, info.TenantRelated);
        }

        public static string GenerateInsert(SqlInfo info)
        {
            return Insert(info.TableColumns, info.TableName);
        }

        public static string GenerateInsertToTemp(SqlInfo info)
        {
            return "DECLARE " + _tempTable + " TABLE (ItemId uniqueidentifier);" +
                                                        "INSERT INTO "+ _tempTable + " " +
                                                        "SELECT " + Field(info.TableName, info.PrimaryKeyName) + " FROM " + GenerateTableName(info.TableName);
        }

        public static string GenerateWhere(IEnumerable<string> selectedFilters, SqlInfo info)
        {
            return Where(selectedFilters, info.TableName) + AndTenantRelated(info.TableName, info.TenantRelated);
        }

        public static string GenerateUpdate(SqlInfo info)
        {
            return Update(info.TableName) + " " + Set(info.TableColumns, info.TableName);
        }

        public static string GenerateUpdateJoin(SqlInfo info)
        {
            return Update(info.JoinTableName) + " "
                   + Set(info.JoinTableColumns, info.TableName) + " "
                   + InnerJoin(info.TableName, info.PrimaryKeyName, info.JoinTableName, info.JoinPrimaryKeyName);
        }

        public static string GenerateRemove(SqlInfo info)
        {
            if (info.JoinTableColumns != null && !string.IsNullOrEmpty(info.JoinTableName))
                return Delete(info.TableName) + " WHERE " + Field(info.TableName, info.PrimaryKeyName) + " IN (SELECT ItemId FROM " + _tempTable + ");" +
                   Delete(info.JoinTableName) + " WHERE " + Field(info.JoinTableName, info.JoinPrimaryKeyName) + " IN (SELECT ItemId FROM " + _tempTable + ");";

            return Delete(info.TableName);
        }

        #endregion

        #region VERSION TABLE

        public static string GenerateInsertToVersionTable(RepositoryInfo info, string repositoryName)
        {
            var classProperties = string.Join(",", info.Elements.Select(x => "[" + x.ToString() + "]"));
            var classValues = string.Join(",", info.Elements.Select(x => "@" + x.ToString()));

            if (info.JoinRepositoryInfo != null)
            {
                var baseInfo = info.JoinRepositoryInfo;
                var property = baseInfo.VersionKey;
                var type = SystemToSqlTypeMapper.GetSqlType(baseInfo.ClassName);
                var joinClassProperties = string.Join(",", baseInfo.Elements.Select(x => "[" + x.ToString() + "]"));
                var joinClassValues = string.Join(",", baseInfo.Elements.Select(x => "@" + x.ToString()));

                return "DECLARE @TempPKTable TABLE (" + property + " " + type + ");\n" +
                       "DECLARE @TempPK" + property + " " + type + ";\n" +
                       "INSERT INTO " + baseInfo.TableName + "(" + joinClassProperties + ")\n" +
                       "OUTPUT INSERTED." + property + " INTO @TempPKTable\n" +
                       "VALUES (" + joinClassValues + ")\n" +
                   "SELECT @TempPK" + property + " = " + property + " FROM @TempPKTable\n" +
                   "INSERT INTO " + repositoryName + "(" + classProperties + ")\n" +
                   "VALUES (" + classValues + ")\n" +
                   "SELECT " + property + " FROM @TempPKTable\n";
            }
            else
            {
                return "INSERT INTO " + repositoryName + "(" + classProperties + ")\n" +
                          "OUTPUT INSERTED." + info.VersionKey + "VALUES (" + classValues + ")";
            }
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

        private static string Insert(IEnumerable<string> columns, string ownerTableName)
        {
            var values = columns as string[] ?? columns.ToArray();
            return "INSERT INTO " + ownerTableName + "(" + string.Join(",", values.Select(e => "[" + e + "]")) + ") VALUES(" + string.Join(",", values.Select(e => "@" + e)) + ")";
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
            return "DELETE FROM" + GenerateTableName(tableName);
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
        public string PrimaryKeyName;
        public string JoinTableName;
        public IEnumerable<string> JoinTableColumns;
        public string JoinPrimaryKeyName;
        public bool TenantRelated;
    }
}