using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCFGenerator.RepositoriesGeneration.Heplers;
using WCFGenerator.RepositoriesGeneration.Infrastructure;

namespace WCFGenerator.RepositoriesGeneration.Core
{
    internal class CodeClassGeneratorVersionedRepositoryService : BaseCodeClassGeneratorRepository
    {
        #region Fields

        private readonly List<RepositoryFieldInfo> _fieldInfoList = new List<RepositoryFieldInfo>();

        private readonly string _cacheRepository = CodeClassGeneratorCacheRepository.RepositoryKind + "Repository";
        private readonly string _versionRepository = CodeClassGeneratorVersionsRepository.RepositoryKind + "Repository";
        private const string DataAccessControllerField = "_dataAccessController";
        private const string DateTimeServiceField = "_dateTimeService";

        #endregion

        #region Properties

        private string CacheRepositoryType
        {
            get { return RepositoryInfo.RepositoryNamespace + "." + RepositoryInfo.ClassName + _cacheRepository; }
        }

        private string CacheRepositoryField
        {
            get { return "_" + CacheRepositoryType.Split('.').Last().FirstSymbolToLower(); }
        }

        private string VersionRepositoryType
        {
            get { return RepositoryInfo.RepositoryNamespace + "." + RepositoryInfo.ClassName + _versionRepository; }
        }

        private string VersionRepositoryField
        {
            get { return "_" + VersionRepositoryType.Split('.').Last().FirstSymbolToLower(); }
        }

        private IEnumerable<RepositoryFieldInfo> AllRepositoryFieldInfos
        {
            get
            {
                if (_fieldInfoList.Count == 0)
                {
                    _fieldInfoList.AddRange(GetFieldInfos());
                }
                return _fieldInfoList;
            }
        }

        #endregion

        #region Overrides of BaseCodeClassGeneratorRepository

        public override RepositoryType RepositoryType
        {
            get { return RepositoryType.VersionService; }
        }

        public override string RepositoryKindName
        {
            get { return "Service"; }
        }

        public override string GetUsings()
        {
            var sb = new StringBuilder(base.GetUsings());

            foreach (var nameSpace in RepositoryInfo.RequiredNamespaces[RepositoryType.VersionService])
            {
                sb.AppendLine("using " + nameSpace + ";");
            }

            return sb.ToString();
        }

        public override string GetClassDeclaration()
        {
            return "public partial class " + RepositoryName + (RepositoryInfo.RepositoryInterfaceName != null ? " : " + RepositoryInfo.RepositoryInterfaceName : "");
        }

        public override string GetConstructors()
        {
            var constructorParamers = new List<string>()
            {
                RepositoryInfo.DataAccessControllerTypeName + " dataAccessController",
                RepositoryInfo.DataAccessServiceTypeName + " dataAccessService",
                RepositoryInfo.DateTimeServiceTypeName + " dateTimeService"
            };
            constructorParamers.AddRange(AllRepositoryFieldInfos.Where(i => !i.InitNew).Select(f => f.InterfaceName + " " + f.TypeName.FirstSymbolToLower()).ToList());

            var parameters = string.Join(",\r\n", constructorParamers);

            var sb = new StringBuilder();

            sb.Append("public " + RepositoryName + "(");
            sb.Append(parameters);
            sb.AppendLine(")");
            sb.AppendLine("{");
            sb.AppendLine(DataAccessControllerField + " = " + "dataAccessController;");
            sb.AppendLine(DateTimeServiceField + " = " + "dateTimeService;");

            foreach (var f in AllRepositoryFieldInfos)
            {
                var init = f.InitNew ? f.Name + " = new " + f.TypeName + "(dataAccessService)" : f.Name + " = (" + f.TypeName + ")" + f.TypeName.FirstSymbolToLower();
                sb.AppendLine(init + ";");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }

        public override string GetFields()
        {
            var sb = new StringBuilder();
            sb.AppendLine("private " + RepositoryInfo.DataAccessControllerTypeName + " " + DataAccessControllerField + ";");
            sb.AppendLine("private " + RepositoryInfo.DateTimeServiceTypeName + " " + DateTimeServiceField + ";");
            foreach (var f in AllRepositoryFieldInfos)
            {
                sb.AppendLine("private " + f.TypeName + " " + f.Name + ";");
            }
            return sb.ToString();
        }

        public override string GetMethods()
        {
            var sb = new StringBuilder();

            // RepositoryMethod.GetAll
            var getAllMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.GetAll)
                .Aggregate("", (s, method) => s + GenerateGetAll(method));

            if (!string.IsNullOrEmpty(getAllMethods))
                sb.AppendLine(getAllMethods);

            // RepositoryMethod.GetBy - for primary key
            var getByPrimaryKeyMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.GetBy && m.FilterInfo.FilterType == FilterType.PrimaryKey)
                .Aggregate("", (s, method) => s + GenerateGetByPrimaryKey(method));

