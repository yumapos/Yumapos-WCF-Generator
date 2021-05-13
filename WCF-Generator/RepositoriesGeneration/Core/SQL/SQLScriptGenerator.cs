using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCFGenerator.RepositoriesGeneration.Enums;
using WCFGenerator.RepositoriesGeneration.Helpers;
using WCFGenerator.RepositoriesGeneration.Infrastructure;

namespace WCFGenerator.RepositoriesGeneration.Core.SQL
{
    internal class SqlScriptGenerator : ISqlScriptGenerator
    {
        private const string _tempTable = "@Temp";
        private const string _sliceDateColumnName = "Modified";
        private const string _syncStateColumnName = "SyncState";
        private const string _versionTableAlias1 = "versionTable1";
        private const string _joinVersionTableAlias1 = "joinVersionTable1";

        public static string GenerateTableName(string tableName)
        {
            tableName = tableName.Trim();

            var nametrue = string.Join(".", tableName.Split('.').Select(part => "[" + part.Trim('[', ']') + "]"));

            return nametrue;
        }

        #region Cache table

        public string GenerateFields(SqlInfo info)
        {
            return Fields(info.TableColumns.Select(c => c.Name), info.TableName);
        }

        public string GenerateValues(SqlInfo info)
        {
            return Values(info.TableColumns.Select(c => c.Name));
        }

        public string GenerateSelectAll(SqlInfo info)
        {
            return GenerateSelectBy(info, null) + " " + WhereTenantAndStoreRelated(info.TableName, info.TenantRelated, info.IsStoreDependent) + " ";
        }

        public string GenerateSelectBy(SqlInfo info, int? topNumber = null)
        {
            if (info.JoinTableColumns != null && !string.IsNullOrEmpty(info.JoinTableName))
            {
                // return select from table and join table
                return Select(info.TableColumns.Select(c => c.Name), info.TableName, topNumber) + ","
                       + Fields(info.JoinTableColumns.Select(c => c.Name), info.JoinTableName) + " "
                       + From(info.TableName) + " "
                       + InnerJoin(info.TableName, info.PrimaryKeyNames.First(), info.JoinTableName, info.JoinPrimaryKeyNames.First()) + " ";//TODO FIX TO MANY KEYS
            }
            // return select from table
            return Select(info.TableColumns.Select(c => c.Name), info.TableName, topNumber) + " " + From(info.TableName) + " ";
        }

        public string GenerateInsert(SqlInfo info)
        {
            // Skip PK if identity = true
            var columns = info.TableColumns
                .Concat(info.HiddenTableColumns)
                .Select(c => c.Name)
                .Except(info.IdentityColumns)
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
                .Concat(info.HiddenTableColumns)
                .Select(c => c.Name)
                .Except(info.IdentityColumnsJoined)
                .ToList();

            var valuesJoined = columnsJoned.Select(c => c == info.JoinVersionKeyName ? info.VersionKeyName : c == info.JoinPrimaryKeyNames.First() ? info.PrimaryKeyNames.First() : c).ToList();
            
            var insertJoinedTable = InsertWithJoined(columnsJoned, valuesJoined, info.JoinTableName, info.JoinPrimaryKeyNames.First(), info.PrimaryKeyType, columns, columns, info.TableName, info.PrimaryKeyNames.First());

            // return inset into table and join table
            return insertJoinedTable;
        }

        public string GenerateInsertManyQueryTemplate(SqlInfo info, RepositoryType repositoryType)
        {
            var columns = info.TableColumns.Select(c => c.Name)
                .Except(info.IdentityColumns)
                .ToList();

            var insertSql = !info.ReturnPrimaryKey
                ? InsertMany(columns, info.HiddenTableColumns.Select(c => c.Name), repositoryType == RepositoryType.Version ? info.VersionTableName : info.TableName)
                : InsertMany(columns, info.HiddenTableColumns.Select(c => c.Name), repositoryType == RepositoryType.Version ? info.VersionTableName : info.TableName, info.PrimaryKeyNames.First());

            // Return if have not joined table 
            if (info.JoinTableColumns == null)
            {
                return insertSql;
            }

            // Skip PK if identity of joined table = true
            var columnsJoned = info.JoinTableColumns.Select(c => c.Name)
                .Except(info.IdentityColumnsJoined)
                .ToList();

            var insertJoinedTable = InsertManyWithJoined(columnsJoned, repositoryType == RepositoryType.Version ? info.JoinVersionTableName : info.JoinTableName, columns, repositoryType == RepositoryType.Version ? info.VersionTableName : info.TableName, info.HiddenTableColumns.Select(c => c.Name).ToList());

            // return inset into table and join table
            return insertJoinedTable;
        }

