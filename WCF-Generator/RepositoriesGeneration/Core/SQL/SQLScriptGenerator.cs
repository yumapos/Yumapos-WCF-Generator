using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VersionedRepositoryGeneration.Generator.Heplers;
using VersionedRepositoryGeneration.Generator.Infrastructure;

namespace VersionedRepositoryGeneration.Generator.Core.SQL
{
    internal static class SqlScriptGenerator
    {
        #region Script generation

        public static string GenerateSelectAll(RepositoryInfo info)
        {
            var codeProps = info.Elements.Select(e => "[" + e + "]");

            return "SELECT " + string.Join(",", codeProps) + string.Format("FROM {0}{1}", GenerateTableName(info.ClassName), info.IsTenantRelated ? " {whereTenantId:}" : "");
        }

        public static string GenerateFieldsList(RepositoryInfo info)
        {
            var sb = new StringBuilder();

            int countMembers = info.Elements.Count;

            int i = 0;
            foreach (var codeProp in info.Elements)
            {
                if (!(info.IsIdentity && codeProp == info.Keys[0]) || info.IsJoined)
                {
                    sb.Append(codeProp);
                    i++;
                    if (i < countMembers) sb.Append(", ");
                    if (i == countMembers)
                    {
                        if (info.IsTenantRelated)
                        {
                            sb.Append("{columns}");
                        }
                    }
                }
                else
                {
                    countMembers = countMembers - 1;
                }
            }

            return sb.ToString();
        }

        public static string GenerateValuesList(RepositoryInfo info)
        {
            var sb = new StringBuilder();
            int countMembers = info.Elements.Count;

            int i = 0;

            foreach (var codeProp in info.Elements)
            {
                if (!(info.IsIdentity && codeProp == info.Keys[0]) || info.IsJoined)
                {
                    sb.Append(codeProp);
                    i++;

                    if (i < countMembers) sb.Append(", ");
                    if (i == countMembers)
                    {
                        if (info.IsTenantRelated)
                        {
                            sb.Append("{values}");
                        }
                    }
                }
                else
                {
                    countMembers = countMembers - 1;
                }
            }

            return sb.ToString();
        }

        public static string GenerateGetBy(RepositoryInfo info)
        {
            var codeProps = info.Elements.Select(e => "[" + e + "]");
            return "SELECT " + string.Join(",", codeProps) + string.Format("FROM {0} ", GenerateTableName(info.TableName));
        }

        public static string GenerateGetByJoined(RepositoryInfo info)
        {
            var codeProps = info.Elements.Concat(info.JoinedElements).Select(e => GenerateTableName(info.TableName) + ".[" + e + "]");
            return "SELECT " + string.Join(",", codeProps) + string.Format(@"FROM {0} INNER JOIN {1} ON {0}.[{2}] = {1}.[{3}] ", GenerateTableName(info.TableName), info.TableNameJoined, info.Keys[0], info.PrimaryKeyJoined);
        }