            if (!string.IsNullOrEmpty(getByPrimaryKeyMethods))
                sb.AppendLine(getByPrimaryKeyMethods);

            // RepositoryMethod.GetBy - for filter key
            var getByFilterKeyMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.GetBy && m.FilterInfo.FilterType == FilterType.FilterKey)
                .Aggregate("", (s, method) => s + GenerateGetByFilterKey(method));

            if (!string.IsNullOrEmpty(getByFilterKeyMethods))
                sb.AppendLine(getByFilterKeyMethods);

            // RepositoryMethod.GetBy - for version key
            var getByVersionKeyMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.GetBy && m.FilterInfo.FilterType == FilterType.VersionKey)
                .Aggregate("", (s, method) => s + GenerateGetByVersionKey(method));

            if (!string.IsNullOrEmpty(getByVersionKeyMethods))
                sb.AppendLine(getByVersionKeyMethods);

            // RepositoryMethod.Insert
            var insertMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.Insert)
                .Aggregate("", (s, method) => s + GenerateInsert(method));

            if (!string.IsNullOrEmpty(insertMethods))
                sb.AppendLine(insertMethods);

            // RepositoryMethod.UpdateBy
            var updateByMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.UpdateBy && m.FilterInfo.FilterType != FilterType.VersionKey)
                .Aggregate("", (s, method) => s + GenerateUpdate(method));

            if (!string.IsNullOrEmpty(updateByMethods))
                sb.AppendLine(updateByMethods);

            // Update many to many
            var updateManyToManyMethods = GenerateUpdateManyToMany();

            if (!string.IsNullOrEmpty(updateManyToManyMethods))
                sb.AppendLine(updateManyToManyMethods);

            // RepositoryMethod.RemoveBy - primary key
            var removeByPrimaryKey = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.RemoveBy && m.FilterInfo.FilterType == FilterType.PrimaryKey)
                .Aggregate("", (s, method) => s + GenerateRemoveByPrimaryKey(method));

            if (!string.IsNullOrEmpty(removeByPrimaryKey))
                sb.AppendLine(removeByPrimaryKey);

            // RepositoryMethod.RemoveBy - filter key
            var removeByFilterKey = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.RemoveBy && m.FilterInfo.FilterType == FilterType.FilterKey)
                .Aggregate("", (s, method) => s + GenerateRemoveByFilterKey(method));

            if (!string.IsNullOrEmpty(removeByFilterKey))
                sb.AppendLine(removeByFilterKey);

            return sb.ToString();
        }

        #endregion

        #region Private

        private string GenerateGetAll(MethodImplementationInfo method)
        {
            var specialParameterIsDeleted = RepositoryInfo.SpecialOptionsIsDeleted.Parameters.First();
            var specialMethodParameterIsDeleted = specialParameterIsDeleted.TypeName + "? " + specialParameterIsDeleted.Name.FirstSymbolToLower() + " = " + specialParameterIsDeleted.DefaultValue;
            var specialMethodParameterIsDeletedName = specialParameterIsDeleted.Name.FirstSymbolToLower();

            var addIsDeletedFilter = RepositoryInfo.IsVersioning && RepositoryInfo.IsDeletedExist;
            var returnType = "IEnumerable<" + RepositoryInfo.ClassFullName + ">";

            var parameters = addIsDeletedFilter ? specialMethodParameterIsDeleted : "";
            var parameterNames = addIsDeletedFilter ? specialMethodParameterIsDeletedName : "";

            var sb = new StringBuilder();

            // Synchronous method
            sb.AppendLine("public " + returnType + " GetAll(" + parameters + ")");
            sb.AppendLine("{");
            sb.AppendLine("return " + CacheRepositoryField + ".GetAll(" + parameterNames + ");");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task<" + returnType + "> GetAllAsync(" + parameters + ")");
            sb.AppendLine("{");
            sb.AppendLine("return await " + CacheRepositoryField + ".GetAllAsync(" + parameterNames + ");");
            sb.AppendLine("}");

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateGetByPrimaryKey(MethodImplementationInfo method)
        {
            var code = GenerateGetByKey(method);
            return method.RequiresImplementation ? code : code.SurroundWithComments();
        }

        private string GenerateGetByFilterKey(MethodImplementationInfo method)
        {
            var code = GenerateGetByKey(method);
            return method.RequiresImplementation ? code : code.SurroundWithComments();
        }

        private string GenerateGetByVersionKey(MethodImplementationInfo method)
        {
            var code = GenerateGetByKey(method);
            return method.RequiresImplementation ? code : code.SurroundWithComments();
        }

        private string GenerateGetByKey(MethodImplementationInfo method)
        {
            var filter = method.FilterInfo;
            var sb = new StringBuilder();

            var parameters = filter.Parameters.Select(k => k.TypeName + " " + k.Name.FirstSymbolToLower()).ToList();
            var parameterNames = filter.Parameters.Select(k => k.Name.FirstSymbolToLower()).ToList();

            var firstOverloadParameters = filter.Parameters.Select(k => k.TypeName + " " + k.Name.FirstSymbolToLower()).ToList();
            var firstOverloadParameterNames = filter.Parameters.Select(k => k.Name.FirstSymbolToLower()).ToList();

            var filterBySliceDate = filter.FilterType != FilterType.VersionKey && RepositoryInfo.IsModifiedExist;
            var filterByIsDeleted = RepositoryInfo.IsModifiedExist;

            if (filterBySliceDate)
            {
                var specialParameterModified = RepositoryInfo.SpecialOptionsModified.Parameters.First();
                var specialMethodParameterModified = specialParameterModified.TypeName + " " + specialParameterModified.Name.FirstSymbolToLower();
                var specialMethodParameterModifiedName = specialParameterModified.Name.FirstSymbolToLower();

                parameters.Add(specialMethodParameterModified);
                parameterNames.Add(specialMethodParameterModifiedName);
            }

            // last parameter - because have default value
            if (filterByIsDeleted)
            {
                var specialParameterIsDeleted = RepositoryInfo.SpecialOptionsIsDeleted.Parameters.First();
                var specialMethodParameterIsDeleted = specialParameterIsDeleted.TypeName + "? " + specialParameterIsDeleted.Name.FirstSymbolToLower() + " = " + specialParameterIsDeleted.DefaultValue;
                var specialMethodParameterIsDeletedName = specialParameterIsDeleted.Name.FirstSymbolToLower();

                parameters.Add(specialMethodParameterIsDeleted);
                parameterNames.Add(specialMethodParameterIsDeletedName);
                firstOverloadParameters.Add(specialMethodParameterIsDeleted);
                firstOverloadParameterNames.Add(specialMethodParameterIsDeletedName);
            }

            var methodParameters = string.Join(", ", parameters);
            var methodParameterNames = string.Join(", ", parameterNames);

            var firstOverloadMethodParameters = string.Join(", ", firstOverloadParameters);
            var firstOverloadMethodParameterNames = string.Join(", ", firstOverloadParameterNames);

            var returnType = method.ReturnType.IsEnumerable() ? "IEnumerable<" + RepositoryInfo.ClassFullName + ">" : RepositoryInfo.ClassFullName;
            var returnFunc = method.ReturnType.IsEnumerable() ? "return result.ToList();" : "return result;";

            var overloads = new[]
            {
                new {repository = VersionRepositoryField, parameters = methodParameters, parameterNames = methodParameterNames, needImplement = true},
                new {repository = CacheRepositoryField, parameters = firstOverloadMethodParameters, parameterNames = firstOverloadMethodParameterNames, needImplement = filter.FilterType != FilterType.VersionKey}
            };

            var methods = overloads.Where(m => m.needImplement).ToList();

            foreach (var overload in methods)
            {
                // Synchronous method
                sb.AppendLine("public " + returnType + " GetBy" + filter.Key + "(" + overload.parameters + ")");
                sb.AppendLine("{");
                sb.AppendLine("var result = " + overload.repository + ".GetBy" + filter.Key + "(" + overload.parameterNames + ");");
                sb.AppendLine(returnFunc);
                sb.AppendLine("}");
                sb.AppendLine();

                //Asynchronous method
                sb.AppendLine("public async Task<" + returnType + "> GetBy" + filter.Key + "Async(" + overload.parameters + ")");
                sb.AppendLine("{");
                sb.AppendLine("var result = await " + overload.repository + ".GetBy" + filter.Key + "Async(" + overload.parameterNames + ");");
                sb.AppendLine(returnFunc);
                sb.AppendLine("}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private string GenerateInsert(MethodImplementationInfo method)
        {
            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();

            var versionKeyProperty = parameterName + "." + RepositoryInfo.VersionKeyName;
            var methodParameter = RepositoryInfo.ClassFullName + " " + parameterName;

            var updateMethodNames = GetUpdateMethodNames();
            var updateMethods = updateMethodNames.Select(name => name + "(" + parameterName + ");").ToList();

            var returnType = "Guid";
            var returnTypeAsync = "Task<" + returnType + ">";
            var returnFunc = "return " + versionKeyProperty + ";";

            var sb = new StringBuilder();

            // synchronous method
            sb.AppendLine("public " + returnType + " Insert(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(parameterName + ".Modified = " + DateTimeServiceField + ".CurrentDateTimeOffset;");
            sb.AppendLine(parameterName + ".ModifiedBy = " + DataAccessControllerField + ".EmployeeId.Value;");
            sb.AppendLine(versionKeyProperty + " = Guid.NewGuid();");

            foreach (var key in RepositoryInfo.PrimaryKeys)
            {
                var primaryKeyProperty = parameterName + "." + key.Name;

                if (key.TypeName.IsInt()) // int skipped on insert
                {
                    sb.AppendLine(primaryKeyProperty + " = 0;");
                }
                else if (key.TypeName.IsGuid()) // throw ArgumentException if primary key if guid and empty, 
                {
                    sb.AppendLine("if(" + primaryKeyProperty + " == null || " + primaryKeyProperty + "== Guid.Empty )");
                    sb.AppendLine("{");
                    sb.AppendLine("throw new ArgumentException(" + RepositoryInfo.PrimaryKeyName.SurroundWithQuotes() + ");");
                    sb.AppendLine("}");
                }
            }

            sb.AppendLine(VersionRepositoryField + ".Insert(" + parameterName + ");");
            sb.AppendLine(CacheRepositoryField + ".Insert(" + parameterName + ");");

            foreach (var m in updateMethods)
            {
                sb.AppendLine(m);
            }

            sb.AppendLine(returnFunc);

            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async " + returnTypeAsync + " InsertAsync(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(parameterName + ".Modified = " + DateTimeServiceField + ".CurrentDateTimeOffset;");
            sb.AppendLine(parameterName + ".ModifiedBy = " + DataAccessControllerField + ".EmployeeId.Value;");
            sb.AppendLine(versionKeyProperty + " = Guid.NewGuid();");

            foreach (var key in RepositoryInfo.PrimaryKeys)
            {
                var primaryKeyProperty = parameterName + "." + key.Name;

                if (key.TypeName.IsInt()) // int skipped on insert
                {
                    sb.AppendLine(primaryKeyProperty + " = 0;");
                }
                else if (key.TypeName.IsGuid()) // throw ArgumentException if primary key if guid and empty, 
                {
                    sb.AppendLine("if(" + primaryKeyProperty + " == null || " + primaryKeyProperty + "== Guid.Empty )");
                    sb.AppendLine("{");
                    sb.AppendLine("throw new ArgumentException(" + RepositoryInfo.PrimaryKeyName.SurroundWithQuotes() + ");");
                    sb.AppendLine("}");
                }
            }

            sb.AppendLine("await " + VersionRepositoryField + ".InsertAsync(" + parameterName + ");");
            sb.AppendLine("await " + CacheRepositoryField + ".InsertAsync(" + parameterName + ");");

            foreach (var m in updateMethods)
            {
                sb.AppendLine(m);
            }

            sb.AppendLine(returnFunc);

            sb.AppendLine("}");


            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateUpdate(MethodImplementationInfo method)
        {
            var filter = method.FilterInfo;
            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var methodParameter = RepositoryInfo.ClassFullName + " " + parameterName;

            var sb = new StringBuilder();

            // Synchronous method
            sb.AppendLine("public void UpdateBy" + filter.Key + "(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(parameterName + ".Modified = " + DateTimeServiceField + ".CurrentDateTimeOffset;");
            sb.AppendLine(parameterName + ".ModifiedBy = " + DataAccessControllerField + ".EmployeeId.Value;");
            sb.AppendLine(parameterName + "." + RepositoryInfo.VersionKeyName + " = Guid.NewGuid();");
            sb.AppendLine(VersionRepositoryField + ".Insert(" + parameterName + ");");
            sb.AppendLine(CacheRepositoryField + ".UpdateBy" + filter.Key + "(" + parameterName + ");");

            var updateMethodNames = GetUpdateMethodNames();
            var updateMethods = updateMethodNames.Select(name => name + "(" + parameterName + ");").ToList();

            foreach (var m in updateMethods)
            {
                sb.AppendLine(m);
            }

            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task UpdateBy" + filter.Key + "Async(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(parameterName + ".Modified = " + DateTimeServiceField + ".CurrentDateTimeOffset;");
            sb.AppendLine(parameterName + ".ModifiedBy = " + DataAccessControllerField + ".EmployeeId.Value;");
            sb.AppendLine(parameterName + "." + RepositoryInfo.VersionKeyName + " = Guid.NewGuid();");
            sb.AppendLine("await " + VersionRepositoryField + ".InsertAsync(" + parameterName + ");");
            sb.AppendLine("await " + CacheRepositoryField + ".UpdateBy" + filter.Key + "Async(" + parameterName + ");");

            // if async
            // var updateMethodsAsync = RepositoryInfo.Many2ManyInfo.Select(info => "Update" + info.ManyToManyRepositoryInfo.ClassName + "Async(" + parameterName + ");");

            foreach (var m in updateMethods)
            {
                sb.AppendLine(m);
            }

            sb.AppendLine("}");

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateUpdateManyToMany()
        {
            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var methodParameter = RepositoryInfo.ClassFullName + " " + parameterName;

            var sb = new StringBuilder();

            foreach (var info in RepositoryInfo.Many2ManyInfo)
            {
                var manyToManyEntityName = info.ManyToManyRepositoryInfo.ClassName;
                var manyToManyCacheRepositoryFieldName = AllRepositoryFieldInfos.First(f => f.TypeName.EndsWith(info.ManyToManyEntytyType.Split('.').Last() + _cacheRepository)).Name;
                var manyToManyVersionRepositoryFieldName = AllRepositoryFieldInfos.First(f => f.TypeName.EndsWith(info.ManyToManyEntytyType.Split('.').Last() + _versionRepository)).Name;

                var entityCacheRepositoryName = AllRepositoryFieldInfos.First(f => f.TypeName.Split('.').Last()== info.EntityRepositoryInfo.ClassName + _cacheRepository).Name;

                var primaryKeyName = RepositoryInfo.PrimaryKeyName;
                var versionKeyName = RepositoryInfo.VersionKeyName;
                var primaryKeyName2 = info.EntityRepositoryInfo.PrimaryKeyName;
                var versionKeyName2 = info.EntityRepositoryInfo.VersionKeyName;
                var propertyName = info.PropertyName;

                // sync method
                sb.AppendLine("private void Update" + manyToManyEntityName + "(" + methodParameter + ")");
                sb.AppendLine("{");

                sb.AppendLine("if (" + parameterName + "." + propertyName + " == null)");
                sb.AppendLine(parameterName + "." + propertyName + " = " + manyToManyCacheRepositoryFieldName + ".GetBy" + primaryKeyName + "(" + parameterName + "." + primaryKeyName + ").Select(i => i." + primaryKeyName2 + ");");

                sb.AppendLine("var listOf" + manyToManyEntityName + " = " + parameterName + "." + propertyName);
                sb.AppendLine(".Select(ids => " + entityCacheRepositoryName + ".GetBy" + primaryKeyName2 + "(ids, null))");
                sb.AppendLine(".Select(item => new " + info.ManyToManyEntytyType + "()");
                sb.AppendLine("{");
                sb.AppendLine(primaryKeyName2 + " = item." + primaryKeyName2 + ",");
                sb.AppendLine(versionKeyName2 + " = item." + versionKeyName2 + ",");
                sb.AppendLine(primaryKeyName + " = " + parameterName + "." + primaryKeyName + ",");
                sb.AppendLine(versionKeyName + " = " + parameterName + "." + versionKeyName + ",");
                sb.AppendLine("}).ToList();");

                sb.AppendLine(manyToManyCacheRepositoryFieldName + ".RemoveBy" + primaryKeyName + "(" + parameterName + "." + primaryKeyName + ");");

                sb.AppendLine("foreach (var mt in listOf" + manyToManyEntityName + ")");
                sb.AppendLine("{");
                sb.AppendLine("mt.Modified = " + DateTimeServiceField + ".CurrentDateTimeOffset;");
                sb.AppendLine("mt.ModifiedBy = " + parameterName + ".ModifiedBy;");
                sb.AppendLine(manyToManyCacheRepositoryFieldName + ".Insert(mt);");
                sb.AppendLine(manyToManyVersionRepositoryFieldName + ".Insert(mt);");
                sb.AppendLine("}");
                sb.AppendLine("}");

                /*  Async method
                sb.AppendLine();

                sb.AppendLine("private Task Update" + manyToManyEntityName + "Async(" + methodParameter ")");
                sb.AppendLine("{");
                
                sb.AppendLine("if (" + parameterName + "." + propertyName + " == null)");
                sb.AppendLine(parameterName + "." + propertyName + " = await " + manyToManyCacheRepositoryFieldName + ".GetBy" + primaryKeyName + "Async(" + parameterName + "." + primaryKeyName + ");");

                sb.AppendLine("var listOf" + manyToManyEntityName + " = " + parameterName + "." + propertyName + ".Select(ids => new " + manyToManyEntityName + "()");
                sb.AppendLine("{");
                sb.AppendLine(primaryKeyName2 + " = ids,");
                sb.AppendLine(versionKeyName2 + " = await " + entityCacheRepositoryName + ".GetBy" + primaryKeyName2 + "Async(ids)" + versionKeyName2 + ",");
                sb.AppendLine(primaryKeyName + " = " + parameterName + "." + primaryKeyName + ",");
                sb.AppendLine(versionKeyName + " = " + parameterName + "." + versionKeyName + ",");
                sb.AppendLine("}).ToList();");

                sb.AppendLine(manyToManyCacheRepositoryFieldName + ".RemoveBy" + primaryKeyName + "(" + parameterName + "." + propertyName + ");");

                sb.AppendLine("foreach (var mt in listOf" + manyToManyEntityName + ")");
                sb.AppendLine("{");
                sb.AppendLine("mt.Modified = DateTimeOffset.Now;");
                sb.AppendLine("mt.ModifiedBy = menuItem.ModifiedBy;");

                sb.AppendLine("await " + manyToManyCacheRepositoryFieldName + ".InsertAsync(mt);");
                sb.AppendLine("await " + manyToManyVersionRepositoryFieldName + ".InsertAsync(mt);");
                sb.AppendLine("}");
                sb.AppendLine("}");
                */
            }

            return sb.ToString();
        }

        private string GenerateRemoveByPrimaryKey(MethodImplementationInfo method)
        {
            var filter = method.FilterInfo;


            var sb = new StringBuilder();

            // Remove by entity (use primary key(s))
            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var methodParameter = RepositoryInfo.ClassFullName + " " + parameterName;

            // Synchronous method
            sb.AppendLine("public void RemoveBy" + filter.Key + "(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(parameterName + ".IsDeleted = true;");
            sb.AppendLine("UpdateBy" + filter.Key + "(" + parameterName + ");");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task RemoveBy" + filter.Key + "Async(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(parameterName + ".IsDeleted = true;");
            sb.AppendLine("await UpdateBy" + filter.Key + "Async(" + parameterName + ");");
            sb.AppendLine("}");

            // Remove by primary key(s)
            var parameterNames = string.Join(",", filter.Parameters.Select(p => p.TypeName + " " + p.Name.FirstSymbolToLower()));
            var parameters = string.Join(",", filter.Parameters.Select(p => p.Name.FirstSymbolToLower()));
            //var parameter2Name = parameter2.Name.FirstSymbolToLower();
            //var methodParameter2 = parameter2.TypeName + " " + parameter2Name;

            // Synchronous method
            sb.AppendLine("public void RemoveBy" + filter.Key + "(" + parameterNames + ")");
            sb.AppendLine("{");
            sb.AppendLine("var result = " + CacheRepositoryField + ".GetBy" + filter.Key + "(" + parameters + ");");
            sb.AppendLine("result.IsDeleted = true;");
            sb.AppendLine("UpdateBy" + filter.Key + "(result);");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task RemoveBy" + filter.Key + "Async(" + parameterNames + ")");
            sb.AppendLine("{");
            sb.AppendLine("var result = await " + CacheRepositoryField + ".GetBy" + filter.Key + "Async(" + parameters + ");");
            sb.AppendLine("result.IsDeleted = true;");
            sb.AppendLine("await UpdateBy" + filter.Key + "Async(result);");
            sb.AppendLine("}");

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateRemoveByFilterKey(MethodImplementationInfo method)
        {
            var filter = method.FilterInfo;

            var sb = new StringBuilder();

            // Remove by entity (use filter key)
            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var methodParameter = RepositoryInfo.ClassFullName + " " + parameterName;

            // Synchronous method
            sb.AppendLine("public void RemoveBy" + filter.Key + "(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(parameterName + ".IsDeleted = true;");
            sb.AppendLine(CacheRepositoryField + ".UpdateBy" + filter.Key + "(" + parameterName + ");");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task RemoveBy" + filter.Key + "Async(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(parameterName + ".IsDeleted = true;");
            sb.AppendLine("await " + CacheRepositoryField + ".UpdateBy" + filter.Key + "Async(" + parameterName + ");");
            sb.AppendLine("}");

            // Remove by filter key
            var parameter2 = filter.Parameters.First();
            var parameter2Name = parameter2.Name.FirstSymbolToLower();
            var methodParameter2 = parameter2.TypeName + " " + parameter2Name;

            // Synchronous method
            sb.AppendLine("public void RemoveBy" + filter.Key + "(" + methodParameter2 + ")");
            sb.AppendLine("{");
            sb.AppendLine("var result = " + CacheRepositoryField + ".GetBy" + filter.Key + "(" + parameter2Name + ");");
            sb.AppendLine("foreach (var item in result)");
            sb.AppendLine("{");
            sb.AppendLine("item.IsDeleted = true;");
            sb.AppendLine("UpdateBy" + filter.Key + "(item);");
            sb.AppendLine("}");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task RemoveBy" + filter.Key + "Async(" + methodParameter2 + ")");
            sb.AppendLine("{");
            sb.AppendLine("var result = await " + CacheRepositoryField + ".GetBy" + filter.Key + "Async(" + parameter2Name + ");");
            sb.AppendLine("foreach (var item in result)");
            sb.AppendLine("{");
            sb.AppendLine("item.IsDeleted = true;");
            sb.AppendLine("await UpdateBy" + filter.Key + "Async(item);");
            sb.AppendLine("}");
            sb.AppendLine("}");

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private IEnumerable<string> GetUpdateMethodNames()
        {
            var updateMethods = RepositoryInfo.Many2ManyInfo.Select(info => "Update" + info.ManyToManyRepositoryInfo.ClassName);

            return updateMethods;
        }

        #endregion

        #region Heplers 

        private IEnumerable<RepositoryFieldInfo> GetFieldInfos()
        {
            var list = new List<RepositoryFieldInfo>
            {
                new RepositoryFieldInfo(CacheRepositoryType, CacheRepositoryField),
                new RepositoryFieldInfo(VersionRepositoryType, VersionRepositoryField),
            };

            var many2ManyRepositories = RepositoryInfo.Many2ManyInfo
                .SelectMany(i => new List<RepositoryFieldInfo>
                {
                    CashField(i.ManyToManyEntytyType, i.ManyToManyRepositoryInfo.RepositoryNamespace),
                    VersionField(i.ManyToManyEntytyType, i.ManyToManyRepositoryInfo.RepositoryNamespace),
                    CashField(i.EntityType, i.EntityRepositoryInfo.RepositoryNamespace),
                });

            list.AddRange(many2ManyRepositories);

            return list;
        }

        private RepositoryFieldInfo CashField(string entityTypeName, string nameSpace)
        {
            var shortName = entityTypeName.Split('.').Last() + _cacheRepository;
            return new RepositoryFieldInfo(nameSpace + "." + shortName, "_" + shortName.FirstSymbolToLower());
        }

        private RepositoryFieldInfo VersionField(string entityTypeName, string nameSpace)
        {
            var shortName = entityTypeName.Split('.').Last() + _versionRepository;
            return new RepositoryFieldInfo(nameSpace + "." + shortName, "_" + shortName.FirstSymbolToLower());
        }


        #endregion
    }
}