        public string GenerateInsertManyValuesTemplate(SqlInfo info)
        {
            var sb = new StringBuilder();

            var columnsPart = InsertManyValuesTemplate(info.TableColumns);
            var hidenColumns = info.HiddenTableColumns.Any()
                ? "," + Values(info.HiddenTableColumns.Select(c => c.Name))
                : "";

            sb.Append("(");
            sb.Append(columnsPart);
            sb.Append(hidenColumns);
            sb.Append(")");

            return sb.ToString();
        }

        public string GenerateInsertManyJoinedValuesTemplate(SqlInfo info)
        {
            var sb = new StringBuilder();

            var columns = InsertManyValuesTemplate(info.JoinTableColumns);
            var hidenColumns = info.HiddenTableColumns.Any()
                ? "," + Values(info.HiddenTableColumns.Select(c => c.Name))
                : "";

            sb.Append("(");
            sb.Append(columns);
            sb.Append(hidenColumns);
            sb.Append(")");

            return sb.ToString();
        }

        public string GenerateInsertToTemp(SqlInfo info)
        {
            return "DECLARE " + _tempTable + " TABLE (ItemId uniqueidentifier);" +
                                                        "INSERT INTO "+ _tempTable + " " +
                                                        "SELECT " + Field(info.TableName, info.PrimaryKeyNames.First()) + " FROM " + info.TableName + " ";//TODO FIX TO MANY KEYS
        }

        public string GenerateWhere(IEnumerable<ParameterInfo> parameters, SqlInfo info)
        {
            return Where(parameters, info.TableName) + AndTenantAndStoreRelated(info.TableName, info.TenantRelated, info.IsStoreDependent) + " ";
        }


        public string GenerateWhere(IEnumerable<string> selectedFilters, SqlInfo info)
        {
            return Where(selectedFilters, info.TableName) + AndTenantAndStoreRelated(info.TableName, info.TenantRelated, info.IsStoreDependent) + " ";
        }

        public string GenerateWhereTemplate(IEnumerable<string> selectedFilters, SqlInfo info)
        {
            var sb = new StringBuilder();
            sb.Append("WHERE ");
            sb.Append(string.Join(" AND ", selectedFilters.Select(i => info.TableName + ".[" + i + "] = @" + i + "{0}")));
            var tenantRelatedString = info.TenantRelated ? "{{andTenantId:" + info.TableName + "}}" : "";
            var storeRelatedString = info.IsStoreDependent ? "{{andStoreIds:" + info.TableName + "}}" : "";
            sb.Append(tenantRelatedString + storeRelatedString);

            return sb.ToString();
        }


        public string GenerateWhere(IEnumerable<string> commonFilters, IEnumerable<string> datesFilters, SqlInfo info)
        {
            if(commonFilters.Any())
            {
                return Where(commonFilters, info.TableName) + AndTenantAndStoreRelated(info.TableName, info.TenantRelated, info.IsStoreDependent) + " AND " + WhereDates(datesFilters, info.TableName);
            }
            return " WHERE " + WhereDates(datesFilters, info.TableName) + AndTenantAndStoreRelated(info.TableName, info.TenantRelated, info.IsStoreDependent);
        }

        public string GenerateWhereJoinPk( SqlInfo info)
        {
            return Where(new [] { info.JoinPrimaryKeyNames.First() }, info.JoinTableName) + AndTenantAndStoreRelated(info.JoinTableName, info.TenantRelated, info.IsStoreDependent) + " "; //TODO FIX TO MANY KEYS
        }


        public string GenerateAnd(string selectedFilter, string ownerTable, string condition = "=")
        {
            return And(new []{selectedFilter}, ownerTable, condition);
        }

        public string GenerateOrderBySliceDate(SqlInfo info)
        {
            return OrderBy("Modified", info.JoinTableName, true);
        }

        public string GenerateUpdate(SqlInfo info)
        {
            var columns = info.UpdateTableColumns
                .Where(c => !c.IgnoreOnUpdate).Select(c => c.Name)
                .Where(c => info.IdentityColumns.All(pk => pk != c) && !info.PrimaryKeyNames.Contains(c))
                .ToList();

            var update = $"{Update(info.TableName)} {Set(columns, info.TableName)}";

            if (info.IsSyncStateEnabled)
            {
                update += $",{SetSyncState(info.TableName, false)}";
            }

            return $"{update} {From(info.TableName)} ";
        }