        public static string GenerateWhere(RepositoryInfo info, int j)
        {
            var sb = new StringBuilder();

            List<string> temp = GenerateCurrentKeys(info.Keys[j]);
            sb.Append(String.Format("\" WHERE {2}.[{0}] = @{1}", temp[0], StringHelper.FirstSymbolToLower(temp[0]), GenerateTableName(info.TableName)));

            for (int n = 1; n < temp.Count(); n++)
            {
                sb.Append(String.Format(" AND {2}.[{0}] = @{1}", temp[n], StringHelper.FirstSymbolToLower(temp[n]), GenerateTableName(info.TableName)));
            }

            if (info.IsTenantRelated)
            {
                var tenant = String.Format("andTenantId:{0}", GenerateTableName(info.TableName));
                sb.Append(String.Format("{0}", '{' + tenant + '}'));
            }

            sb.Append(" \";");

            if (info.FilterData != null || info.FilterDataJoined != null)
            {
                var filterData = info.FilterData != null ? info.FilterData : info.FilterDataJoined;
                var tableName = info.FilterData != null ? GenerateTableName(info.TableName) : GenerateTableName(info.TableNameJoined);

                sb.Append(String.Format("\r\n        private const string WhereQueryBy{0}{1}", info.PrimaryKeyNames[j], "And" + filterData.Name));
                temp.Add(filterData.Name);
                sb.Append(String.Format("= \" WHERE {2}.[{0}] = @{1}", temp[0], StringHelper.FirstSymbolToLower(temp[0]), tableName));

                for (int n = 1; n < temp.Count(); n++)
                {
                    sb.Append(String.Format(" AND {2}.[{0}] = @{1}", temp[n], StringHelper.FirstSymbolToLower(temp[n]), tableName));
                }

                if (info.IsTenantRelated)
                {
                    var tenant = String.Format("andTenantId:{0}", tableName);
                    sb.Append(String.Format("{0}", '{' + tenant + '}'));
                }

                sb.Append(" \";");
                if (!info.IsFilterDataGeneration)
                {
                    string relation;
                    if (!info.IsTenantRelated)
                    {
                        sb.Append("\r\n        private const string WhereWithFilterData = ");
                        relation = "WHERE";
                        if (info.IsTenantRelated)
                        {
                            relation = "AND";
                        }
                        sb.Append(" " + relation + " " + tableName + ".[" + filterData.Name + "] = " + StringHelper.FirstSymbolToLower(filterData.Name));
                        sb.Append("; \";");
                    }
                    sb.Append("\r\n        private const string AndWithFilterData = WHERE");
                    relation = "AND";
                    sb.Append(" " + relation + " " + tableName + ".[" + filterData.Name + "] = " + StringHelper.FirstSymbolToLower(filterData.Name));
                    sb.Append(" \";");
                }
                info.IsFilterDataGeneration = true;
            }

            return sb.ToString();
        }

        public static string GenerateUpdate(RepositoryInfo info, int j)
        {
            var sb = new StringBuilder();

            int countMembers = info.Elements.Count;
            sb.Append(String.Format("\r\n        private const string UpdateQueryBy{0} ", info.PrimaryKeyNames[j]));

            sb.Append(String.Format("= \"UPDATE {0} SET ", GenerateTableName(info.TableName)));
            List<string> temp = GenerateCurrentKeys(info.Keys[j]);

            int i = 0;
            foreach (var codeProp in info.Elements)
            {
                if (temp.Count == 1 && j == 0)
                {
                    if (codeProp != info.Keys[j])
                    {
                        sb.Append(String.Format("[{0}] = @{0}", codeProp));
                        i++;
                        if (i < countMembers) sb.Append(", ");
                        if (i == countMembers) sb.Append(" ");
                    }
                    else
                    {
                        countMembers = countMembers - 1;
                    }
                }
                else
                {
                    sb.Append(String.Format("[{0}] = @{0}", codeProp));
                    i++;
                    if (i < countMembers) sb.Append(", ");
                    if (i == countMembers) sb.Append(" ");
                }
            }

            sb.Append(String.Format("FROM {0} ", GenerateTableName(info.TableName)));

            sb.Append("\";");

            return sb.ToString();
        }

        public static string GenerateUpdateJoined(RepositoryInfo info)
        {
            var sb = new StringBuilder();

            int countMembers = info.JoinedElements.Count;

            sb.Append(String.Format("= \"UPDATE {0} SET ", GenerateTableName(info.TableNameJoined)));
            int i = 0;
            foreach (var codeProp in info.JoinedElements)
            {
                if (codeProp != info.PrimaryKeyJoined)
                {
                    sb.Append(String.Format("[{0}] = @{0}", codeProp));
                    i++;
                    if (i < countMembers) sb.Append(", ");
                    if (i == countMembers) sb.Append(" ");
                }
                else
                {
                    countMembers = countMembers - 1;
                }
            }

            sb.Append(String.Format("FROM {0} INNER JOIN {1} ON {0}.[{2}] = {1}.[{3}]  ", info.TableNameJoined, info.TableName,
                info.PrimaryKeyJoined, info.Keys[0]));

            sb.Append("\";");

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
            string declareVariable = string.Format(" \"DECLARE @TempValueDb TABLE ({0} {1});", info.PrimaryKeyJoined,
                type);
            string declareInsert = "\"INSERT INTO @TempValueDb ";
            string selectSql = string.Format("\"SELECT {0}  FROM {1} ", info.Keys[0],
                info.TableName);
            string firstDelete = string.Format("\"DELETE {0} FROM {0}" +
                                               " WHERE [{1}] IN (SELECT {2} FROM @TempValueDb)", info.TableName, info.Keys[0],
                info.PrimaryKeyJoined);
            string secondDelete = string.Format("\" DELETE {0} FROM {0} WHERE {1} IN (SELECT {1} FROM @TempValueDb)",
                info.TableNameJoined, info.PrimaryKeyJoined);
            string whiteSpace = "\"+\r\n                                                          ";
            sb.Append(String.Format("{0}{3}{1}{3}{2}", declareVariable, declareInsert, selectSql, whiteSpace));

            sb.Append("\"; \r\n");

            sb.Append(String.Format("{0}{2}{1}", firstDelete, secondDelete, whiteSpace));
            sb.Append("\"; \r\n");

            return sb.ToString();
        }

