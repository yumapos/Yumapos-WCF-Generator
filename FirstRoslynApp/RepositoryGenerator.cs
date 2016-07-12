using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;

namespace FirstRoslynApp
{
    public class RepositoryAndDo
    {
        public ClassDeclarationSyntax DOClass { get; set; }
        public ClassDeclarationSyntax RepositoryClass { get; set; }
        public InterfaceDeclarationSyntax RepositoryInterface { get; set; }
        public List<MethodDeclarationSyntax> MethodsInRepositoryClass { get; set; }
        public IEnumerable<MethodDeclarationSyntax> MethodsInRepositoryInterface { get; set; }
        public IEnumerable<MethodDeclarationSyntax> UnRealeasedMethod { get; set; }
        public GenerationClassElements GenerationElements { get; set; }
    }

    public class GenerationClassElements
    {
        public string ClassName { get; set; }
        public string ClassFullName { get; set; }
        public List<string> NamePrimaryKeys { get; set; }
        public string TableName { get; set; }
        public string Namespace { get; set; }
        public bool Constructor { get; set; }
        public List<string> Keys { get; set; }
        public List<string> FilterKeys { get; set; }
        public List<string> Elements { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsTenantRelated { get; set; }
        public bool IsJoined { get; set; }
        public ClassDeclarationSyntax JoinedClass { get; set; }
        public string JoinedClassName { get; set; }
        public string PrimaryKeyJoined { get; set; }
        public List<string> JoinedElements { get; set; }
        public string TableNameJoined { get; set; }
        public bool IsIdentityJoined { get; set; }
        public FilterOption FilterData { get; set; }
        public bool IsFilterDataGeneration { get; set; }
    }

    public class Signature
    {
        public string Parameter { get; set; }
        public string TypeParameter { get; set; }
    }

    public class MethodRealization
    {
        public string Method { get; set; }
        public bool Realization { get; set; }
    }

    public class FilterOption
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string DefaultValue { get; set; }
    }

    public class AttributeAndPropeperties
    {
        public string Name { get; set; }
        public Dictionary<string, string> Patameters { get; set; }

        public string GetParameterByKeyName(string key)
        {
            return Patameters.FirstOrDefault(x => x.Key.ToString().Trim() == key).Value;
        }
    }

    public class QueryTemplate
    {
        public string QueryType { get; set; }
        public string TableName { get; set; }
        public string[] Fields { get; set; }
        public string[] IgnoreFields { get; set; }
    }

