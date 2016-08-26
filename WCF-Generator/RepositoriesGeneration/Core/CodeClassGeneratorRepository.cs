using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionedRepositoryGeneration.Generator.Core.SQL;
using VersionedRepositoryGeneration.Generator.Heplers;
using VersionedRepositoryGeneration.Generator.Infrastructure;

namespace VersionedRepositoryGeneration.Generator.Core
{
    internal class CodeClassGeneratorRepository : BaseCodeClassGeneratorRepository
    {
        #region  Overrides of BaseCodeClassGeneratorRepository

        public override string GetUsings()
        {
            var compilation = CSharpCompilation.Create("compilation").AddSyntaxTrees(RepositoryInfo.RepositoryInterface.SyntaxTree);
            var model = compilation.GetSemanticModel(RepositoryInfo.RepositoryInterface.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(RepositoryInfo.RepositoryInterface);
            var fullName = symbol.ToString();
            var nameSpace = fullName.Replace("." + RepositoryInfo.RepositoryInterface.Identifier.Text, "");

            var sb = new StringBuilder();
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using " + nameSpace + ";");

            return sb.ToString();
        }
        public override string GetFields()
        {
            var sb = new StringBuilder();

            // Update list of key name to SQL format // TODO REFACTOR
            RepositoryInfo.PrimaryKeyNames = SqlScriptGenerator.GenerateNamePrimaryKeys(RepositoryInfo.Keys);

            sb.AppendLine("#region Fields");
            sb.AppendLine("");

            sb.AppendLine("internal const string Fields = " + SqlScriptGenerator.GenerateFieldsList(RepositoryInfo).SurroundWithQuotes());
            sb.AppendLine("internal const string Values = " + SqlScriptGenerator.GenerateValuesList(RepositoryInfo).SurroundWithQuotes());

            if (!RepositoryInfo.IsJoined)
            {
                var type = SyntaxAnalysisHelper.FindType(RepositoryInfo.DOClass, RepositoryInfo.Keys[0]);

                sb.AppendLine("private const string SelectAllQuery" + RepositoryInfo.ClassName + " = " + SqlScriptGenerator.GenerateSelectAll(RepositoryInfo).SurroundWithQuotes() + ";");
                sb.AppendLine("private const string InsertQuery = " + SqlScriptGenerator.GenerateInsert(RepositoryInfo).SurroundWithQuotes() + ";");

                if (RepositoryInfo.IsIdentity && type == "System.Guid")
                {
                    sb.AppendLine("private const string InsertQueryGenerateId = " + SqlScriptGenerator.GenerateInsert(RepositoryInfo).SurroundWithQuotes() + ";");
                }
                sb.AppendLine("private const string SelectQuery = " + SqlScriptGenerator.GenerateGetBy(RepositoryInfo).SurroundWithQuotes() + ";");
                sb.AppendLine("private const string DeleteQuery = " + SqlScriptGenerator.GenerateRemove(RepositoryInfo).SurroundWithQuotes() + ";");

                for (int i = 0; i < RepositoryInfo.Keys.Count(); i++)
                {
                    sb.AppendLine("private const string WhereQueryBy" + RepositoryInfo.PrimaryKeyNames[i] + " = " + SqlScriptGenerator.GenerateWhere(RepositoryInfo, i).SurroundWithQuotes() + ";");
                    sb.AppendLine("private const string UpdateQueryBy" + RepositoryInfo.PrimaryKeyNames[i] + " = " + SqlScriptGenerator.GenerateUpdate(RepositoryInfo, i).SurroundWithQuotes() + ";");
                }
            }
            else
            {
                var type = SyntaxAnalysisHelper.FindType(RepositoryInfo.JoinedClass, RepositoryInfo.PrimaryKeyJoined);

                sb.AppendLine("private const string SelectIntoTempTableJoin" + RepositoryInfo.JoinedClassName + " = " + SqlScriptGenerator.GenerateGetAllJoined(RepositoryInfo).SurroundWithQuotes() + ";");
                sb.AppendLine("private const string InsertQueryJoin = " + SqlScriptGenerator.GenerateInsertJoined(RepositoryInfo).SurroundWithQuotes() + ";");

                if (RepositoryInfo.IsIdentityJoined && type == "System.Guid")
                {
                    sb.AppendLine("private const string InsertQueryJoin = " + SqlScriptGenerator.GenerateInsertJoined(RepositoryInfo).SurroundWithQuotes() + ";");
                }

                sb.AppendLine("private const string SelectQueryJoin" + RepositoryInfo.JoinedClassName + " = " + SqlScriptGenerator.GenerateGetByJoined(RepositoryInfo).SurroundWithQuotes() + ";");
                sb.AppendLine("private const string DeleteQueryJoin" + RepositoryInfo.JoinedClassName + " = " + SqlScriptGenerator.GenerateRemoveJoined(RepositoryInfo).SurroundWithQuotes() + ";");
                sb.AppendLine("private const string UpdateQueryJoin" + RepositoryInfo.JoinedClassName + " = " + SqlScriptGenerator.GenerateUpdateJoined(RepositoryInfo).SurroundWithQuotes() + ";");

                for (int i = 0; i < RepositoryInfo.Keys.Count(); i++)
                {
                    sb.AppendLine("private const string WhereQueryBy" + RepositoryInfo.PrimaryKeyNames[i] + "And" + RepositoryInfo.FilterDataJoined.Name + " = " + SqlScriptGenerator.GenerateWhere(RepositoryInfo, i).SurroundWithQuotes() + ";");
                    sb.AppendLine("private const string UpdateQueryJoin" + RepositoryInfo.PrimaryKeyNames[i] + " = " + SqlScriptGenerator.GenerateUpdate(RepositoryInfo, i).SurroundWithQuotes() + ";");
                }
            }

            return sb.ToString();
        }
        
        public override string GetMethods()
        {
            var sb = new StringBuilder();

            // Update list of key name to SQL format // TODO REFACTOR
            RepositoryInfo.PrimaryKeyNames = SqlScriptGenerator.GenerateNamePrimaryKeys(RepositoryInfo.Keys);

            foreach (var methodInfo in RepositoryInfo.MethodImplementationInfo)
            {
                var commented = !methodInfo.RequiresImplementation;
                var codeText = "";

                switch (methodInfo.Method)
                {
                    case RepositoryMethod.GetAll:
                        codeText = GenerateGetAll(RepositoryInfo);
                        break;
                    case RepositoryMethod.Insert:
                        codeText = GenerateInsert();
                        break;
                    case RepositoryMethod.GetBy:
                        // codeText = GenerateGetBy(methodInfo);
                        break;
                    case RepositoryMethod.UpdateBy:
                        // codeText = GenerateUpdate();
                        break;
                    case RepositoryMethod.RemoveBy:
                        //  codeText = GenerateRemove() + GenerateRemoveById(j);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                codeText = commented ? codeText.SurroundWithComments() : codeText;
                sb.AppendLine(codeText);
            }

            return sb.ToString();
        }
        
        #endregion

        #region Generation of basic methods

        private static string GenerateGetAll(RepositoryInfo generationInfo)
        {
            var sb = new StringBuilder();

            string signature = "";
            string whereSql = "SelectAllQuery" + generationInfo.ClassName + ", null";
            string checkNull = "";
            string enterString = "";

            if (generationInfo.IsJoined)
            {
                whereSql = "SelectAllQuery" + generationInfo.ClassName + "Join" + generationInfo.JoinedClassName + ", null";
            }

            if (generationInfo.FilterData != null || generationInfo.FilterDataJoined != null)
            {
                enterString = "\r\n            ";
                var filterData = generationInfo.FilterData ?? generationInfo.FilterDataJoined;
                string queryName = generationInfo.FilterDataJoined != null ? generationInfo.ClassName + "Join" + generationInfo.JoinedClassName : generationInfo.ClassName;


                string parameters = "new {" + filterData.Name.FirstSymbolToLower() + "}";
                signature = filterData.Type + "? " + filterData.Name.FirstSymbolToLower() + " = " + filterData.DefaultValue;
                whereSql = "sql, parameters";
                checkNull = @"object parameters = null;
                var sql = SelectAllQuery@p2;
                if (@p0.HasValue)
                {
                    parameters = @p1;
                    sql = sql + @p3WithFilterData;
                }";


                checkNull = checkNull.Replace("@p1", parameters);
                checkNull = checkNull.Replace("@p0", filterData.Name.FirstSymbolToLower());
                checkNull = checkNull.Replace("@p2", queryName);
                checkNull = checkNull.Replace("@p3", generationInfo.IsTenantRelated ? "And" : "Where");
            }
            if (!generationInfo.IsJoined)
            {
                sb.Append("\t\t public async Task<IEnumerable<" + generationInfo.ClassFullName + "> GetAllAsync(" + signature + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + checkNull + enterString + "var result = await DataAccessService.GetAsync< " + generationInfo.ClassFullName + "> (" + whereSql + "\n");
                sb.Append("\t\t\t return result.ToList();\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public IEnumerable<" + generationInfo.ClassFullName + "> GetAll(" + signature + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + checkNull + enterString + "var result = DataAccessService.Get<" + generationInfo.ClassFullName + "> (" + whereSql + "\n");
                sb.Append("\t\t\t return result.ToList();\n");
                sb.Append("\t\t }\n");
            }
            else
            {
                sb.Append("\t\t public async Task<IEnumerable<" + generationInfo.ClassFullName + "> GetAllAsync(" + signature + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + checkNull + "var result = await DataAccessService.GetAsync< " + generationInfo.ClassFullName + "> (" + whereSql + "\n");
                sb.Append("\t\t\t return result.ToList();\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public IEnumerable<" + generationInfo.ClassFullName + "> GetAll(" + signature + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + checkNull + enterString + "var result = DataAccessService.Get<" + generationInfo.ClassFullName + "> (" + whereSql + "\n");
                sb.Append("\t\t\t return result.ToList();\n");
                sb.Append("\t\t }\n");
            }

            return sb.ToString();
        }

        private string GenerateGetBy(int j)
        {
            string result = "";
            string returnType = "";
            string firstOrDefault = "";

            List<string> temp = ParseKeys(RepositoryInfo.Keys[j]);
            FilterOption filter = null;

            if (RepositoryInfo.FilterData != null || RepositoryInfo.FilterDataJoined != null)
            {
                filter = RepositoryInfo.FilterData != null ? RepositoryInfo.FilterData : RepositoryInfo.FilterDataJoined;
            }

            if (j > 0)
            {
                foreach (MemberDeclarationSyntax member in RepositoryInfo.RepositoryInterface.Members)
                {
                    var cf = member as MethodDeclarationSyntax;
                    if (cf != null)
                    {
                        if ((cf.Identifier.ToString() == "GetBy" + RepositoryInfo.PrimaryKeyNames[j] || cf.Identifier.ToString() == "GetBy" + RepositoryInfo.PrimaryKeyNames[j] + "Async") && cf.ReturnType.ToString().Contains("IEnumerable"))
                        {
                            returnType = "IEnumerable<" + RepositoryInfo.ClassFullName + ">";
                            firstOrDefault = "";
                        }
                    }
                }
            }

            if (RepositoryInfo.Keys != null && RepositoryInfo.FilterKeys != null && RepositoryInfo.Keys.Count() - RepositoryInfo.FilterKeys.Count() == 0)
            {
                foreach (MemberDeclarationSyntax member in RepositoryInfo.RepositoryInterface.Members)
                {
                    var cf = member as MethodDeclarationSyntax;

                    if (cf != null)
                    {
                        for (int i = 0; i < RepositoryInfo.Keys.Count(); i++)
                        {
                            if ((cf.Identifier.ToString() == "GetBy" + RepositoryInfo.Keys[i] || cf.Identifier.ToString() == "GetBy" + RepositoryInfo.Keys[i] + "Async") && cf.ReturnType.ToString().Contains("IEnumerable"))
                            {
                                returnType = "IEnumerable<" + RepositoryInfo.ClassFullName + ">";
                                firstOrDefault = "";
                            }
                        }
                    }
                }
            }

            if (returnType == "")
            {
                firstOrDefault = ".FirstOrDefault()";
                returnType = RepositoryInfo.ClassFullName;
            }

            if (temp.Count() > 1)
            {
                foreach (string param in temp)
                {
                    string type = SyntaxAnalysisHelper.FindType(RepositoryInfo.DOClass, param);
                    result = result + type.TrimEnd('?') + " " + param.FirstSymbolToLower() + ",";
                }
                if (filter != null)
                {
                    result = result + filter.Type + "? " + filter.Name.FirstSymbolToLower() + " = " + filter.DefaultValue;
                }
                else
                {
                    result = result.Substring(0, result.Length - 1);
                }
            }
            else
            {
                string type = SyntaxAnalysisHelper.FindType(RepositoryInfo.DOClass, temp[0]);
                if (type.Count() != 0 && type[type.Length - 1] == '?')
                {
                    type = type.TrimEnd('?');
                }
                result = type + " " + temp[0].FirstSymbolToLower();
                if (filter != null)
                {
                    result = result + ", " + filter.Type + "? " + filter.Name.FirstSymbolToLower() + " = " + filter.DefaultValue;
                }
            }

            string parameters = ",new{";
            string outParameters = "";
            string checkNull = "";
            string filterParameters = "";
            for (int k = 0; k < temp.Count(); k++)
            {
                parameters += temp[k].FirstSymbolToLower();
                if (k != temp.Count() - 1)
                    parameters += ',';
            }
            parameters = parameters + "}";
            outParameters = parameters;
            string enterString = "";
            string sqlQueries = "SelectQuery+WhereQueryBy" + RepositoryInfo.PrimaryKeyNames[j];
            if (RepositoryInfo.IsJoined)
            {
                sqlQueries = "SelectQueryJoin" + RepositoryInfo.JoinedClassName + "+WhereQueryBy" + RepositoryInfo.PrimaryKeyNames[j];
            }

            if (filter != null)
            {
                filterParameters = parameters.Substring(1, parameters.Length - 2);
                filterParameters = filterParameters + "," + filter.Name.FirstSymbolToLower() + "}";
                outParameters = ",parameters";
                checkNull = @"object parameters = @p2;
                    var sql = SelectQuery@p4+WhereQueryBy@p3; 
                    if (@p0.HasValue)
                    {
                        parameters = @p1;
                        sql = sql + AndWithFilterData;
                    }";

                enterString = "\r\n            ";
                checkNull = checkNull.Replace("@p0", filter.Name.FirstSymbolToLower());
                checkNull = checkNull.Replace("@p1", filterParameters);
                parameters = parameters.Substring(1, parameters.Length - 1);
                checkNull = checkNull.Replace("@p2", parameters);
                checkNull = checkNull.Replace("@p3", RepositoryInfo.PrimaryKeyNames[j]);
                sqlQueries = "sql";
                checkNull = RepositoryInfo.IsJoined ? checkNull.Replace("@p4", "Join" + RepositoryInfo.JoinedClassName) : checkNull.Replace("@p4", "");
            }

            var sb = new StringBuilder();

            if (!RepositoryInfo.IsJoined)
            {
                sb.Append("\t\t public async Task<" + returnType + "> GetBy" + RepositoryInfo.PrimaryKeyNames[j] + " Async(" + result + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + checkNull + enterString + " var result = await DataAccessService.GetAsync<" + RepositoryInfo.ClassFullName + ">(" + sqlQueries + outParameters + " );\n");
                sb.Append("\t\t\t return result" + firstOrDefault + ";\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public " + returnType + "  GetBy" + RepositoryInfo.PrimaryKeyNames[j] + " (" + result + " )\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + checkNull + " " + enterString + " var result = DataAccessService.Get<" + RepositoryInfo.ClassFullName + " >(" + sqlQueries + outParameters + " );\n");
                sb.Append("\t\t\t return result" + firstOrDefault + ";\n");
                sb.Append("\t\t }\n");
            }
            else
            {
                sb.Append("\t\t public async Task<" + returnType + " > GetBy" + RepositoryInfo.PrimaryKeyNames[j] + " Async(" + result + " )\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + checkNull + " " + enterString + " var result = await DataAccessService.GetAsync < " + RepositoryInfo.ClassFullName + " > (" + sqlQueries + outParameters + ");\n");
                sb.Append("\t\t\t return result" + firstOrDefault + ";\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public " + returnType + "  GetBy" + RepositoryInfo.PrimaryKeyNames[j] + " (" + result + " )\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + checkNull + " " + enterString + " var result = DataAccessService.Get<" + RepositoryInfo.ClassFullName + " >(" + sqlQueries + outParameters + " );\n");
                sb.Append("\t\t\t return result" + firstOrDefault + ";\n");
                sb.Append("\t\t }\n");
            }

            return sb.ToString();
        }

        private string GenerateInsert()
        {
            string returnType = "";
            string returnTypeNotAsync = "";
            string conversion = "";
            string variable = "";
            string checkingKey = "";
            string queryName = "InsertQuery";

            if (RepositoryInfo.Keys.Count() != 0)
            {
                if (!RepositoryInfo.Keys[0].Contains(","))
                {
                    returnType = SyntaxAnalysisHelper.FindType(RepositoryInfo.DOClass, RepositoryInfo.Keys[0]);
                    if (returnType[returnType.Length - 1] == '?')
                    {
                        returnType = returnType.TrimEnd();
                    }
                    returnTypeNotAsync = returnType;
                    conversion = "return (" + returnType + ")result;";
                    variable = "var result = ";
                }
                else
                {
                    returnType = "";
                    variable = "";
                    returnTypeNotAsync = "void";
                }
            }

            foreach (MemberDeclarationSyntax member in RepositoryInfo.RepositoryInterface.Members)
            {
                if (member != null)
                {
                    var cf = member as MethodDeclarationSyntax;
                    if (cf != null)
                    {
                        if ((cf.Identifier.ToString() == "Insert" && cf.ReturnType.ToString() == "Task") || (cf.ReturnType.ToString() == "void" && cf.Identifier.ToString() == "Insert"))
                        {
                            returnType = "";
                            returnTypeNotAsync = "void";
                            conversion = "";
                            variable = "";
                        }
                    }
                }
            }

            if (RepositoryInfo.IsJoined && !RepositoryInfo.IsIdentityJoined)
            {
                queryName = "InsertQueryJoin";
            }

            if (returnType == "System.Guid" && (RepositoryInfo.IsIdentity || RepositoryInfo.IsIdentityJoined))
            {
                string joinPrefix = "";

                if (RepositoryInfo.IsJoined)
                {
                    joinPrefix = "Join";
                }

                checkingKey = @"var sql = InsertQuery@p3GenerateId;
                                if (@p0.@p1 != @p2)
                                {
                                    sql = InsertQuery@p3;
                                }";

                checkingKey = checkingKey.Replace("@p0", RepositoryInfo.ParameterName);
                checkingKey = checkingKey.Replace("@p1", !RepositoryInfo.IsJoined ? RepositoryInfo.Keys[0] : RepositoryInfo.PrimaryKeyJoined);
                checkingKey = checkingKey.Replace("@p2", "System.Guid.Empty");
                checkingKey = checkingKey.Replace("@p3", joinPrefix);
                checkingKey = checkingKey + "\r\n            ";
                queryName = "sql";
            }

            if (returnType != "")
            {
                returnType = '<' + returnType + '>';
            }

            var sb = new StringBuilder();

            if (!RepositoryInfo.IsJoined)
            {
                sb.Append("\t\t public async Task " + returnType + "InsertAsync(" + RepositoryInfo.ClassFullName + "" + RepositoryInfo.ParameterName + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + checkingKey + variable + "await DataAccessService.InsertObjectAsync(" + RepositoryInfo.ParameterName + "," + queryName + ");\n");
                sb.Append("\t\t\t " + conversion + "\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public " + returnTypeNotAsync + "Insert(" + RepositoryInfo.ClassFullName + "" + RepositoryInfo.ParameterName + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + checkingKey + variable + "DataAccessService.InsertObject(" + RepositoryInfo.ParameterName + "," + queryName + ");\n");
                sb.Append("\t\t\t " + conversion + "\n");
                sb.Append("\t\t }\n");
            }
            else
            {
                var joinedName = RepositoryInfo.JoinedClassName;

                sb.Append("\t\t public async Task" + returnType + " InsertAsync(" + RepositoryInfo.ClassFullName + " " + RepositoryInfo.ParameterName + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + checkingKey + variable + " await DataAccessService.InsertObjectAsync(" + RepositoryInfo.ParameterName + "," + queryName + ");\n");
                sb.Append("\t\t\t " + conversion + "\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public " + returnTypeNotAsync + " Insert(" + RepositoryInfo.ClassFullName + " " + RepositoryInfo.ParameterName + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + checkingKey + variable + " DataAccessService.InsertObject(" + RepositoryInfo.ParameterName + "," + queryName + ");\n");
                sb.Append("\t\t\t " + conversion + "\n");
                sb.Append("\t\t }\n");
            }

            return sb.ToString();
        }

        private string GenerateUpdate(RepositoryInfo generationInfo, int j)
        {
            var sb = new StringBuilder();

            if (!generationInfo.IsJoined)
            {
                sb.Append("\t\t public async Task UpdateBy" + generationInfo.PrimaryKeyNames[j] + "Async(" + RepositoryInfo.ClassFullName + " " + RepositoryInfo.ParameterName + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t await DataAccessService.PersistObjectAsync(" + RepositoryInfo.ParameterName + ",UpdateQueryBy" + generationInfo.PrimaryKeyNames[j] + "+WhereQueryBy" + generationInfo.PrimaryKeyNames[j] + ");\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public void UpdateBy" + generationInfo.PrimaryKeyNames[j] + "(" + RepositoryInfo.ClassFullName + " " + RepositoryInfo.ParameterName + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t DataAccessService.PersistObject(" + RepositoryInfo.ParameterName + ",UpdateQueryBy" + generationInfo.PrimaryKeyNames[j] + "+WhereQueryBy" + generationInfo.PrimaryKeyNames[j] + ");\n");
                sb.Append("\t\t }\n");
            }
            else
            {
                string joinedName = generationInfo.JoinedClassName;

                sb.Append("\t\t public async Task UpdateBy" + generationInfo.PrimaryKeyNames[j] + "Async(" + RepositoryInfo.ClassFullName + " " + RepositoryInfo.ParameterName + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t await DataAccessService.PersistObjectAsync(" + RepositoryInfo.ParameterName + ",UpdateQueryBy" + generationInfo.PrimaryKeyNames[j] + "+WhereQueryBy" + generationInfo.PrimaryKeyNames[j] + "UpdateQueryJoin" + joinedName + " + WhereQueryBy" + generationInfo.PrimaryKeyNames[j] + ");\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public void UpdateBy" + generationInfo.PrimaryKeyNames[j] + "(" + RepositoryInfo.ClassFullName + " " + RepositoryInfo.ParameterName + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t DataAccessService.PersistObject(" + RepositoryInfo.ParameterName + ",UpdateQueryBy" + generationInfo.PrimaryKeyNames[j] + "+WhereQueryBy" + generationInfo.PrimaryKeyNames[j] + "UpdateQueryJoin" + joinedName + " + WhereQueryBy" + generationInfo.PrimaryKeyNames[j] + ");\n");
                sb.Append("\t\t }\n");
            }

            return sb.ToString();
        }

        private string GenerateRemove(RepositoryInfo generationInfo, int j)
        {
            var sb = new StringBuilder();

            if (!generationInfo.IsJoined)
            {
                sb.Append("\t\t public async Task RemoveBy" + generationInfo.PrimaryKeyNames[j] + "Async(" + RepositoryInfo.ClassFullName + " " + RepositoryInfo.ParameterName + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t await DataAccessService.PersistObjectAsync(" + RepositoryInfo.ParameterName + ",DeleteQuery+WhereQueryBy" + generationInfo.PrimaryKeyNames[j] + ");\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public void RemoveBy" + generationInfo.PrimaryKeyNames[j] + "(" + RepositoryInfo.ClassFullName + " " + RepositoryInfo.ParameterName + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t DataAccessService.PersistObject(" + RepositoryInfo.ParameterName + ",DeleteQuery+WhereQueryBy" + generationInfo.PrimaryKeyNames[j] + ");\n");
                sb.Append("\t\t }\n");
            }
            else
            {
                var joinedName = generationInfo.JoinedClassName;

                sb.Append("\t\t public async Task RemoveBy" + generationInfo.PrimaryKeyNames[j] + "Async(" + RepositoryInfo.ClassFullName + " " + RepositoryInfo.ParameterName + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t await DataAccessService.PersistObjectAsync(" + RepositoryInfo.ParameterName + ",SelectIntoTempTableJoin" + joinedName + "+WhereQueryBy" + generationInfo.PrimaryKeyNames[j] + "+DeleteQueryJoin" + joinedName + ");\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public void RemoveBy" + generationInfo.PrimaryKeyNames[j] + "(" + RepositoryInfo.ClassFullName + " " + RepositoryInfo.ParameterName + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t DataAccessService.PersistObject(" + RepositoryInfo.ParameterName + ",SelectIntoTempTableJoin" + joinedName + "+WhereQueryBy" + generationInfo.PrimaryKeyNames[j] + "+DeleteQueryJoin" + joinedName + ");\n");
                sb.Append("\t\t }\n");
            }

            return sb.ToString();
        }

        private string GenerateRemoveById(int j)
        {
            var sb = new StringBuilder();
            string result = "";
            List<string> temp = ParseKeys(RepositoryInfo.Keys[j]);
            if (temp.Count() > 1)
            {
                foreach (string param in temp)
                {
                    string type = SyntaxAnalysisHelper.FindType(RepositoryInfo.DOClass, param);
                    if (type.Count() != 0 && type[type.Length - 1] == '?')
                    {
                        type = type.TrimEnd('?'); ;
                    }
                    result = result + type + " " + param.FirstSymbolToLower() + ",";
                }
                result = result.Substring(0, result.Length - 1);
            }
            else
            {
                string type = SyntaxAnalysisHelper.FindType(RepositoryInfo.DOClass, temp[0]);
                if (type.Count() != 0 && type[type.Length - 1] == '?')
                {
                    type = type.TrimEnd('?');
                }
                result = type + " " + temp[0].FirstSymbolToLower();
            }

            string parameters = "";

            for (int k = 0; k < temp.Count(); k++)
            {
                parameters += temp[k].FirstSymbolToLower();
                if (k != temp.Count() - 1)
                    parameters += ',';
            }

            if (!RepositoryInfo.IsJoined)
            {
                sb.Append("\t\t public async Task RemoveBy" + RepositoryInfo.PrimaryKeyNames[j] + "Async(" + result + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t await DataAccessService.PersistObjectAsync<" + RepositoryInfo.ClassFullName + ">(DeleteQuery+WhereQueryBy" + RepositoryInfo.PrimaryKeyNames[j] + " , new { " + parameters + " });\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public void RemoveBy" + RepositoryInfo.PrimaryKeyNames[j] + "(" + result + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t DataAccessService.PersistObject<" + RepositoryInfo.ClassFullName + ">(DeleteQuery+WhereQueryBy" + RepositoryInfo.PrimaryKeyNames[j] + " , new { " + parameters + " });\n");
                sb.Append("\t\t }\n");
            }
            else
            {
                var joinedName = RepositoryInfo.JoinedClassName;

                sb.Append("\t\t public async Task RemoveBy" + RepositoryInfo.PrimaryKeyNames[j] + "Async(" + result + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t await DataAccessService.PersistObjectAsync<" + RepositoryInfo.ClassFullName + ">(SelectIntoTempTableJoin" + joinedName + "+WhereQueryBy" + RepositoryInfo.PrimaryKeyNames[j] + "+DeleteQueryJoin" + joinedName + ", new { " + parameters + " });\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public void RemoveBy" + RepositoryInfo.PrimaryKeyNames[j] + "(" + result + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t DataAccessService.PersistObject<" + RepositoryInfo.ClassFullName + ">(SelectIntoTempTableJoin" + joinedName + "+WhereQueryBy" + RepositoryInfo.PrimaryKeyNames[j] + "+DeleteQueryJoin" + joinedName + ", new { " + parameters + " });\n");
                sb.Append("\t\t }\n");
            }

            return sb.ToString();
        }

        #endregion

        #region Private

        private static List<string> ParseKeys(string Key)
        {
            var filters = Key.Split(',').ToList();
            return filters;
        }
        
        #endregion
    }
}