        public static string GenerateGetAllJoined(RepositoryInfo info)
        {
            var sb = new StringBuilder();

            sb.Append(String.Format("\r\n        private const string SelectAllQuery{0}Join{1} ", info.ClassName, info.JoinedClassName));
            sb.Append("= \"SELECT ");

            foreach (var codeProp in info.Elements)
            {
                sb.Append(String.Format("{1}.[{0}], ", codeProp, info.TableName));
            }

            int i = 0;
            int countMembers = info.JoinedElements.Count;
            foreach (var codeProp in info.JoinedElements)
            {
                sb.Append(String.Format("{1}.[{0}]", codeProp, GenerateTableName(info.TableNameJoined)));
                i++;
                if (i < countMembers) sb.Append(", ");
                if (i == countMembers) sb.Append(" ");
            }

            sb.Append(String.Format("FROM {0} INNER JOIN {1} ON {0}.[{2}] = {1}.[{3}] ", GenerateTableName(info.TableName), GenerateTableName(info.TableNameJoined), info.Keys[0],
                info.PrimaryKeyJoined));

            if (info.IsTenantRelated)
            {
                var tenant = string.Format("whereTenantId:{0}", GenerateTableName(info.TableName));
                sb.Append(String.Format("{0}", '{' + tenant + '}'));
            }

            sb.Append("\";");

            return sb.ToString();
        }

        public static string GenerateInsert(RepositoryInfo info)
        {
            var sb = new StringBuilder();

            int i = 0;
            int countMembers = info.Elements.Count;

            if (!info.IsJoined)
            {
                sb.Append(string.Format("INSERT INTO {0} (", info.TableName));
            }
            else
            {
                sb.Append(string.Format("\"INSERT INTO {0} (", info.TableName, info.JoinedClassName)); // TODO ���-�� ��� �� ���...
            }

            foreach (var codeProp in info.Elements)
            {
                if ((!(info.IsIdentity && codeProp == info.Keys[0]) || info.IsJoined || !info.IsNewKey)
                    && !(info.IsIdentity && codeProp == info.Keys[0] && info.JoinedClassName == "int"))
                {
                    sb.Append(String.Format("[{0}]", codeProp));
                    i++;
                    if (i < countMembers) sb.Append(", ");
                    if (i == countMembers)
                    {
                        if (info.IsTenantRelated)
                        {
                            sb.Append("{columns}");
                        }
                        sb.Append(") ");
                    }
                }
                else
                {
                    countMembers = countMembers - 1;
                }
            }

            if ((info.Keys.Count() != 0 && !info.IsJoined) || !info.IsNewKey)
            {
                List<string> temp = GenerateCurrentKeys(info.Keys[0]);
                sb.Append(String.Format("OUTPUT INSERTED.{0}", temp[0]));
            }

            sb.Append(" VALUES (");
            i = 0;

            foreach (var codeProp in info.Elements)
            {
                if ((!(info.IsIdentity && codeProp == info.Keys[0]) || !info.IsJoined || !info.IsNewKey)
                    && !(info.IsIdentity && codeProp == info.Keys[0] && info.JoinedClassName == "int"))
                {
                    if (!info.IsJoined && codeProp == info.Keys[0])
                    {
                        if (info.IsIdentityJoined)
                        {
                            sb.Append(String.Format("@TempPK{0}, ", info.PrimaryKeyJoined));
                            i++;
                        }
                        else
                        {
                            sb.Append(String.Format("@{0}, ", info.PrimaryKeyJoined));
                            i++;
                        }
                    }
                    else
                    {
                        sb.Append(String.Format("@{0}", codeProp));
                        i++;

                        if (i < countMembers) sb.Append(", ");
                        if (i == countMembers)
                        {
                            if (info.IsTenantRelated)
                            {
                                sb.Append("{values}");
                            }
                            sb.Append(");");
                        }
                    }
                }
            }

            if (info.IsJoined && info.IsIdentityJoined)
            {
                sb.Append(String.Format("+ \r\n                                               SELECT {0} FROM @TempPKTable", info.PrimaryKeyJoined));
            }

            sb.Append(" ;");

            return sb.ToString();
        }