        private static string SetSyncState(string table, bool value)
        {
            var syncState = value ? "1" : "0";
            return $"{table}.[{_syncStateColumnName}] = '{syncState}'";
        }

        public string GenerateUpdateJoin(SqlInfo info)
        {
            // use pk from inherit model
            var values = info.JoinTableColumns.Where(c => !c.IgnoreOnUpdate).Select(c => c.Name)
                .Except(info.IdentityColumnsJoined)
                .Select(c => new KeyValuePair<string,string>(c,c == info.JoinVersionKeyName ? info.VersionKeyName : c == info.JoinPrimaryKeyNames.First() ? info.PrimaryKeyNames.First() : c));//TODO FIX TO MANY KEYS

            return Update(info.JoinTableName) + " "
                   + Set(values, info.JoinTableName) + " "
                   + From(info.JoinTableName) + " ";
        }


        public string GenerateUpdateMany(SqlInfo info)
        {
            var columns = info.UpdateTableColumns
                .Where(propertyInfo => !propertyInfo.IgnoreOnUpdate
                                       && !info.PrimaryKeyNames.Contains(propertyInfo.Name)
                                       && !info.IdentityColumns.Contains(propertyInfo.Name));

            var columnsString = UpdateManyValuesTemplate(columns);

            var update = Update(info.TableName) + " SET " + columnsString;

            if (info.IsSyncStateEnabled)
            {
                update += $",{SetSyncState(info.TableName, false)}";
            }

            var updateManySql = $"{update} {GenerateWhereTemplate(info.PrimaryKeyNames, info)}";

            return updateManySql;
        }

        public string GenerateUpdateManyJoined(SqlInfo info)
        {
            var columns = info.JoinTableColumns
                .Where(propertyInfo => !propertyInfo.IgnoreOnUpdate
                                       && !info.PrimaryKeyNames.Contains(propertyInfo.Name)
                                       && !info.IdentityColumns.Contains(propertyInfo.Name));

            var columnsString = UpdateManyValuesTemplate(columns);

            var updateManySql = Update(info.JoinTableName) + " SET " + columnsString + " " + GenerateWhereTemplate(info.JoinPrimaryKeyNames, info);

            return updateManySql;
        }


        public string GenerateRemove(SqlInfo info)
        {
            // base type of repository and "isDeleted" flag - set IsDeleted = true
            if (info.IsDeleted && info.VersionTableName == null)
            {
                string syncSql = info.IsSyncStateEnabled || info.IsSyncStateFiledExists ? $", {_syncStateColumnName} = 0 " : " "; // SyncState always 0 on remove, can't be override
                if (info.JoinTableColumns != null && !string.IsNullOrEmpty(info.JoinTableName))
                {
                    return Update(info.JoinTableName) + " SET IsDeleted = 1" + syncSql;
                }
                else
                {
                    return Update(info.TableName) + " SET IsDeleted = 1" + syncSql;
                }
            }
            else
            {
                // base or version repository without "isDeleted" flag
                if (info.JoinTableColumns != null && !string.IsNullOrEmpty(info.JoinTableName))
                {
                    return Delete(info.TableName) + " WHERE " + Field(info.TableName, info.PrimaryKeyNames.First()) + " IN (SELECT ItemId FROM " + _tempTable + ");" + //TODO FIX TO MANY KEYS
                           Delete(info.JoinTableName) + " WHERE " + Field(info.JoinTableName, info.JoinPrimaryKeyNames.First()) + " IN (SELECT ItemId FROM " + _tempTable + ");" + " "; //TODO FIX TO MANY KEYS
                }
                else
                {
                    return Delete(info.TableName) + " ";
                }
            }
        }

        #endregion

        #region VERSION TABLE

