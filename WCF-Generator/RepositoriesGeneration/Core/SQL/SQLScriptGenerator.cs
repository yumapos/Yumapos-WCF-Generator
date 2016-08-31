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
        #region Cache table

        public static string GenerateSelectAll(IEnumerable<string> parameters, string table)
        {
            return Select(parameters) + " " + From(table);
        }

        public static string GenerateSelectBy(IEnumerable<string> parameters, string table)
        {
            var enumerable = parameters as string[] ?? parameters.ToArray();
            return Select(enumerable) + " " + From(table) + " " + Where(enumerable, table);
        }

        public static string GenerateInsert(IEnumerable<string> parameters, string table)
        {
            return Insert(parameters, table);
        }

        public static string GenerateWhere(IEnumerable<string> parameters, string table)
        {
            return Where(parameters, table);
        }

        public static string GenerateUpdate(IEnumerable<string> parameters, string table)
        {
            return Update(table) + " " + Set(parameters, table);
        }

        public static string GenerateRemove(IEnumerable<string> parameters, string table)
        {
            return Delete(table) + " " + Where(parameters, table);
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

        private static string Select(IEnumerable<string> columns)
        {
            return "SELECT " + string.Join(",", columns.Select(e => "[" + e + "]"));
        }

        private static string Insert(IEnumerable<string> columns, string ownerTableName)
        {
            var values = columns as string[] ?? columns.ToArray();
            return "INSERT INTO " + ownerTableName + "(" + string.Join(",", values.Select(e => "[" + e + "]")) + ") VALUES(" + string.Join(",", values.Select(e => "@" + e)) + ")";
        }

        private static string From(string tableName)
        {
            return "FROM " + GenerateTableName(tableName);
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

        #endregion
    }
}