    public class vsProjectType
    {
        public const string SolutionFolder = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";
        public const string VisualBasic = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}";
        public const string VisualCSharp = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
        public const string VisualCPlusPlus = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
        public const string VisualJSharp = "{E6FDF86B-F3D1-11D4-8576-0002A516ECE8}";
        public const string WebProject = "{E24C65DC-7377-472b-9ABA-BC803B73C61A}";
    }

    public class RepositoryGenerator
    {
        private static readonly MSBuildWorkspace _workspace = MSBuildWorkspace.Create();
        private static List<Tuple<string, SourceText, string[]>> tasks = new List<Tuple<string, SourceText, string[]>>();

        public string ProjectName { get; set; }
        public string SolutionPath { get; set; }

        Project Project { get; set; }
        Solution Solution { get; set; }

        public List<string> RepositoryClassProjects { get; set; }

        public RepositoryGenerator()
        {
            RepositoryClassProjects = new List<string>();
        }

        public string RepositoryMainPlace { get; set; }

        public string RepositorySuffix { get; set; }

        public string RepositoryAttribute { get; set; }

        public string RepositoryInterfaces { get; set; }

        public Dictionary<string, string> SystemTypesToSqlTypes = new Dictionary<string, string>
        {
            {"System.Guid","uniqueidentifier"},
            {"System.Int32","int"}
        };

        public string GetNameSpace(ClassDeclarationSyntax codeclass)
        {
            var nameSpace = codeclass.Identifier.ToString();

            return nameSpace.Remove(nameSpace.LastIndexOf(".", StringComparison.Ordinal), nameSpace.Length);
        }

        public string FindType(ClassDeclarationSyntax DOClass, string parameter)
        {
            PropertyDeclarationSyntax elementTrue = null;
            foreach (var elem in DOClass.Members)
            {
                var codeProperty = elem as PropertyDeclarationSyntax;
                if (codeProperty != null && codeProperty.Identifier.ToString() == parameter)
                    elementTrue = codeProperty;
            }

            if (elementTrue == null)
            {
                return "";
            }

            return elementTrue.Type.ToString();

        }

        public string FindTypeSql(ClassDeclarationSyntax DOClass, string parameter)
        {
            var variable = FindType(DOClass, parameter);
            return SystemTypesToSqlTypes.FirstOrDefault(x => x.Key == variable).Value;
        }

        public Signature GetNewSignature(GenerationClassElements elements)
        {
            var sign = new Signature();

            sign.TypeParameter = elements.ClassFullName;
            sign.Parameter = firstSymbolToLower(elements.ClassName);

            return sign;
        }

        public List<MethodRealization> AllPosibleMethods(List<string> PossibleName)
        {
            List<MethodRealization> methods = new List<MethodRealization>();
            methods.Add(new MethodRealization() { Method = "GetAll", Realization = false });
            methods.Add(new MethodRealization() { Method = "Insert", Realization = false });
            foreach (string name in PossibleName)
            {
                methods.Add(new MethodRealization() { Method = "GetBy" + name, Realization = false });
                methods.Add(new MethodRealization() { Method = "UpdateBy" + name, Realization = false });
                methods.Add(new MethodRealization() { Method = "RemoveBy" + name, Realization = false });
            }

            return methods;
        }

        public List<MethodRealization> AllPosibleMethodsWithJoin(List<string> PossibleName, string joinName)
        {
            List<MethodRealization> methods = new List<MethodRealization>
            {
                new MethodRealization() {Method = "GetAll", Realization = false},
                new MethodRealization() {Method = "Insert", Realization = false},
                new MethodRealization() {Method = "GetAll" + joinName, Realization = false},
                new MethodRealization() {Method = "Insert" + joinName, Realization = false}
            };
            foreach (string name in PossibleName)
            {
                methods.Add(new MethodRealization() { Method = "GetBy" + name, Realization = false });
                methods.Add(new MethodRealization() { Method = "UpdateBy" + name, Realization = false });
                methods.Add(new MethodRealization() { Method = "RemoveBy" + name, Realization = false });
                methods.Add(new MethodRealization() { Method = "GetBy" + name + joinName, Realization = false });
                methods.Add(new MethodRealization() { Method = "UpdateBy" + name + joinName, Realization = false });
                methods.Add(new MethodRealization() { Method = "RemoveBy" + name + joinName, Realization = false });
            }

            return methods;
        }

        public void FindKey(RepositoryAndDo similarClass)
        {
            similarClass.GenerationElements = new GenerationClassElements
            {
                Keys = new List<string>(),
                FilterKeys = new List<string>(),
                Elements = new List<string>()
            };

            string keyString = "";
            AttributeListSyntax attributes = null;
            foreach (var element in similarClass.DOClass.Members.ToList())
            {
                var cp = element as PropertyDeclarationSyntax;
                if (element != null && (IsExistGetterInCodeProperty(cp) || IsExistSetterInCodeProperty(cp)))
                {
                    similarClass.GenerationElements.Elements.Add(cp.Identifier.ToString());
                    var value = element.ToString();
                    if (cp != null && cp.AttributeLists.Count > 0)
                    {
                        attributes = cp.AttributeLists.First();

                        foreach (var attr in attributes.Attributes)
                        {
                            if (attr.Name.ToString() == "Key")
                            {
                                keyString = keyString + cp.Identifier + ',';
                            }
                            if (attr.Name.ToString() == "DbIgnore")
                            {
                                similarClass.GenerationElements.Elements.RemoveAt(similarClass.GenerationElements.Elements.Count - 1);
                            }
                            if (attr.Name.ToString() == "DataFilter")
                            {
                                var datafilterAttr = GetAttributesAndPropepertiesCollection(element).FirstOrDefault(x => x.Name == "DataFilter");
                                var defaultValue = datafilterAttr.GetParameterByKeyName("DefaultValue").ToString();
                                similarClass.GenerationElements.FilterData = new FilterOption
                                {
                                    Name = cp.Identifier.ToString(),
                                    Type = cp.Type.ToString(),
                                    DefaultValue = defaultValue
                                };
                                similarClass.GenerationElements.IsFilterDataGeneration = false;
                            }
                        }
                    }
                }
            }

            if (!(keyString.Length <= 0))
            {
                similarClass.GenerationElements.Keys.Add(keyString.Substring(0, keyString.Length - 1));
            }

            var dataAccessAttr = GetAttributesAndPropepertiesCollection(similarClass.DOClass).FirstOrDefault(x => x.Name == RepositoryAttribute);


            if (dataAccessAttr != null)
            {
                if (dataAccessAttr.GetParameterByKeyName("TableName") != null)
                {
                    similarClass.GenerationElements.TableName = dataAccessAttr.GetParameterByKeyName("TableName").ToString().Trim();
                }
                else
                {
                    similarClass.GenerationElements.TableName = similarClass.DOClass.Identifier.ToString() + 's';
                }

                similarClass.GenerationElements.TableName = GenerateTableName(similarClass.GenerationElements.TableName);

                var identity = dataAccessAttr.GetParameterByKeyName("Identity");
                if (identity != string.Empty)
                {
                    similarClass.GenerationElements.IsIdentity = Convert.ToBoolean(identity);
                }
                else
                {
                    similarClass.GenerationElements.IsIdentity = false;
                }
                if (dataAccessAttr.GetParameterByKeyName("FilterKey1") != null)
                {
                    similarClass.GenerationElements.Keys.Add(dataAccessAttr.GetParameterByKeyName("FilterKey1").ToString());
                    similarClass.GenerationElements.FilterKeys.Add(dataAccessAttr.GetParameterByKeyName("FilterKey1").ToString());
                }
                if (dataAccessAttr.GetParameterByKeyName("FilterKey2") != null)
                {
                    similarClass.GenerationElements.Keys.Add(dataAccessAttr.GetParameterByKeyName("FilterKey2").ToString());
                    similarClass.GenerationElements.FilterKeys.Add(dataAccessAttr.GetParameterByKeyName("FilterKey2").ToString());
                }
                if (dataAccessAttr.GetParameterByKeyName("FilterKey3") != null)
                {
                    similarClass.GenerationElements.Keys.Add(dataAccessAttr.GetParameterByKeyName("FilterKey3").ToString());
                    similarClass.GenerationElements.FilterKeys.Add(dataAccessAttr.GetParameterByKeyName("FilterKey3").ToString());
                }
            }

        }

        public string firstSymbolToLower(string parms)
        {
            char temp = ' ';
            temp = parms[0];
            temp = char.ToLower(temp);
            parms = parms.Substring(1, parms.Length - 1);
            parms = parms.Insert(0, Convert.ToString(temp));
            return parms;
        }

        public string DeleteLastSymbol(string key)
        {
            return key = key.Substring(0, key.Length - 1);
        }

        public List<string> GenerateNamePrimaryKeys(List<string> Keys)
        {
            List<string> nameMethods = new List<string>();
            foreach (string _keys in Keys)
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

        public List<string> GenerateCurrentKeys(string Key)
        {
            List<string> filters = Key.Split(',').ToList();
            return filters;
        }

        public string GenerateTableName(string tableName)
        {
            tableName = tableName.Trim();
            string nametrue = "";
            if (tableName.Contains('.'))
            {
                List<string> fullName = tableName.Split('.').ToList();
                foreach (var prefix in fullName)
                {
                    if (prefix[0] == '[')
                        return tableName;
                    else
                    {
                        nametrue = nametrue + "[" + prefix + "].";
                    }
                }
                nametrue = nametrue.Substring(0, nametrue.Length - 1);
            }
            else
            {
                if (tableName[0] != '[')
                {
                    nametrue = "[" + tableName + "]";
                }
                else
                {
                    return tableName;
                }
            }
            return nametrue;
        }

        public List<RepositoryAndDo> FindRepository()
        {
            var listOfSimilarClasses = new List<RepositoryAndDo>();

            foreach (var repositoryClasses in RepositoryClassProjects)
            {
                var findClasses = GeneratorHelper.GetAllClasses(repositoryClasses, false, RepositoryAttribute).Result;

                foreach (var doClass in findClasses)
                {
                    var repositoryAndDo = new RepositoryAndDo {DOClass = doClass};

                    var repositoryClass = GeneratorHelper.FindClassWithBasedClass(RepositoryMainPlace, doClass.Identifier + RepositorySuffix).Result;

                    if (repositoryClass != null)
                    {
                        var repositoryInterfaces = GeneratorHelper.GetImplementedInterfaces(RepositoryInterfaces, repositoryClass).Result.ToList();
                        repositoryAndDo.RepositoryClass = repositoryClass;
                        repositoryAndDo.MethodsInRepositoryClass = GeneratorHelper.GetMethodsFromMembers(repositoryClass.Members.ToList());

                        var compilation = CSharpCompilation.Create("compilation").AddSyntaxTrees(repositoryClass.SyntaxTree);

                        var model = compilation.GetSemanticModel(repositoryClass.SyntaxTree);
                        var symbol = model.GetDeclaredSymbol(repositoryClass);

                        var fullName = symbol.ToString();
                        var nameSpace = fullName.Replace("." + repositoryClass.Identifier.ToString(),"");

                        foreach (InterfaceDeclarationSyntax repInterface in repositoryInterfaces)
                        {
                            var infName = repInterface?.Identifier.ToString();

                            if (infName?.IndexOf("I", StringComparison.Ordinal) == 0 && infName.Remove(0, 1) == doClass.Identifier + RepositorySuffix)
                            {
                                repositoryAndDo.RepositoryInterface = (InterfaceDeclarationSyntax)repInterface;
                                repositoryAndDo.MethodsInRepositoryClass = GeneratorHelper.GetMethodsFromMembers(repositoryClass.Members.ToList());
                                repositoryAndDo.MethodsInRepositoryInterface = GeneratorHelper.GetMethodsFromMembers(repInterface.Members.ToList());
                                FindKey(repositoryAndDo);
                                repositoryAndDo.GenerationElements.Namespace = nameSpace;
                                listOfSimilarClasses.Add(repositoryAndDo);
                                break;
                            }
                        }
                    }
                    else
                    {
                        var repositoryInterface = GeneratorHelper.GetImplementedInterfaces(RepositoryInterfaces, 'I' + RepositorySuffix + '<' + doClass.Identifier.ToString() + '>').Result;

                        foreach (InterfaceDeclarationSyntax repInterface in repositoryInterface)
                        {
                            var infName = repInterface?.Identifier.ToString();

                            if (infName?.IndexOf("I", StringComparison.Ordinal) == 0 && infName.Remove(0, 1) == doClass.Identifier.ToString() + RepositorySuffix)
                            {
                                repositoryAndDo.RepositoryInterface = repInterface;
                                repositoryAndDo.MethodsInRepositoryInterface = GeneratorHelper.GetMethodsFromMembers(repInterface.Members.ToList());
                                FindKey(repositoryAndDo);
                                repositoryAndDo.GenerationElements.Namespace = RepositoryMainPlace;
                                listOfSimilarClasses.Add(repositoryAndDo);
                                break;
                            }
                        }
                    }
                }
            }

            return listOfSimilarClasses;
        }

        public List<RepositoryAndDo> FindConstructor(List<RepositoryAndDo> listOfSimilarClasses)
        {
            foreach (var constr in listOfSimilarClasses)
            {
                if (constr.RepositoryClass != null)
                {
                    foreach (MemberDeclarationSyntax elem in constr.RepositoryClass.Members)
                        if (constr.RepositoryClass.Identifier.ToString() == elem.ToString())
                            constr.GenerationElements.Constructor = true;
                }
                else
                {
                    constr.GenerationElements.Constructor = false;
                }
            }


            return listOfSimilarClasses;
        }

        public bool FindTenant(ClassDeclarationSyntax DOClass)
        {
            var interfaces = GeneratorHelper.GetImplementedInterfaces(RepositoryMainPlace, DOClass).Result;

            return interfaces.Any(inter => inter.Identifier.ToString().Trim() == "ITenantRelated");
        }

        public void FindUnreleasedMethod(RepositoryAndDo similarClass)
        {

            var UnreleasedM = new List<MethodDeclarationSyntax>();

            if (similarClass.RepositoryClass != null)
            {
                if (similarClass.MethodsInRepositoryInterface != null)
                {

                    foreach (var IMethod in similarClass.MethodsInRepositoryInterface)
                    {
                        bool exist = false;
                        foreach (var CMethod in similarClass.MethodsInRepositoryClass)
                        {
                            if (IMethod.Identifier == CMethod.Identifier)
                                exist = true;
                        }
                        if (!exist)
                        {
                            UnreleasedM.Add(IMethod);
                        }
                    }
                }
            }
            else
            {
                UnreleasedM = similarClass.MethodsInRepositoryInterface.ToList();
            }

            similarClass.UnRealeasedMethod = UnreleasedM;
        }

        public List<string> FindNamespace(ref List<RepositoryAndDo> listOfSimilarClasses)
        {
            var namespaces = new List<string>();

            foreach (var similarClass in listOfSimilarClasses)
            {
                if (namespaces.Count == 0)
                    namespaces.Add(similarClass.GenerationElements.Namespace);

                if (similarClass.RepositoryClass != null)
                {
                    foreach (MemberDeclarationSyntax elem in similarClass.RepositoryClass.Members)
                        if (similarClass.RepositoryClass.Identifier.ToString() == elem.ToString())
                            similarClass.GenerationElements.Constructor = true;
                }
                else
                {
                    similarClass.GenerationElements.Constructor = false;
                }
            }
            return namespaces;
        }

        public bool IsExistSetterInCodeProperty(PropertyDeclarationSyntax codeproperty)
        {
            try
            {
                if (codeproperty.AccessorList.Accessors.FirstOrDefault( x => x.Kind() == SyntaxKind.SetAccessorDeclaration) != null)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        public bool IsExistGetterInCodeProperty(PropertyDeclarationSyntax codeproperty)
        {
            try
            {
                if (codeproperty.AccessorList.Accessors.FirstOrDefault(x => x.Kind() == SyntaxKind.GetAccessorDeclaration) != null)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        public List<AttributeAndPropeperties> GetAttributesAndPropepertiesCollection(MemberDeclarationSyntax element)
        {
            SyntaxList<AttributeListSyntax> attributes = new SyntaxList<AttributeListSyntax>();

            var codeClass = element as ClassDeclarationSyntax;
            if (codeClass != null)
                attributes = codeClass.AttributeLists;

            var codeProperty = element as PropertyDeclarationSyntax;
            if (codeProperty != null)
                attributes = codeProperty.AttributeLists;

            var attributeCollection = new List<AttributeAndPropeperties>();
            var listOfStringProperties = new List<string>();

            //            Regex attributesRegex = new Regex(@"\[\s*(?<Name>\w*)\s*(\((?<arguments>.*)\))?\s*\]", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //            MatchCollection matchesAttrs = attributesRegex.Matches(attributeString);

            foreach (var ca in attributes)
            {
                foreach (var attr in ca.Attributes)
                {
                    var properties = attr.ArgumentList?.ToString() ?? "";

                    var dictionaryOfAttributes = new Dictionary<string, string>();
                    var countProperties = 0;
                    listOfStringProperties.Clear();

                    Regex attributesRegex = new Regex(@"(@""(?:""""|[^""])*"")|(""(?:\\""|\\r|\\n|\\t|\\\\|[^""\\])*"")",
                        RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    MatchCollection matchesProperties = attributesRegex.Matches(properties);

                    foreach (var property in matchesProperties)
                    {
                        properties = properties.Replace(property.ToString(), "%%string" + countProperties + "%%");
                        listOfStringProperties.Add(property.ToString());
                        countProperties++;
                    }

                    countProperties = 0;
                    foreach (string prop in properties.Split(',').ToList())
                    {
                        var property = prop.Replace("(", "").Replace(")", "");
                        if (property.Contains("%%string"))
                        {
                            if (property.Split(':', '=').Count() == 2)
                            {
                                if (property.Split(':', '=')[1].Contains("%%string"))
                                    dictionaryOfAttributes.Add(property.Split(':', '=')[0],
                                        listOfStringProperties[
                                            Convert.ToInt32(property.Split(':', '=')[1].Replace("%%string", "")
                                                .Replace("%", ""))].Replace("\"", ""));
                            }
                            else
                                dictionaryOfAttributes.Add(countProperties.ToString(),
                                    listOfStringProperties[
                                        Convert.ToInt32(property.Replace("%%string", "").Replace("%", ""))].Replace("\"", ""));
                        }
                        else
                        {
                            if (property.Split(':', '=').Count() == 2)
                                dictionaryOfAttributes.Add(property.Split(':', '=')[0], property.Split(':', '=')[1]);
                            else
                                dictionaryOfAttributes.Add(countProperties.ToString(), property);
                        }

                        countProperties++;
                    }

                    attributeCollection.Add(new AttributeAndPropeperties
                    {
                        Name = attr.Name.ToString(),
                        Patameters = dictionaryOfAttributes
                    });

                }
            }

            return attributeCollection;
        }

        public string SQLGenerateGetBy(GenerationClassElements generationElements)
        {
            var sb = new StringBuilder();
            int countMembers = generationElements.Elements.Count;
            sb.Append("\r\n        private const string SelectQuery ");

            sb.Append("= \"SELECT ");

            int i = 0;
            foreach (var codeProp in generationElements.Elements)
            {
                sb.Append("[" + codeProp + "]");
                i++;

                if (i < countMembers) sb.Append(", ");
                if (i == countMembers) sb.Append(" ");
            }

            sb.Append(String.Format("FROM {0} ", generationElements.TableName));
            sb.Append("\";");

            return sb.ToString();
        }

        public string SQLGenerateGetByJoined(GenerationClassElements generationElements)
        {
            var sb = new StringBuilder();

            int countMembers = generationElements.JoinedElements.Count();
            sb.Append("\r\n        private const string SelectQueryJoin" + generationElements.JoinedClassName);

            sb.Append("= \"SELECT ");


            foreach (var codeProp in generationElements.Elements)
            {
                sb.Append(String.Format("{0}.[{1}], ", generationElements.TableName, codeProp));
            }

            int i = 0;
            foreach (var codeProp in generationElements.JoinedElements)
            {
                sb.Append(String.Format("{0}.[{1}]", generationElements.TableNameJoined, codeProp));
                i++;

                if (i < countMembers) sb.Append(", ");
                if (i == countMembers) sb.Append(" ");
            }

            sb.Append(String.Format("FROM {0} INNER JOIN {1} ON {0}.[{2}] = {1}.[{3}] ", generationElements.TableName, generationElements.TableNameJoined, generationElements.Keys[0], generationElements.PrimaryKeyJoined));
            sb.Append(String.Format("\";"));

            return sb.ToString();
        }

        public string SQLGenerateWhere(GenerationClassElements generationElements, int j)
        {
            var sb = new StringBuilder();

            sb.Append(String.Format("\r\n        private const string WhereQueryBy{0} ", generationElements.NamePrimaryKeys[j]));

            List<string> temp = GenerateCurrentKeys(generationElements.Keys[j]);
            sb.Append(String.Format("= \" WHERE {2}.[{0}] = @{1}", temp[0], firstSymbolToLower(temp[0]), generationElements.TableName));

            for (int n = 1; n < temp.Count(); n++)
            {
                sb.Append(String.Format(" AND {2}.[{0}] = @{1}", temp[n], firstSymbolToLower(temp[n]), generationElements.TableName));
            }

            if (generationElements.IsTenantRelated)
            {
                var tenant = String.Format("andTenantId:{0}", generationElements.TableName);
                sb.Append(String.Format("{0}", '{' + tenant + '}'));
            }

            sb.Append("; \";");

            if (generationElements.FilterData != null)
            {
                var filterData = generationElements.FilterData;
                sb.Append(String.Format("\r\n        private const string WhereQueryBy{0}{1}", generationElements.NamePrimaryKeys[j], "And" + filterData.Name));
                temp.Add(filterData.Name);
                sb.Append(String.Format("= \" WHERE {2}.[{0}] = @{1}", temp[0], firstSymbolToLower(temp[0]), generationElements.TableName));

                for (int n = 1; n < temp.Count(); n++)
                {
                    sb.Append(String.Format(" AND {2}.[{0}] = @{1}", temp[n], firstSymbolToLower(temp[n]), generationElements.TableName));
                }

                if (generationElements.IsTenantRelated)
                {
                    var tenant = String.Format("andTenantId:{0}", generationElements.TableName);
                    sb.Append(String.Format("{0}", '{' + tenant + '}'));
                }

                sb.Append("; \";");
                if (!generationElements.IsFilterDataGeneration)
                {
                    sb.Append("\r\n        private const string WhereWithFilterData = \"");
                    string relation = "WHERE";
                    if (generationElements.IsTenantRelated)
                    {
                        relation = "AND";
                    }
                    sb.Append(String.Format(" {3} {2}.[{0}] = @{1}", filterData.Name, firstSymbolToLower(filterData.Name), generationElements.TableName, relation));
                    sb.Append("; \";");
                }
                generationElements.IsFilterDataGeneration = true;
            }

            return sb.ToString();
        }

        public string SQLGenerateUpdate(GenerationClassElements generationElements, int j)
        {
            var sb = new StringBuilder();

            int countMembers = generationElements.Elements.Count;
            sb.Append(String.Format("\r\n        private const string UpdateQueryBy{0} ", generationElements.NamePrimaryKeys[j]));

            sb.Append(String.Format("= \"UPDATE {0} SET ", generationElements.TableName));
            List<string> temp = GenerateCurrentKeys(generationElements.Keys[j]);

            int i = 0;
            foreach (var codeProp in generationElements.Elements)
            {
                if (temp.Count == 1 && j == 0)
                {
                    if (codeProp != generationElements.Keys[j])
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

            sb.Append(String.Format("FROM {0} ", generationElements.TableName));

            sb.Append("\";");

            return sb.ToString();
        }

        public string SQLGenerateUpdateJoined(GenerationClassElements generationElements)
        {
            var sb = new StringBuilder();

            int countMembers = generationElements.JoinedElements.Count;
            sb.Append(String.Format("\r\n        private const string UpdateQueryJoin{0} ", generationElements.JoinedClassName));

            sb.Append(String.Format("= \"UPDATE {0} SET ", generationElements.TableNameJoined));
            int i = 0;
            foreach (var codeProp in generationElements.JoinedElements)
            {
                if (codeProp != generationElements.PrimaryKeyJoined)
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

            sb.Append(String.Format("FROM {0} INNER JOIN {1} ON {0}.[{2}] = {1}.[{3}]  ", generationElements.TableNameJoined, generationElements.TableName,
                    generationElements.PrimaryKeyJoined, generationElements.Keys[0]));

            sb.Append("\";");

            return sb.ToString();
        }

        public string SQLGenerateRemove(GenerationClassElements generationElements)
        {
            var sb = new StringBuilder();

            sb.Append("\r\n        private const string DeleteQuery ");

            sb.Append(String.Format("= \"DELETE FROM {0} ", generationElements.TableName));

            sb.Append("\"; \r\n");

            return sb.ToString();
        }

        public string SQLGenerateRemoveJoined(GenerationClassElements generationElements)
        {
            var sb = new StringBuilder();

            sb.Append(String.Format("\r\n        private const string SelectIntoTempTableJoin{0} = ", generationElements.JoinedClassName));

            string type = FindTypeSql(generationElements.JoinedClass, generationElements.PrimaryKeyJoined);
            string declareVariable = string.Format(" \"DECLARE @TempValueDb TABLE ({0} {1});", generationElements.PrimaryKeyJoined,
            type);
            string declareInsert = "\"INSERT INTO @TempValueDb ";
            string selectSql = string.Format("\"SELECT {0}  FROM {1} ", generationElements.Keys[0],
            generationElements.TableName);
            string firstDelete = string.Format("\"DELETE {0} FROM {0}" +
             " WHERE [{1}] IN (SELECT {2} FROM @TempValueDb)", generationElements.TableName, generationElements.Keys[0],
             generationElements.PrimaryKeyJoined);
            string secondDelete = string.Format("\" DELETE {0} FROM {0} WHERE {1} IN (SELECT {1} FROM @TempValueDb)",
            generationElements.TableNameJoined, generationElements.PrimaryKeyJoined);
            string whiteSpace = "\"+\r\n                                                          ";
            sb.Append(String.Format("{0}{3}{1}{3}{2}", declareVariable, declareInsert, selectSql, whiteSpace));

            sb.Append("\"; \r\n");

            sb.Append(String.Format("\r\n        private const string DeleteQueryJoin{0} = ", generationElements.JoinedClassName));

            sb.Append(String.Format("{0}{2}{1}", firstDelete, secondDelete, whiteSpace));
            sb.Append("\"; \r\n");

            return sb.ToString();
        }

        public string SQLGenerateGetAll(GenerationClassElements generationElements)
        {
            var sb = new StringBuilder();

            int countMembers = generationElements.Elements.Count();
            sb.Append(String.Format("\r\n        private const string SelectAllQuery{0} ", generationElements.ClassName));
            sb.Append("= \"SELECT ");

            int i = 0;
            foreach (var codeProp in generationElements.Elements)
            {
                sb.Append(String.Format("[{0}]", codeProp));
                i++;
                if (i < countMembers) sb.Append(", ");
                if (i == countMembers) sb.Append(" ");
            }
            if (!generationElements.IsTenantRelated)
            {
                sb.Append(String.Format("FROM {0}  ", generationElements.TableName));
            }
            else
            {
                sb.Append(String.Format("FROM {0} {1}", generationElements.TableName, "{whereTenantId:}"));
            }

            sb.Append("\";");

            return sb.ToString();
        }

        public string SQLGenerateGetAllJoined(GenerationClassElements generationElements)
        {
            var sb = new StringBuilder();

            sb.Append(String.Format("\r\n        private const string SelectAllQuery{0}Join{1} ", generationElements.ClassName, generationElements.JoinedClassName));
            sb.Append("= \"SELECT ");

            foreach (var codeProp in generationElements.Elements)
            {
                sb.Append(String.Format("{1}.[{0}], ", codeProp, generationElements.TableName));
            }

            int i = 0;
            int countMembers = generationElements.JoinedElements.Count;
            foreach (var codeProp in generationElements.JoinedElements)
            {
                sb.Append(String.Format("{1}.[{0}]", codeProp, generationElements.TableNameJoined));
                i++;
                if (i < countMembers) sb.Append(", ");
                if (i == countMembers) sb.Append(" ");
            }

            sb.Append(String.Format("FROM {0} INNER JOIN {1} ON {0}.[{2}] = {1}.[{3}] ", generationElements.TableName, generationElements.TableNameJoined, generationElements.Keys[0],
             generationElements.PrimaryKeyJoined));

            if (generationElements.IsTenantRelated)
            {
                var tenant = string.Format("whereTenantId:{0}", generationElements.TableName);
                sb.Append(String.Format("{0}", '{' + tenant + '}'));
            }

            sb.Append("\";");

            return sb.ToString();
        }

        public string SQLGenerateInsert(GenerationClassElements generationElements, bool IsJoined)
        {
            var sb = new StringBuilder();

            int i = 0;
            int countMembers = generationElements.Elements.Count;

            if (!IsJoined)
            {
                sb.Append(String.Format("\r\n        private const string InsertQuery = \"INSERT INTO {0} (", generationElements.TableName));
            }
            else
            {
                sb.Append(String.Format("\r\n                                               \"INSERT INTO {0} (", generationElements.TableName));
            }

            foreach (var codeProp in generationElements.Elements)
            {
                if (!(generationElements.IsIdentity && codeProp == generationElements.Keys[0]) || IsJoined)
                {
                    sb.Append(String.Format("[{0}]", codeProp)); i++;
                    if (i < countMembers) sb.Append(", ");
                    if (i == countMembers)
                    {
                        if (generationElements.IsTenantRelated)
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

            if (generationElements.Keys.Count() != 0 && !IsJoined)
            {
                List<string> temp = GenerateCurrentKeys(generationElements.Keys[0]);
                sb.Append(String.Format("OUTPUT INSERTED.{0}", temp[0]));
            }

            sb.Append(" VALUES (");
            i = 0;

            foreach (var codeProp in generationElements.Elements)
            {
                if (!(generationElements.IsIdentity && codeProp == generationElements.Keys[0]) || IsJoined)
                {
                    if (IsJoined && codeProp == generationElements.Keys[0])
                    {
                        if (generationElements.IsIdentityJoined)
                        {
                            sb.Append(String.Format("@TempPK{0}, ", generationElements.PrimaryKeyJoined));
                            i++;
                        }
                        else
                        {
                            sb.Append(String.Format("@{0}, ", generationElements.PrimaryKeyJoined));
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
                            if (generationElements.IsTenantRelated)
                            {
                                sb.Append("{values}");
                            }
                            sb.Append(");");
                        }
                    }
                }
            }

            if (IsJoined && generationElements.IsIdentityJoined)
            {
                sb.Append(String.Format("\"+ \r\n                                               \"SELECT {0} FROM @TempPKTable", generationElements.PrimaryKeyJoined));
            }

            sb.Append(" \";");

            return sb.ToString();
        }

        public string SQLGenerateInsertJoined(GenerationClassElements generationElements)
        {
            var sb = new StringBuilder();

            int i = 0;
            int countMembers = generationElements.JoinedElements.Count;
            if (generationElements.IsIdentityJoined)
            {
                string type = FindTypeSql(generationElements.JoinedClass, generationElements.PrimaryKeyJoined);
                string declaringTable = String.Format("DECLARE @TempPKTable TABLE ({0} {1}); \"+", generationElements.PrimaryKeyJoined, type);
                string declaringVariable = String.Format("\r\n                                               \"DECLARE @TempPK{0} {1}; \"+", generationElements.PrimaryKeyJoined, type);
                sb.Append(String.Format("\r\n        private const string InsertQueryJoin = \"{1}{2} \r\n                                               \"INSERT INTO {0} (", generationElements.TableNameJoined, declaringTable, declaringVariable));
            }
            else
            {
                sb.Append(String.Format("\r\n        private const string InsertQueryJoin = \"INSERT INTO {0} (", generationElements.TableNameJoined));
            }

            foreach (var codeProp in generationElements.JoinedElements)
            {
                if (!(generationElements.IsIdentityJoined && codeProp == generationElements.PrimaryKeyJoined))
                {
                    sb.Append("[{" + codeProp + "}]");
                    i++;
                    if (i < countMembers) sb.Append(", ");
                    if (i == countMembers)
                    {
                        if (generationElements.IsTenantRelated)
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

            if (generationElements.IsIdentityJoined)
            {
                sb.Append(String.Format("OUTPUT INSERTED.{0} INTO @TempPKTable", generationElements.PrimaryKeyJoined));
            }

            sb.Append(" VALUES (");
            i = 0;

            foreach (var codeProp in generationElements.JoinedElements)
            {
                if (!(generationElements.IsIdentityJoined && codeProp == generationElements.PrimaryKeyJoined))
                {
                    sb.Append("@{" + codeProp + "}");
                    i++;

                    if (i < countMembers) sb.Append(", ");
                    if (i == countMembers)
                    {
                        if (generationElements.IsTenantRelated)
                        {
                            sb.Append("{values}");
                        }
                        sb.Append(");\"+");
                    }
                }
            }

            if (generationElements.IsIdentityJoined)
            {
                sb.Append(String.Format("\r\n                                               \"SELECT @TempPK{0} = {0} FROM @TempPKTable \"+", generationElements.PrimaryKeyJoined));
            }
            else
            {
                sb.Append("\"+");
            }

            SQLGenerateInsert(generationElements, true);

            return sb.ToString();
        }

        public string GenerateConstructor(bool constructor, string name)
        {
            var sb = new StringBuilder();
            if (!constructor)
                sb.Append("public " + name + "Repository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }");

            return sb.ToString();
        }

        public string GenerateGetAll(GenerationClassElements generationElements)
    {
        var sb = new StringBuilder();

        string signature = "";
        string whereSql = ", null";
        string checkNull = "";
        string enterString = "";
        if (generationElements.FilterData != null)
        {
            enterString = "\r\n            ";
            var filterData = generationElements.FilterData;
            string parameters = "new {" + firstSymbolToLower(filterData.Name) + "}";
            signature = filterData.Type + "? " + firstSymbolToLower(filterData.Name) + " = " + filterData.DefaultValue;
            whereSql = "+WhereWithFilterData, parameters";
            checkNull = @"object parameters = null; 
            if (@p0.HasValue)
            {
               parameters = @p1;
            }";
            checkNull = checkNull.Replace("@p1", parameters);
            checkNull = checkNull.Replace("@p0", firstSymbolToLower(filterData.Name));
        }
        if (!generationElements.IsJoined)
        {

            sb.Append("\t\t public async Task<IEnumerable<" + generationElements.ClassFullName + "> GetAllAsync(" + signature + ")\n");
            sb.Append("\t\t {\n");
            sb.Append("\t\t\t " + checkNull + enterString + "var result = await DataAccessService.GetAsync< " + generationElements.ClassFullName + "> (SelectAllQuery" + generationElements.ClassName + whereSql + "\n");
            sb.Append("\t\t\t return result.ToList();\n");
            sb.Append("\t\t }\n");

            sb.Append("\t\t public IEnumerable<" + generationElements.ClassFullName + "> GetAll(" + signature + ")\n");
            sb.Append("\t\t {\n");
            sb.Append("\t\t\t " + checkNull + enterString + "var result = DataAccessService.Get<" + generationElements.ClassFullName + "> (SelectAllQuery" + generationElements.ClassName + whereSql + "\n");
            sb.Append("\t\t\t return result.ToList();\n");
            sb.Append("\t\t }\n");

        } else 
        {
            string joinedName = generationElements.JoinedClassName;

            sb.Append("\t\t public async Task<IEnumerable<" + generationElements.ClassFullName + "> GetAllAsync(" + signature + ")\n");
            sb.Append("\t\t {\n");
            sb.Append("\t\t\t " + checkNull + "var result = await DataAccessService.GetAsync< " + generationElements.ClassFullName + "> (SelectAllQuery" + generationElements.ClassName + "Join" + joinedName + whereSql + "\n");
            sb.Append("\t\t\t return result.ToList();\n");
            sb.Append("\t\t }\n");

            sb.Append("\t\t public IEnumerable<" + generationElements.ClassFullName + "> GetAll(" + signature + ")\n");
            sb.Append("\t\t {\n");
            sb.Append("\t\t\t " + checkNull + enterString + "var result = DataAccessService.Get<" + generationElements.ClassFullName + "> (SelectAllQuery" + generationElements.ClassName + "Join" + joinedName + whereSql + "\n");
            sb.Append("\t\t\t return result.ToList();\n");
            sb.Append("\t\t }\n");

        }

            return sb.ToString();
    }

        public string GenerateGetBy(RepositoryAndDo similarClass, int j)
        {
            string result = "";
            string returnType = "";
            string firstOrDefault = "";

            List<string> temp = GenerateCurrentKeys(similarClass.GenerationElements.Keys[j]);
            FilterOption filter = null;
            if (similarClass.GenerationElements.FilterData != null)
            {
                filter = similarClass.GenerationElements.FilterData;
            }

            if (j > 0)
            {
                foreach (MemberDeclarationSyntax member in similarClass.RepositoryInterface.Members)
                {
                    var cf = member as MethodDeclarationSyntax;
                    if (cf != null)
                    {
                        if ((cf.Identifier.ToString() == "GetBy" + similarClass.GenerationElements.NamePrimaryKeys[j] ||
                            cf.Identifier.ToString() == "GetBy" + similarClass.GenerationElements.NamePrimaryKeys[j] + "Async") && cf.ReturnType.ToString().Contains("IEnumerable"))
                        {
                            returnType = "IEnumerable<" + similarClass.GenerationElements.ClassFullName + ">";
                            firstOrDefault = "";
                        }
                    }
                }
            }

            if (similarClass.GenerationElements.Keys != null && similarClass.GenerationElements.FilterKeys != null &&
                similarClass.GenerationElements.Keys.Count() - similarClass.GenerationElements.FilterKeys.Count() == 0)
            {
                foreach (MemberDeclarationSyntax member in similarClass.RepositoryInterface.Members)
                {
                    var cf = member as MethodDeclarationSyntax;

                    if (cf != null)
                    {
                        for (int i = 0; i < similarClass.GenerationElements.Keys.Count(); i++)
                        {
                            if ((cf.Identifier.ToString() == "GetBy" + similarClass.GenerationElements.Keys[i] || cf.Identifier.ToString() == "GetBy" + similarClass.GenerationElements.Keys[i] + "Async")
                                && cf.ReturnType.ToString().Contains("IEnumerable"))
                            {
                                returnType = "IEnumerable<" + similarClass.GenerationElements.ClassFullName + ">";
                                firstOrDefault = "";
                            }
                        }
                    }
                }
            }

            if (returnType == "")
            {
                firstOrDefault = ".FirstOrDefault()";
                returnType = similarClass.GenerationElements.ClassFullName;
            }

            if (temp.Count() > 1)
            {
                foreach (string param in temp)
                {
                    string type = FindType(similarClass.DOClass, param);
                    if (type[type.Length - 1] == '?')
                    {
                        type = DeleteLastSymbol(type);
                    }
                    result = result + type + " " + firstSymbolToLower(param) + ",";
                }
                if (filter != null)
                {
                    result = result + filter.Type + "? " + firstSymbolToLower(filter.Name) + " = " + filter.DefaultValue;
                }
                else
                {
                    result = result.Substring(0, result.Length - 1);
                }
            }
            else
            {
                string type = FindType(similarClass.DOClass, temp[0]);
                if (type.Count() != 0 && type[type.Length - 1] == '?')
                {
                    type = DeleteLastSymbol(type);
                }
                result = type + " " + firstSymbolToLower(temp[0]);
                if (filter != null)
                {
                    result = result + ", " + filter.Type + "? " + firstSymbolToLower(filter.Name) + " = " + filter.DefaultValue;
                }
            }

            string parameters = ",new{";
            string outParameters = "";
            string checkNull = "";
            string filterParameters = "";
            for (int k = 0; k < temp.Count(); k++)
            {
                parameters += firstSymbolToLower(temp[k]);
                if (k != temp.Count() - 1)
                    parameters += ',';
            }
            parameters = parameters + "}";
            outParameters = parameters;
            string enterString = "";
            if (filter != null)
            {
                filterParameters = parameters.Substring(1, parameters.Length - 2);
                filterParameters = filterParameters + "," + firstSymbolToLower(filter.Name) + "}";
                outParameters = string.Format("And{0},parameters", filter.Name);
                checkNull = @"object parameters = @p2; 
            if (@p0.HasValue)
            {
               parameters = @p1;
            }";

                enterString = "\r\n            ";
                checkNull = checkNull.Replace("@p0", firstSymbolToLower(filter.Name));
                checkNull = checkNull.Replace("@p1", filterParameters);
                parameters = parameters.Substring(1, parameters.Length - 1);
                checkNull = checkNull.Replace("@p2", parameters);
            }

            var sb = new StringBuilder();

            if (!similarClass.GenerationElements.IsJoined)
            {
                sb.Append("\t\t public async Task<" + returnType  + "> GetBy" +similarClass.GenerationElements.NamePrimaryKeys[j] + " Async(" + result  + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + checkNull + enterString + " var result = await DataAccessService.GetAsync<" + similarClass.GenerationElements.ClassFullName + ">(SelectQuery+WhereQueryBy" + similarClass.GenerationElements.NamePrimaryKeys[j] + " " + outParameters + " );\n");
                sb.Append("\t\t\t return result" + firstOrDefault + ";\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public " + returnType + "  GetBy" + similarClass.GenerationElements.NamePrimaryKeys[j] + " (" + result + " )\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + checkNull + " " + enterString + " var result = DataAccessService.Get<" + similarClass.GenerationElements.ClassFullName + " >(SelectQuery+WhereQueryBy" + similarClass.GenerationElements.NamePrimaryKeys[j] + " " + outParameters + " );\n");
                sb.Append("\t\t\t return result" + firstOrDefault + ";\n");
                sb.Append("\t\t }\n");

            } else 
            {
                var joinedName = similarClass.GenerationElements.JoinedClassName;

                sb.Append("\t\t public async Task<" + returnType + " > GetBy" + similarClass.GenerationElements.NamePrimaryKeys[j] + " Async(" + result + " )\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + checkNull  + " " + enterString  + " var result = await DataAccessService.GetAsync < " +similarClass.GenerationElements.ClassFullName + " > (SelectQueryJoin" + joinedName  + " + WhereQueryBy" +similarClass.GenerationElements.NamePrimaryKeys[j] + " " + outParameters  + ");\n");
                sb.Append("\t\t\t return result" + firstOrDefault + ";\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public " + returnType + "  GetBy" + similarClass.GenerationElements.NamePrimaryKeys[j] + " (" + result + " )\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + checkNull + " " + enterString + " var result = DataAccessService.Get<" + similarClass.GenerationElements.ClassFullName + " >(SelectQueryJoin" + joinedName + " +WhereQueryBy" + similarClass.GenerationElements.NamePrimaryKeys[j] + " " + outParameters + " );\n");
                sb.Append("\t\t\t return result" + firstOrDefault + ";\n");
                sb.Append("\t\t }\n");
            }

            return sb.ToString();
        }

        public string GenerateInsert(RepositoryAndDo similarClass)
        {
            string returnType = "";
            string returnTypeNotAsync = "";
            string conversion = "";
            string variable = "";

            if (similarClass.GenerationElements.Keys.Count() != 0)
            {
                if (!similarClass.GenerationElements.Keys[0].Contains(","))
                {
                    returnType = FindType(similarClass.DOClass, similarClass.GenerationElements.Keys[0]);
                    if (returnType[returnType.Length - 1] == '?')
                    {
                        returnType = DeleteLastSymbol(returnType);
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

            foreach (MemberDeclarationSyntax member in similarClass.RepositoryInterface.Members)
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

            if (returnType != "")
            {
                returnType = '<' + returnType + '>';
            }

            var sb = new StringBuilder();
            var parameters = GetNewSignature(similarClass.GenerationElements);

            if (!similarClass.GenerationElements.IsJoined)
            {
                sb.Append("\t\t public async Task " + returnType + "InsertAsync(" + parameters.TypeParameter + "" + parameters.Parameter + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + variable + "await DataAccessService.InsertObjectAsync(" + parameters.Parameter + ",InsertQuery);\n");
                sb.Append("\t\t\t " + conversion + "\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public " + returnTypeNotAsync + "Insert(" + parameters.TypeParameter + "" + parameters.Parameter + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + variable + "DataAccessService.InsertObject(" + parameters.Parameter + ",InsertQuery);\n");
                sb.Append("\t\t\t " + conversion + "\n");
                sb.Append("\t\t }\n");
        
            } else 
            {
                var joinedName = similarClass.GenerationElements.JoinedClassName;

                sb.Append("\t\t public async Task" + returnType + " InsertAsync(" + parameters.TypeParameter + " " + parameters.Parameter + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + variable + " await DataAccessService.InsertObjectAsync(" + parameters.Parameter + ",InsertQueryJoin);\n");
                sb.Append("\t\t\t " + conversion + "\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public " + returnTypeNotAsync + " Insert(" + parameters.TypeParameter + " " + parameters.Parameter + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t " + variable + " DataAccessService.InsertObject(" + parameters.Parameter + ",InsertQueryJoin);\n");
                sb.Append("\t\t\t " + conversion + "\n");
                sb.Append("\t\t }\n");

            }

            return sb.ToString();
        }

        public string GenerateUpdate(GenerationClassElements generationElements, int j)
        {
            var sb = new StringBuilder();

            var parameters = GetNewSignature(generationElements);
            if (!generationElements.IsJoined)
            {
                sb.Append("\t\t public async Task UpdateBy" + generationElements.NamePrimaryKeys[j] + "Async(" + parameters.TypeParameter + " " + parameters.Parameter + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t await DataAccessService.PersistObjectAsync(" + parameters.Parameter + ",UpdateQueryBy" + generationElements.NamePrimaryKeys[j] + "+WhereQueryBy" + generationElements.NamePrimaryKeys[j] + ");\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public void UpdateBy" + generationElements.NamePrimaryKeys[j] + "(" + parameters.TypeParameter + " " + parameters.Parameter + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t DataAccessService.PersistObject(" + parameters.Parameter + ",UpdateQueryBy" + generationElements.NamePrimaryKeys[j] + "+WhereQueryBy" + generationElements.NamePrimaryKeys[j] + ");\n");
                sb.Append("\t\t }\n");

             } else 
             {
                string joinedName = generationElements.JoinedClassName;

                sb.Append("\t\t public async Task UpdateBy" + generationElements.NamePrimaryKeys[j] + "Async(" + parameters.TypeParameter + " " + parameters.Parameter + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t await DataAccessService.PersistObjectAsync(" + parameters.Parameter + ",UpdateQueryBy" + generationElements.NamePrimaryKeys[j] + "+WhereQueryBy" + generationElements.NamePrimaryKeys[j] + 
                                                                "UpdateQueryJoin" + joinedName + " + WhereQueryBy" + generationElements.NamePrimaryKeys[j] + ");\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public void UpdateBy" + generationElements.NamePrimaryKeys[j] + "(" + parameters.TypeParameter + " " + parameters.Parameter + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t DataAccessService.PersistObject(" + parameters.Parameter + ",UpdateQueryBy" + generationElements.NamePrimaryKeys[j] + "+WhereQueryBy" + generationElements.NamePrimaryKeys[j] + 
                                                                "UpdateQueryJoin" + joinedName + " + WhereQueryBy" + generationElements.NamePrimaryKeys[j] + ");\n");
                sb.Append("\t\t }\n");

            }

            return sb.ToString();
        }

        public string GenerateRemove(GenerationClassElements generationElements, int j)
        {
            var sb = new StringBuilder();

            var parameters = GetNewSignature(generationElements);
            if (!generationElements.IsJoined)
            {
                sb.Append("\t\t public async Task RemoveBy" + generationElements.NamePrimaryKeys[j] + "Async(" + parameters.TypeParameter + " " + parameters.Parameter + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t await DataAccessService.PersistObjectAsync(" + parameters.Parameter + ",DeleteQuery+WhereQueryBy" + generationElements.NamePrimaryKeys[j] + ");\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public void RemoveBy" + generationElements.NamePrimaryKeys[j] + "(" + parameters.TypeParameter + " " + parameters.Parameter + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t DataAccessService.PersistObject(" + parameters.Parameter + ",DeleteQuery+WhereQueryBy" + generationElements.NamePrimaryKeys[j] + ");\n");
                sb.Append("\t\t }\n");

            } else
            {
                var joinedName = generationElements.JoinedClassName;

                sb.Append("\t\t public async Task RemoveBy" + generationElements.NamePrimaryKeys[j] + "Async(" + parameters.TypeParameter + " " + parameters.Parameter + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t await DataAccessService.PersistObjectAsync(" + parameters.Parameter + ",SelectIntoTempTableJoin" + joinedName + "+WhereQueryBy" + generationElements.NamePrimaryKeys[j] + "+DeleteQueryJoin" + joinedName + ");\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public void RemoveBy" + generationElements.NamePrimaryKeys[j] + "(" + parameters.TypeParameter + " " + parameters.Parameter + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t DataAccessService.PersistObject(" + parameters.Parameter + ",SelectIntoTempTableJoin" + joinedName + "+WhereQueryBy" + generationElements.NamePrimaryKeys[j] + "+DeleteQueryJoin" + joinedName + ");\n");
                sb.Append("\t\t }\n");
            }

            return sb.ToString();
        }

        public string GenerateRemoveById(RepositoryAndDo similarClass, int j)
        {
            var sb = new StringBuilder();
            string result = "";
            List<string> temp = GenerateCurrentKeys(similarClass.GenerationElements.Keys[j]);
            if (temp.Count() > 1)
            {
                foreach (string param in temp)
                {
                    string type = FindType(similarClass.DOClass, param);
                    if (type.Count() != 0 && type[type.Length - 1] == '?')
                    {
                        type = DeleteLastSymbol(type);
                    }
                    result = result + type + " " + firstSymbolToLower(param) + ",";
                }
                result = result.Substring(0, result.Length - 1);
            }
            else
            {
                string type = FindType(similarClass.DOClass, temp[0]);
                if (type.Count() != 0 && type[type.Length - 1] == '?')
                {
                    type = DeleteLastSymbol(type);
                }
                result = type + " " + firstSymbolToLower(temp[0]);
            }

            string parameters = "";

            for (int k = 0; k < temp.Count(); k++)
            {
                parameters += firstSymbolToLower(temp[k]);
                if (k != temp.Count() - 1)
                    parameters += ',';
            }

            if (!similarClass.GenerationElements.IsJoined)
            {
                sb.Append("\t\t public async Task RemoveBy" + similarClass.GenerationElements.NamePrimaryKeys[j] + "Async(" + result + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t await DataAccessService.PersistObjectAsync<" + similarClass.GenerationElements.ClassFullName + ">(DeleteQuery+WhereQueryBy" + similarClass.GenerationElements.NamePrimaryKeys[j] + " , new { " + parameters + " });\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public void RemoveBy" + similarClass.GenerationElements.NamePrimaryKeys[j] + "(" + result + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t DataAccessService.PersistObject<" + similarClass.GenerationElements.ClassFullName + ">(DeleteQuery+WhereQueryBy" + similarClass.GenerationElements.NamePrimaryKeys[j] + " , new { " + parameters + " });\n");
                sb.Append("\t\t }\n");

            } else 
            {
                var joinedName = similarClass.GenerationElements.JoinedClassName;

                sb.Append("\t\t public async Task RemoveBy" + similarClass.GenerationElements.NamePrimaryKeys[j] + "Async(" + result + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t await DataAccessService.PersistObjectAsync<" + similarClass.GenerationElements.ClassFullName + ">(SelectIntoTempTableJoin" + joinedName + "+WhereQueryBy" + similarClass.GenerationElements.NamePrimaryKeys[j] + "+DeleteQueryJoin" + joinedName + ", new { " + parameters + " });\n");
                sb.Append("\t\t }\n");

                sb.Append("\t\t public void RemoveBy" + similarClass.GenerationElements.NamePrimaryKeys[j] + "(" + result + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t DataAccessService.PersistObject<" + similarClass.GenerationElements.ClassFullName + ">(SelectIntoTempTableJoin" + joinedName + "+WhereQueryBy" + similarClass.GenerationElements.NamePrimaryKeys[j] + "+DeleteQueryJoin" + joinedName + ", new { " + parameters + " });\n");
                sb.Append("\t\t }\n");

            }
            
            return sb.ToString();
        }

        public string GenerateUnreleasedMethod(RepositoryAndDo similarClass, List<MethodRealization> allMethods)
        {
            var sb = new StringBuilder();


            if (!similarClass.GenerationElements.IsJoined)
            {
                sb.Append(SQLGenerateGetAll(similarClass.GenerationElements));
                sb.Append(SQLGenerateInsert(similarClass.GenerationElements, false));
                sb.Append(SQLGenerateGetBy(similarClass.GenerationElements));
                sb.Append(SQLGenerateRemove(similarClass.GenerationElements));

                for (int i = 0; i < similarClass.GenerationElements.Keys.Count(); i++)
                {
                    sb.Append(SQLGenerateWhere(similarClass.GenerationElements, i));
                    sb.Append(SQLGenerateUpdate(similarClass.GenerationElements, i));
                }
            }
            else
            {
                sb.Append(SQLGenerateGetAllJoined(similarClass.GenerationElements));
                sb.Append(SQLGenerateInsertJoined(similarClass.GenerationElements));
                sb.Append(SQLGenerateGetByJoined(similarClass.GenerationElements));
                sb.Append(SQLGenerateRemoveJoined(similarClass.GenerationElements));
                sb.Append(SQLGenerateUpdateJoined(similarClass.GenerationElements));

                for (int i = 0; i < similarClass.GenerationElements.Keys.Count(); i++)
                {
                    sb.Append(SQLGenerateWhere(similarClass.GenerationElements, i));
                    sb.Append(SQLGenerateUpdate(similarClass.GenerationElements, i));
                }
            }

            sb.Append("\r\n");

            foreach (MethodRealization DOmethods in allMethods)
            {
                if (DOmethods.Method == "GetAllAsync" || DOmethods.Method == "GetAll")
                {
                    if (DOmethods.Realization)
                    {
                        sb.Append(GenerateGetAll(similarClass.GenerationElements));
                    }
                    else
                    {
                        sb.Append("/*\r\n");
                        sb.Append(GenerateGetAll(similarClass.GenerationElements));
                        sb.Append("*/\r\n");
                    }
                }

                for (int j = 0; j < similarClass.GenerationElements.Keys.Count(); j++)
                {
                    if (DOmethods.Method == "GetBy" + similarClass.GenerationElements.NamePrimaryKeys[j] + "Async"
                        || DOmethods.Method == "GetBy" + similarClass.GenerationElements.NamePrimaryKeys[j])
                    {
                        if (DOmethods.Realization)
                        {
                            sb.Append(GenerateGetBy(similarClass, j));
                        }
                        else
                        {
                            sb.Append("/*\r\n");
                            sb.Append(GenerateGetBy(similarClass, j));
                            sb.Append("*/\r\n");
                        }
                    }
                }
                if (DOmethods.Method == "InsertAsync" || DOmethods.Method == "Insert")
                {
                    if (DOmethods.Realization)
                    {
                        sb.Append(GenerateInsert(similarClass));
                    }
                    else
                    {
                        sb.Append("/*\r\n");
                        sb.Append(GenerateInsert(similarClass));
                        sb.Append("*/\r\n");
                    }
                }
                for (int j = 0; j < similarClass.GenerationElements.Keys.Count(); j++)
                {
                    if (DOmethods.Method == "UpdateBy" + similarClass.GenerationElements.NamePrimaryKeys[j] + "Async"
                        || DOmethods.Method == "UpdateBy" + similarClass.GenerationElements.NamePrimaryKeys[j])
                    {
                        if (DOmethods.Realization)
                        {
                            sb.Append(GenerateUpdate(similarClass.GenerationElements, j));
                        }
                        else
                        {
                            sb.Append("/*\r\n");
                            sb.Append(GenerateUpdate(similarClass.GenerationElements, j));
                            sb.Append("*/\r\n");
                        }
                    }
                }

                for (int j = 0; j < similarClass.GenerationElements.Keys.Count(); j++)
                    if (DOmethods.Method == "RemoveBy" + similarClass.GenerationElements.NamePrimaryKeys[j] + "Async"
                        || DOmethods.Method == "RemoveBy" + similarClass.GenerationElements.NamePrimaryKeys[j])
                    {
                        if (DOmethods.Realization)
                        {
                            sb.Append(GenerateRemove(similarClass.GenerationElements, j));
                            sb.Append(GenerateRemoveById(similarClass, j));
                        }
                        else
                        {
                            sb.Append("/*\r\n");
                            sb.Append(GenerateRemove(similarClass.GenerationElements, j));
                            sb.Append(GenerateRemoveById(similarClass, j));
                            sb.Append("*/\r\n");
                        }
                    }
            }

            return sb.ToString();
        }

        public async void GenerateRepository()
        {
            Solution = await _workspace.OpenSolutionAsync(SolutionPath);
            Project = Solution.Projects.First(x => x.Name == ProjectName);

            GeneratorHelper.Solution = Solution;
            GeneratorHelper.Project = Project;

            var listOfSimilarClasses = FindRepository();
            var sb = new StringBuilder();

            var Namespaces = FindNamespace(ref listOfSimilarClasses);

            foreach (var similarClass in listOfSimilarClasses)
            {
                if (Namespaces.Count == 0)
                {
                    Namespaces.Add(similarClass.GenerationElements.Namespace);
                }
                else
                {
                    if (!Namespaces.Contains(similarClass.GenerationElements.Namespace))
                    {
                        Namespaces.Add(similarClass.GenerationElements.Namespace);
                    }
                }
            }

            if (Namespaces.Count != 0)
            {
                foreach (var similarClass in listOfSimilarClasses)
                {
                    sb.Append(
                        "//------------------------------------------------------------------------------\n"
                        + "// <auto-generated>\n"
                        + "//     This code was generated from a template.\n"
                        + "//\n"
                        + "//     Manual changes to this file may cause unexpected behavior in your application.\n"
                        + "//     Manual changes to this file will be overwritten if the code is regenerated.\n"
                        + "// </auto-generated>\n"
                        + "//------------------------------------------------------------------------------\n\n");

                    sb.Append("using System.Collections.Generic;\n"
                        + "using System.Linq;"
                        + "using System.Threading.Tasks;\n\n");

                    sb.Append("namespace " + similarClass.GenerationElements.Namespace + "\n"
                        + "{ \n");

                    if (similarClass.RepositoryClass != null)
                    {
                        sb.Append("\t public partial class " + similarClass.DOClass.Identifier + "Repository \n \t {");
                    }
                    else
                    {
                        sb.Append("\t public class " + similarClass.DOClass.Identifier + "Repository:RepositoryBase, " + similarClass.RepositoryInterface.Identifier.FullSpan + " \n \t { \n");
                    }

                    sb.Append(GenerateConstructor(similarClass.GenerationElements.Constructor, similarClass.DOClass.Identifier.ToString()));

                    similarClass.GenerationElements.ClassName = similarClass.DOClass.Identifier.ToString();
                    similarClass.GenerationElements.ClassFullName = similarClass.DOClass.Identifier.FullSpan.ToString();

                    var interfaces = GeneratorHelper.GetImplementedInterfaces(RepositoryMainPlace, similarClass.DOClass);
                    similarClass.GenerationElements.IsTenantRelated = FindTenant(similarClass.DOClass);

                    similarClass.GenerationElements.IsJoined = false;
                    RepositoryAndDo baseSimilarClass = null;

                    var baseClasses = similarClass.DOClass.BaseList;
                    var baseClass = baseClasses?.Types.OfType<ClassDeclarationSyntax>().FirstOrDefault();

                    if (baseClass != null && baseClass.Identifier.FullSpan.ToString() != "System.Object" && listOfSimilarClasses.FirstOrDefault(x => x.DOClass.Identifier.ToString() == baseClass.Identifier.ToString()) != null)
                    {
                        baseSimilarClass = listOfSimilarClasses.FirstOrDefault(x => x.DOClass.Identifier.ToString() == baseClass.Identifier.ToString());
                        similarClass.GenerationElements.IsJoined = true;
                        similarClass.GenerationElements.JoinedClass = baseSimilarClass.DOClass;
                        similarClass.GenerationElements.PrimaryKeyJoined = baseSimilarClass.GenerationElements.Keys[0];
                        similarClass.GenerationElements.JoinedElements = baseSimilarClass.GenerationElements.Elements;
                        similarClass.GenerationElements.TableNameJoined = baseSimilarClass.GenerationElements.TableName;
                        similarClass.GenerationElements.IsIdentityJoined = FindTenant(baseSimilarClass.DOClass);
                        similarClass.GenerationElements.JoinedClassName = baseSimilarClass.DOClass.Identifier.ToString();
                    }

                    FindUnreleasedMethod(similarClass);

                    similarClass.GenerationElements.NamePrimaryKeys = GenerateNamePrimaryKeys(similarClass.GenerationElements.Keys);

                    List<MethodRealization> allmethods = null;

                    if (similarClass.GenerationElements.IsJoined)
                    {
                        allmethods = AllPosibleMethodsWithJoin(similarClass.GenerationElements.NamePrimaryKeys, similarClass.GenerationElements.JoinedClassName);
                    }
                    else
                    {
                        allmethods = AllPosibleMethods(similarClass.GenerationElements.NamePrimaryKeys);
                    }

                    foreach (var _method in allmethods)
                    {
                        foreach (var method in similarClass.UnRealeasedMethod)
                        {
                            if (_method.Method.Trim() == method.Identifier.ToString().Trim() || _method.Method.Trim() + "Async" == method.Identifier.ToString().Trim())
                            {
                                _method.Realization = true;
                            }
                        }
                    }

                    sb.Append(GenerateUnreleasedMethod(similarClass, allmethods));

                    sb.Append("/t/t } /n }");

                    CreateDocument(sb.ToString(), "Repositories/" + similarClass.DOClass.Identifier + ".cs");
                }
            }

            ApplyChanges();
        }

        private static void CreateDocument(string code, string fileName)
        {
            if (fileName != null)
            {
                var folders = fileName.Split('/');
                fileName = folders[folders.Length - 1];
                folders = folders.Where((val, idx) => idx != folders.Length - 1).ToArray();

                var sourceText = SourceText.From(code);
                tasks.Add(Tuple.Create(fileName, sourceText, folders));
            }
        }

        private void ApplyChanges()
        {
            var project = Solution.Projects.First(x => x.Name == RepositoryMainPlace);

            foreach (var doc in tasks)
            {
                var oldDocument = project.Documents.FirstOrDefault(x => x.Name == doc.Item1);

                if (oldDocument != null)
                {
                    bool isEqual = oldDocument.Folders.SequenceEqual(doc.Item3);

                    if (isEqual)
                    {
                        project = project.RemoveDocument(oldDocument.Id);
                    }
                }

                var newDocument = project.AddDocument(doc.Item1, doc.Item2, doc.Item3);
                project = newDocument.Project;
            }

            _workspace.TryApplyChanges(project.Solution);
        }
    }
}