        public string GenerateInsertToVersionTable(SqlInfo info)
        {
            var columns = info.TableColumns.Concat(info.HiddenTableColumns).Select(c => c.Name).ToList();
            var classProperties = Fields(columns, info.TableName);
            var classValues = Values(columns);

            if (info.JoinTableName != null)
            {
                var joinFields = info.JoinTableColumns.Concat(info.HiddenTableColumns).Select(c => c.Name).ToList();
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

        public string GenerateSelectByToVersionTable(SqlInfo info)
        {
            if (info.JoinTableColumns != null && !string.IsNullOrEmpty(info.JoinTableName))
            {
                // return select from table and join table
                return Select(info.TableColumns.Select(c=>c.Name), info.VersionTableName) + ","
                       + Fields(info.JoinTableColumns.Select(c => c.Name), info.JoinVersionTableName) + " "
                       + From(info.VersionTableName) + " "
                       + (info.IsVersionTableJoined()
                       ? InnerJoin(info.VersionTableName, info.VersionKeyName, info.JoinVersionTableName, info.JoinVersionKeyName)
                       : InnerJoin(info.VersionTableName, info.PrimaryKeyNames.First(), info.JoinVersionTableName, info.JoinPrimaryKeyNames.First())) + " ";//TODO FIX TO MANY KEYS
            }
            // return select from table
            return Select(info.TableColumns.Select(c => c.Name), info.VersionTableName) + " " + From(info.VersionTableName) + " ";
        }

        public string GenerateSelectByKeyAndSliceDateToVersionTable(SqlInfo info)
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
                return Select(info.TableColumns.Select(c => c.Name), info.VersionTableName) + " FROM " + selectMaxModifiedJoin + " " + join1 + " " + join2;
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

                return Select(info.TableColumns.Select(c => c.Name), info.VersionTableName) + " FROM " + selectMaxModified + " " + join1;
            }
        }

        public string GenerateWhereVersions(IEnumerable<string> selectedFilters, SqlInfo info)
        {
            return Where(selectedFilters, info.VersionTableName) + AndTenantAndStoreRelated(info.VersionTableName, info.TenantRelated, info.IsStoreDependent) + " ";
        }

        public string GenerateWhereVersionsWithAlias(IEnumerable<string> selectedFilters, SqlInfo info)
        {
            return Where(selectedFilters, _versionTableAlias1) + AndTenantAndStoreRelated(_versionTableAlias1, info.TenantRelated, info.IsStoreDependent) + " ";
        }

        public string GenerateAndVersionsWithAlias(string selectedFilter, SqlInfo info, string condition = "=")
        {
            var tableAlias = info.JoinVersionTableName != null ? _joinVersionTableAlias1 : _versionTableAlias1;
            return And(new[] { selectedFilter }, tableAlias, condition);
        }

        public string GenerateWhereJoinPkVersion(SqlInfo info)
        {
            return Where(new[] { info.JoinPrimaryKeyNames.First() }, info.JoinVersionTableName) + AndTenantAndStoreRelated(info.JoinVersionTableName, info.TenantRelated, info.IsStoreDependent) + " "; //TODO FIX TO MANY KEYS
        }

        public string GenerateInsertOrUpdate(List<ParameterInfo> primaryKeys, SqlInfo info)
        {
            var insert = GenerateInsert(info);
            var where = GenerateWhere(primaryKeys, info);
            var conflict = " IF @@ROWCOUNT = 0 BEGIN ";
            var update = GenerateUpdate(info);
            return update + " "+ where + conflict + insert + " END";
        }

        public string GenerateNoCheckConstraint(SqlInfo sqlInfo, RepositoryType repositoryType)
        {
            var table = repositoryType == RepositoryType.Version ? sqlInfo.VersionTableName : sqlInfo.TableName;
            return $"ALTER TABLE {table} NOCHECK CONSTRAINT ALL";
        }

