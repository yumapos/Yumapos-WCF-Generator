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

        #region Old methods

        public static string GenerateUpdateJoined(RepositoryInfo info)
        {
            var sb = new StringBuilder();

            sb.Append(string.Format("UPDATE {0} SET ", GenerateTableName(info.TableNameJoined)));

            sb.Append(info.JoinedElements.Where(codeProp => codeProp != info.PrimaryKeyJoined).Select(codeProp => string.Format("[{0}] = @{0}", codeProp)));

            sb.Append(string.Format("FROM {0} INNER JOIN {1} ON {0}.[{2}] = {1}.[{3}]  ", info.TableNameJoined, info.TableName, info.PrimaryKeyJoined, info.Keys[0]));

            return sb.ToString();
        }

        public static string GenerateRemove(RepositoryInfo info)
        {
            return "DELETE FROM " + info.TableName;
        }

        public static string GenerateRemoveJoined(RepositoryInfo info)
        {
            var sb = new StringBuilder();

            string type = SystemToSqlTypeMapper.GetSqlType(info.JoinedClassName);
            string declareVariable = string.Format("DECLARE @TempValueDb TABLE ({0} {1});", info.PrimaryKeyJoined,
                type);
            string declareInsert = "INSERT INTO @TempValueDb ";
            string selectSql = string.Format("SELECT {0}  FROM {1} ", info.Keys[0],
                info.TableName);
            string firstDelete = string.Format("DELETE {0} FROM {0} WHERE [{1}] IN (SELECT {2} FROM @TempValueDb)", info.TableName, info.Keys[0],
                info.PrimaryKeyJoined);
            string secondDelete = string.Format("DELETE {0} FROM {0} WHERE {1} IN (SELECT {1} FROM @TempValueDb)",
                info.TableNameJoined, info.PrimaryKeyJoined);
            sb.Append(string.Format("{0} {1} {2}", declareVariable, declareInsert, selectSql));

            sb.Append(string.Format("{0} {1}", firstDelete, secondDelete));

            return sb.ToString();
        }

        public static string GenerateGetAllJoined(RepositoryInfo info)
        {
            var sb = new StringBuilder();

            sb.Append("SELECT " + string.Join(",", info.Elements.Select(e => info.TableName + ".[" + e + "]")));

            sb.Append("," + string.Join(",", info.JoinedElements.Select(e => info.TableNameJoined + ".[" + e + "]")));
            
            sb.Append(string.Format(" FROM {0} INNER JOIN {1} ON {0}.[{2}] = {1}.[{3}] ", info.TableName, info.TableNameJoined, info.Keys[0], info.PrimaryKeyJoined));

            if (info.IsTenantRelated)
            {
                sb.Append(string.Format("{{whereTenantId:{0}}}", info.TableName));
            }

            return sb.ToString();
        }

        public static string GenerateInsert(RepositoryInfo info)
        {
            var sb = new StringBuilder();

            sb.Append(string.Format("INSERT INTO {0} (", info.TableName));

            sb.Append(string.Join(",", info.Elements
                .Where(codeProp => (info.IsJoined || !info.IsNewKey))
                .Select(e => info.TableName + ".[" + e + "]")));

            //if ((info.Keys.Count() != 0 && !info.IsJoined) || !info.IsNewKey)
            //{
            //    var temp = GenerateCurrentKeys(info.Keys[0]);
            //    sb.Append(string.Format("OUTPUT INSERTED.{0}", temp[0]));
            //}

            sb.Append(" VALUES (");

            var i = 0;

            var elements = info.Elements.Where(codeProp => (!info.IsJoined || !info.IsNewKey));
            foreach (var codeProp in elements)
            {
               
                if (!info.IsJoined && codeProp == info.Keys[0])
                {
                    if (info.IsIdentityJoined)
                    {
                        sb.Append(string.Format("@TempPK{0}, ", info.PrimaryKeyJoined));
                        i++;
                    }
                    else
                    {
                        sb.Append(string.Format("@{0}, ", info.PrimaryKeyJoined));
                        i++;
                    }
                }
                else
                {
                    sb.Append(string.Format("@{0}", codeProp));
                    i++;

                    if (i < info.Elements.Count) sb.Append(", ");
                    if (i == info.Elements.Count)
                    {
                        if (info.IsTenantRelated)
                        {
                            sb.Append("{values}");
                        }
                        sb.Append(");");
                    }
                }
            }

            if (info.IsJoined && info.IsIdentityJoined)
            {
                sb.Append(string.Format(" SELECT {0} FROM @TempPKTable", info.PrimaryKeyJoined));
            }

            return sb.ToString();
        }

        public static string GenerateInsertJoined(RepositoryInfo info)
        {
            var sb = new StringBuilder();

            if (info.IsIdentityJoined)
            {
                string type = SystemToSqlTypeMapper.GetSqlType(info.JoinedClassName);
                sb.Append(string.Format("DECLARE @TempPKTable TABLE ({0} {1});", info.PrimaryKeyJoined, type));
                sb.Append(string.Format("DECLARE @TempPK{0} {1};", info.PrimaryKeyJoined, type));
            }

            sb.Append(string.Format(" INSERT INTO {0} (", info.TableNameJoined));

            sb.Append(string.Join(",", info.JoinedElements.Where(codeProp=> !(info.IsIdentityJoined && codeProp == info.PrimaryKeyJoined) || !info.IsNewKey)
                .Select(codeProp => "[{" + codeProp + "}]")));

            if (info.IsTenantRelated)
            {
                sb.Append("{columns}");
            }
            sb.Append(")");

            if (info.IsIdentityJoined || !info.IsNewKey)
            {
                sb.Append(string.Format("OUTPUT INSERTED.{0} INTO @TempPKTable", info.PrimaryKeyJoined));
            }

            sb.Append(" VALUES (");

            sb.Append(string.Join(",", info.JoinedElements.Where(codeProp => !(info.IsIdentityJoined && codeProp == info.PrimaryKeyJoined) || !info.IsNewKey)
               .Select(codeProp => "@{" + codeProp + "}")));

            if (info.IsIdentityJoined)
            {
                sb.Append(string.Format(" SELECT @TempPK{0} = {0} FROM @TempPKTable", info.PrimaryKeyJoined));
            }

            sb.Append(GenerateInsert(info));

            return sb.ToString();
        }

        public static List<string> GenerateNamePrimaryKeys(IEnumerable<string> keys)
        {
            List<string> nameMethods = new List<string>();
            foreach (string _keys in keys)
            {
                List<string> filters = _keys.Split(',').ToList();
                string temp = "";
                if (filters.Count() > 1)
                {
                    temp = filters[0];
                    for (int i = 1; i < filters.Count(); i++)
                    {
                        temp = temp + "And" + filters[i];
                        temp.Trim();
                    }
                }
                else
                {
                    temp = _keys;
                }
                nameMethods.Add(temp);
            }

            return nameMethods;
        }

        #endregion

        #region VERSION TABLE

        public static string GenerateInsertToVersionTable(RepositoryInfo info, string repositoryName)
        {
            var classProperties = string.Join(",", info.Elements.Select(x => "[" + x.ToString() + "]"));
            var classValues = string.Join(",", info.Elements.Select(x => "@" + x.ToString()));

            var property = info.VersionKeyJoined;
            var type = SystemToSqlTypeMapper.GetSqlType(info.JoinedClassName);

            if (info.IsJoined)
            {
                var joinClassProperties = string.Join(",", info.JoinedElements.Select(x => "[" + x.ToString() + "]"));
                var joinClassValues = string.Join(",", info.JoinedElements.Select(x => "@" + x.ToString()));

                return "DECLARE @TempPKTable TABLE (" + property + " " + type + ");\n" +
                   "DECLARE @TempPK" + property + " " + type + ";\n" +
                   "INSERT INTO " + info.TableNameJoined + "(" + joinClassProperties + ")\n" +
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

        private static string GenerateFieldsList(RepositoryInfo info)
        {
            var sb = new StringBuilder();

            sb.Append(string.Join(",", info.Elements.Select(e => "[" + e + "]")));

            return sb.ToString();
        }

        private static string GenerateValuesList(RepositoryInfo info)
        {
            var sb = new StringBuilder();

            sb.Append(string.Join(",", info.Elements.Select(e => "@" + e)));

            return sb.ToString();
        }


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