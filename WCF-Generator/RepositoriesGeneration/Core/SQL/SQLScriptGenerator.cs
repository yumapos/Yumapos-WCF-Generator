using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCFGenerator.RepositoriesGeneration.Core.SQL
{
    internal class SqlScriptGenerator
    {

        private const string _tempTable = "@Temp";
        private const string _tenantId = "TenantId";

        public static string GenerateTableName(string tableName)
        {
            tableName = tableName.Trim();

            var nametrue = string.Join(".", tableName.Split('.').Select(part => "[" + part.Trim('[', ']') + "]"));

            return nametrue;
        }

        #region Cache table

        public static string GenerateFields(SqlInfo info)
        {
            var columns = GetFullColumnList(info);
            return Fields(columns, info.TableName);
        }

        public static string GenerateValues(SqlInfo info)
        {
            var columns = GetFullColumnList(info);
            return Values(columns);
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
            var columnsWithoutSelectedPk = info.TableColumns.Where(c => info.SkipPrimaryKey.All(pk => pk != c)).ToList();
            if (info.TenantRelated && !columnsWithoutSelectedPk.Contains(_tenantId))
            {
                columnsWithoutSelectedPk.Add(_tenantId);
            }
            if (info.JoinTableColumns != null && !string.IsNullOrEmpty(info.JoinTableName))
            {
                var columnsJonedWithoutSelectedPk = info.JoinTableColumns.Where(c => info.SkipPrimaryKey.All(pk => pk != c)).ToList();
                if (info.TenantRelated && !columnsJonedWithoutSelectedPk.Contains(_tenantId))
                {
                    columnsJonedWithoutSelectedPk.Add(_tenantId);
                }
                var outputKey = info.ReturnPrimarayKey && string.IsNullOrEmpty(info.PrimaryKeyName) ? "" : "OUTPUT INSERTED." + info.PrimaryKeyName;

                // use pk from inherite model
                var values = columnsJonedWithoutSelectedPk.Select(c => c == info.JoinVersionKeyName ? info.VersionKeyName : c == info.JoinPrimaryKeyName ? info.PrimaryKeyName : c);
                var joinClassValues = Values(values);

                // return inset into table and join table
                return "INSERT INTO " + info.JoinTableName + "(" + Fields(columnsJonedWithoutSelectedPk, info.JoinTableName) + ") VALUES(" + joinClassValues + ")\r\n "
                     + "INSERT INTO " + info.TableName + "(" + Fields(columnsWithoutSelectedPk, info.TableName) + ") " + outputKey + " VALUES(" + Values(columnsWithoutSelectedPk) + ") ";
            }
            // return select from table
            return Insert(columnsWithoutSelectedPk, info.TableName, info.ReturnPrimarayKey ? info.PrimaryKeyName : null) + " ";
        }

        public static string GenerateInsertToTemp(SqlInfo info)
        {
            return "DECLARE " + _tempTable + " TABLE (ItemId uniqueidentifier);" +
                                                        "INSERT INTO "+ _tempTable + " " +
                                                        "SELECT " + Field(info.TableName, info.PrimaryKeyName) + " FROM " + info.TableName + " ";
        }

        public static string GenerateWhere(IEnumerable<string> selectedFilters, SqlInfo info)
        {
            return Where(selectedFilters, info.TableName) + AndTenantRelated(info.TableName, info.TenantRelated) + " ";
        }

        public static string GenerateWhereJoinPk( SqlInfo info)
        {
            return Where(new [] { info.JoinPrimaryKeyName }, info.JoinTableName) + AndTenantRelated(info.JoinTableName, info.TenantRelated) + " ";
        }


        public static string GenerateAnd(IEnumerable<string> selectedFilters, string ownerTable)
        {
            return And(selectedFilters, ownerTable);
        }

        public static string GenerateUpdate(SqlInfo info)
        {
            var columns = info.TableColumns.Where(c => info.SkipPrimaryKey.All(pk => pk != c)).ToList();
            return Update(info.TableName) + " " 
                    + Set(columns, info.TableName) + " " 
                    + From(info.TableName) + " ";
        }

        public static string GenerateUpdateJoin(SqlInfo info)
        {
            // use pk from inherite model
            var values = info.JoinTableColumns.Select(c => new KeyValuePair<string,string>(c,c == info.JoinVersionKeyName ? info.VersionKeyName : c == info.JoinPrimaryKeyName ? info.PrimaryKeyName : c));

            return Update(info.JoinTableName) + " "
                   + Set(values, info.JoinTableName) + " "
                   + From(info.JoinTableName) + " ";
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
            var columns = GetFullColumnList(info);
            var classProperties = Fields(columns, info.TableName);
            var classValues = Values(columns);

            if (info.JoinTableName != null)
            {
                var joinFields = GetFullJoinedColumnList(info);
                var joinClassProperties = Fields(joinFields, info.JoinTableName);
                // use versionId & primary key from inherite model
                var values = joinFields.Select(c => c == info.JoinVersionKeyName ? info.VersionKeyName : c == info.JoinPrimaryKeyName ? info.PrimaryKeyName : c).ToList();
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
            return "INSERT INTO " + ownerTableName + "(" + Fields(columns, ownerTableName) + ") " + outputKey + " VALUES(" + Values(columns) + ")";
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

        private static string From(string tableName)
        {
            return "FROM " + tableName;
        }
        private static string InnerJoin(string tableName, string tablePrimaryKey, string joinTableName, string joinTablePrimaryKey)
        {
            return "INNER JOIN " + joinTableName + " ON " + tableName + ".[" + tablePrimaryKey + "] = " + joinTableName + ".[" + joinTablePrimaryKey + "]";
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
            return isTenantRelated ? "{andTenantId:" + tableName +"}" : "";
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

        private static List<string> GetFullColumnList(SqlInfo info)
        {
            var list = info.TableColumns.ToList();
            if (info.TenantRelated && !list.Contains(_tenantId))
            {
                list.Add(_tenantId);
            }
            return list;
        }

        private static List<string> GetFullJoinedColumnList(SqlInfo info)
        {
            var list = info.JoinTableColumns.ToList();
            if (info.TenantRelated && !list.Contains(_tenantId))
            {
                list.Add(_tenantId);
            }
            return list;
        }

        #endregion
    }

    internal struct SqlInfo
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
        public string JoinVersionKeyName;
        public string JoinVersionTableName;
        public bool IsManyToMany;
        public IEnumerable<string> SkipPrimaryKey;
        public string PrimaryKeyType;
    }
}