        public string GenerateCheckConstraint(SqlInfo sqlInfo, RepositoryType repositoryType)
        {
            var table = repositoryType == RepositoryType.Version ? sqlInfo.VersionTableName : sqlInfo.TableName;
            return $"ALTER TABLE {table} CHECK CONSTRAINT ALL";
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

        private static string InsertManyValuesTemplate(IEnumerable<PropertyInfo> columns)
        {
            var columnList = columns.ToList();
            var sb = new StringBuilder();
            var index = 1;

            for (var i = 0; i < columnList.Count; i++)
            {
                var c = columnList[i];
                if (c.IsParameter)
                {
                    sb.Append($"@{c.Name}{{0}}");
                }
                else
                {
                    sb.Append($"'{{{index}}}'");
                    index++;
                }

                if (i < columnList.Count - 1)
                {
                    sb.Append(",");
                }
            }

            return sb.ToString();
        }

        private static string UpdateManyValuesTemplate(IEnumerable<PropertyInfo> columns)
        {
            var columnList = columns.ToList();
            var sb = new StringBuilder();
            var index = 1;

            for (var i = 0; i < columnList.Count; i++)
            {
                var c = columnList[i];
                if (c.IsParameter)
                {
                    sb.Append($"{c.Name} = @{c.Name}{{0}}");
                }
                else
                {
                    sb.Append($"{c.Name} = '{{{index}}}'");
                    index++;
                }

                if (i < columnList.Count - 1)
                {
                    sb.Append(",");
                }
            }

            return sb.ToString();
        }

        private static string Select(IEnumerable<string> columns, string table, int? topNumber = null)
        {
            return "SELECT " + (topNumber.HasValue ? " TOP " + topNumber : "") + Fields(columns, table);
        }

        private static string Insert(IEnumerable<string> tableColumns, string ownerTableName)
        {
            var columns = tableColumns.ToList();
            return "INSERT INTO " + ownerTableName + "(" + Fields(columns, ownerTableName) + ")  VALUES(" + Values(columns) + ") ";
        }

        private static string InsertMany(IEnumerable<string> tableColumns, IEnumerable<string> hiddenTableColumns, string ownerTableName, int index = 0)
        {
            var hiddenColumns = hiddenTableColumns.ToList();
            var columns = tableColumns.ToList();
            return "INSERT INTO " + ownerTableName + "(" + Fields(columns.Concat(hiddenColumns), ownerTableName) + ")  VALUES {" + index + "}";
        }

        private static string Insert(IEnumerable<string> tableColumns, string ownerTableName, string returnInserted)
        {
            var columns = tableColumns.ToList();
            var outputKey = "OUTPUT INSERTED." + returnInserted;
            return "INSERT INTO " + ownerTableName + "(" + Fields(columns, ownerTableName) + ") " + outputKey + " VALUES(" + Values(columns) + ") ";
        }

        private static string InsertMany(IEnumerable<string> tableColumns, IEnumerable<string> hiddenTableColumns, string ownerTableName, string returnInserted)
        {
            var hiddenColumns = hiddenTableColumns.ToList();
            var columns = tableColumns.ToList();
            var outputKey = "OUTPUT INSERTED." + returnInserted;
            return "INSERT INTO " + ownerTableName + "(" + Fields(columns.Concat(hiddenColumns), ownerTableName) + ") " + outputKey + " VALUES {0}";
        }

        private static string InsertWithJoined(List<string> joinedTableColumns, List<string> joinedTableValues, string joinedTableName, string joinedPkColumn, string joinedPkType, List<string> tableColumns, List<string> tableValues, string tableName, string pkColumn)
        {
            var tableForSave = "TempTable";
            var declareTable = "DECLARE @" + tableForSave + " TABLE (" + joinedPkColumn + " " + joinedPkType + ");";
            var insertToJoined = "INSERT INTO " + joinedTableName + "(" + Fields(joinedTableColumns, joinedTableName) + ") " + "OUTPUT INSERTED." + joinedPkColumn + (string.IsNullOrEmpty(tableForSave) ? "" : " INTO @" + tableForSave) + " VALUES(" + Values(joinedTableValues) + ");";

            var tempValue = "TempId" ;
            var insertedValue = "DECLARE @" + tempValue + " " + joinedPkType + "; SELECT @" + tempValue + " = "+ joinedPkColumn + " FROM @" + tableForSave + ";";

            var insert = "INSERT INTO " + tableName + "(" + Fields(tableColumns, tableName) + ") " + "OUTPUT INSERTED." + pkColumn + (string.IsNullOrEmpty(tableForSave) ? "" : " INTO @" + tableForSave) + " VALUES(" + Values(tableValues.Select(v=> v == pkColumn ? tempValue : v)) + ");";
            var selectId = "SELECT " + joinedPkColumn + " FROM @" + tableForSave + ";";
            return declareTable + insertToJoined + insertedValue + insert + selectId;
        }

        private static string InsertManyWithJoined(List<string> joinedTableColumns, string joinedTableName, List<string> tableColumns, string tableName, List<string> hiddenTableColumns)
        {
            var sb = new StringBuilder();
            var insertToJoined = InsertMany(joinedTableColumns, hiddenTableColumns, joinedTableName);
            var insert = InsertMany(tableColumns, hiddenTableColumns, tableName, 1);

            sb.Append(insertToJoined);
            sb.Append(";");
            sb.Append(insert);

            return sb.ToString();
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
        private static string Where(IEnumerable<ParameterInfo> parameters, string ownerTableName)
        {
            var sb = new StringBuilder();
            sb.Append("WHERE ");

            foreach (var parameter in parameters)
            {
                if (parameter == null)
                {
                    sb.Append("\"INVALID FILTERKEY\"");
                    break;
                }
                if (parameter.NeedGeneratePeriod)
                {
                    sb.Append($" AND {ownerTableName}.[{parameter.Name}] >= @start{parameter.Name}");
                    sb.Append($" AND {ownerTableName}.[{parameter.Name}] < @end{parameter.Name}");
                }
                else if (parameter.IsNullable)
                {
                    sb.Append($" AND (({ownerTableName}.[{parameter.Name}] IS NULL AND @{parameter.Name} IS NULL) OR {ownerTableName}.[{parameter.Name}] = @{parameter.Name})");
                }
                else
                {
                    sb.Append($" AND {ownerTableName}.[{parameter.Name}] = @{parameter.Name}");
                }
            }
            return sb.ToString().Replace("WHERE  AND ", "WHERE ");

        }

        private static string Where(IEnumerable<string> parameters, string ownerTableName)
        {
            var sb = new StringBuilder();

            sb.Append("WHERE ");
            sb.Append(string.Join(" AND ", parameters.Select(i => ownerTableName + ".[" + i + "] = @" + i)));

            return sb.ToString();
        }

        private static string WhereDates(IEnumerable<string> parameters, string ownerTableName)
        {
            var sb = new StringBuilder();

            sb.Append(string.Join(" AND ", parameters.Select(i => ownerTableName + ".[" + i + "] >= @start" + i)));
            sb.Append(" AND ");
            sb.Append(string.Join(" AND ", parameters.Select(i => ownerTableName + ".[" + i + "] < @end" + i)));

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

        private static string AndTenantAndStoreRelated(string tableName, bool isTenantRelated, bool isStoreRelated)
        {
            var tenantRelatedString =  isTenantRelated ? "{andTenantId:" + tableName +"}" : "";
            var storeRelatedString = isStoreRelated ? "{andStoreIds:" + tableName + "}" : "";
            return tenantRelatedString + storeRelatedString;
        }

        private static string OrderBy(string columnName, string ownerTableName, bool desc)
        {
            return "ORDER BY " + ownerTableName + "." + columnName + (desc ? " DESC":"");
        }


        private static string WhereTenantAndStoreRelated(string tableName, bool isTenantRelated, bool isStoreRelated)
        {
            var tenantRelatedString = isTenantRelated
                ? "{whereTenantId:" + tableName + "}"
                : "";
            var storeRelatedString = isStoreRelated ? "{andStoreIds:" + tableName + "}" : "";
            return tenantRelatedString + storeRelatedString;
        }

        #endregion

        #region Private

        private static string Field(string tableName, string fieldName)
        {
            return tableName + ".[" + fieldName + "]";
        }

        #endregion

        public SqlInfo GetTableInfo(SqlInfo repositoryInfo)
        {
            repositoryInfo.TableName = GenerateTableName(repositoryInfo.TableName);
            repositoryInfo.VersionTableName = repositoryInfo.VersionTableName != null ? GenerateTableName(repositoryInfo.VersionTableName) : null;
            repositoryInfo.JoinTableName = repositoryInfo.JoinTableName != null ? GenerateTableName(repositoryInfo.JoinTableName) : null;
            repositoryInfo.JoinVersionTableName = repositoryInfo.JoinVersionTableName != null ? GenerateTableName(repositoryInfo.JoinVersionTableName) : null;
            repositoryInfo.VersionKeyType = repositoryInfo.VersionKeyType != null ? SystemToSqlTypeMapper.GetSqlType(repositoryInfo.VersionKeyType) : null;
            repositoryInfo.PrimaryKeyType = repositoryInfo.PrimaryKeyType != null ? SystemToSqlTypeMapper.GetSqlType(repositoryInfo.PrimaryKeyType) : null;

            return repositoryInfo;
        }
    }

    internal struct SqlInfo
    {
        public string TableName;
        public IList<PropertyInfo> TableColumns;
        public IList<PropertyInfo> HiddenTableColumns;
        public string VersionKeyType;
        public bool ReturnPrimaryKey;
        public string JoinTableName;
        public IList<PropertyInfo> JoinTableColumns;
        public IList<string> JoinPrimaryKeyNames;
        public bool TenantRelated;
        public bool IsStoreDependent;
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
        public bool IsDeleted { get; set; }
        public bool IsSyncStateEnabled { get; set; }
        public bool IsSyncStateFiledExists { get; set; }
        public DatabaseType DatabaseType { get; set; }
        public IList<PropertyInfo> UpdateTableColumns { get; set; }
    }
}