        public static string GenerateInsertJoined(RepositoryInfo info)
        {
            var sb = new StringBuilder();

            int i = 0;
            int countMembers = info.JoinedElements.Count;
            var variableName = "InsertQueryJoin";

            if (info.IsNewKey)
            {
                variableName = "InsertQueryJoinGenerateId";
            }

            if (info.IsIdentityJoined)
            {
                string type = SystemToSqlTypeMapper.GetSqlType(info.JoinedClassName);
                string declaringTable = String.Format("DECLARE @TempPKTable TABLE ({0} {1}); \"+", info.PrimaryKeyJoined, type);
                string declaringVariable = String.Format("\r\n                                               \"DECLARE @TempPK{0} {1}; \"+", info.PrimaryKeyJoined, type);
                sb.Append(String.Format("\r\n        private const string {3} InsertQueryJoin = \"{1}{2} \r\n                                               \"INSERT INTO {0} (", info.TableNameJoined, declaringTable, declaringVariable, variableName));
            }
            else
            {
                sb.Append(String.Format("\r\n        private const string {1} InsertQueryJoin = \"INSERT INTO {0} (", info.TableNameJoined, variableName));
            }

            foreach (var codeProp in info.JoinedElements)
            {
                if (!(info.IsIdentityJoined && codeProp == info.PrimaryKeyJoined) || !info.IsNewKey)
                {
                    sb.Append("[{" + codeProp + "}]");
                    i++;
                    if (i < countMembers) sb.Append(", ");
                    if (i == countMembers)
                    {
                        if (info.IsTenantRelated)
                        {
                            sb.Append("{columns}");
                        }
                        sb.Append(") ");
                    }
                }
                else
                {
                    --countMembers;
                }
            }

            if (info.IsIdentityJoined || !info.IsNewKey)
            {
                sb.Append(String.Format("OUTPUT INSERTED.{0} INTO @TempPKTable", info.PrimaryKeyJoined));
            }

            sb.Append(" VALUES (");
            i = 0;

            foreach (var codeProp in info.JoinedElements)
            {
                if (!(info.IsIdentityJoined && codeProp == info.PrimaryKeyJoined) || !info.IsNewKey)
                {
                    sb.Append("@{" + codeProp + "}");
                    i++;

                    if (i < countMembers) sb.Append(", ");
                    if (i == countMembers)
                    {
                        if (info.IsTenantRelated)
                        {
                            sb.Append("{values}");
                        }
                        sb.Append(");\"+");
                    }
                }
            }

            if (info.IsIdentityJoined)
            {
                sb.Append(String.Format("\r\n                                          �     \"SELECT @TempPK{0} = {0} FROM @TempPKTable \"+", info.PrimaryKeyJoined));
            }
            else
            {
                sb.Append("\"+");
            }

            GenerateInsert(info);

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

        #region Private

        private static List<string> GenerateCurrentKeys(string Key)
        {
            List<string> filters = Key.Split(',').ToList();
            return filters;
        }


        private static readonly Dictionary<string, string> TableNameCash = new Dictionary<string, string>();

        private static string GenerateTableName(string tableName)
        {
            tableName = tableName.Trim();
            string cashtableName;

            TableNameCash.TryGetValue(tableName, out cashtableName);

            if (cashtableName != null)
            {
                return cashtableName;
            }

            var nametrue = "";

            if (tableName.Contains('.'))
            {
                var fullName = tableName.Split('.').ToList();
                foreach (var prefix in fullName)
                {
                    if (prefix[0] == '[')
                    {
                        nametrue = tableName;
                    }
                    else
                    {
                        nametrue = nametrue + "[" + prefix + "].";
                    }
                }
                nametrue = nametrue.Substring(0, nametrue.Length - 1);
            }
            else if (tableName[0] != '[')
            {
                nametrue = "[" + tableName + "]";
            }

            TableNameCash.Add(tableName, nametrue);

            return nametrue;
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

    }
}