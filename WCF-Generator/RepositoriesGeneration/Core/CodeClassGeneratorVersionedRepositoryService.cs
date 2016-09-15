using System.Collections.Generic;
using System.Linq;
using System.Text;
using VersionedRepositoryGeneration.Generator.Heplers;
using VersionedRepositoryGeneration.Generator.Infrastructure;

namespace VersionedRepositoryGeneration.Generator.Core
{
    internal class CodeClassGeneratorVersionedRepositoryService : BaseCodeClassGeneratorRepository
    {
        #region Fields

        private readonly List<KeyValuePair<string, string>> _fieldsList = new List<KeyValuePair<string, string>>();
        private readonly List<string> _namespaces = new List<string>();

        private readonly string _cacheRepository = CodeClassGeneratorCacheRepository.RepositoryKind + "Repository";
        private readonly string _versionRepository = CodeClassGeneratorVersionsRepository.RepositoryKind + "Repository";

        #endregion

        #region Properties

        private string CacheRepositoryType
        {
            get { return RepositoryInfo.ClassName + _cacheRepository; }
        }

        private string CacheRepositoryField
        {
            get { return "_" + CacheRepositoryType.FirstSymbolToLower(); }
        }

        private string VersionRepositoryType
        {
            get { return RepositoryInfo.ClassName + _versionRepository; }
        }

        private string VersionRepositoryField
        {
            get { return "_" + VersionRepositoryType.FirstSymbolToLower(); }
        }

        private IEnumerable<KeyValuePair<string, string>> AllFields
        {
            get
            {
                if (_fieldsList.Count == 0)
                {
                    _fieldsList.AddRange(GetAllFields());
                }
                return _fieldsList;
            }
        }

        public IEnumerable<string> Namespaces
        {
            get
            {
                if (_namespaces.Count == 0)
                {
                    _namespaces.AddRange(GetNamespases());
                }
                return _namespaces;
            }
        }

        #endregion

        #region Overrides of BaseCodeClassGeneratorRepository

        public override RepositoryType RepositoryType
        {
            get { return RepositoryType.VersionService; }
        }

        public override string GetUsings()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using YumaPos.Server.Infrastructure.Repositories;");
            sb.AppendLine("using YumaPos.Server.Infrastructure.DataObjects;");
            
            // add namespace of repositories which added from relation many to many 
            foreach (var n in Namespaces)
            {
                sb.AppendLine("using " + n + ";");
            }

            sb.AppendLine(base.GetUsings());

            return sb.ToString();
        }

        public override string GetClassDeclaration()
        {
            return "public partial class " + RepositoryName + (RepositoryInfo.RepositoryInterfaceName != null ? " : " + RepositoryInfo.RepositoryInterfaceName : "");
        }

        public override string GetConstructors()
        {
            var sb = new StringBuilder();

            sb.AppendLine("public "+ RepositoryName + "(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService)");
            sb.AppendLine("{");

            foreach (var f in AllFields)
            {
                sb.AppendLine(f.Value + " = new " + f.Key + "(dataAccessService);");
            }
            sb.AppendLine("}");

            return sb.ToString();
        }

        public override string GetFields()
        {
            var sb = new StringBuilder();

            foreach (var f in AllFields)
            {
                sb.AppendLine("private " + f.Key + " " + f.Value + ";");
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
            var updateMethods = GenerateUpdateManyToMany();

            if (!string.IsNullOrEmpty(updateMethods))
                sb.AppendLine(updateMethods);

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
            var sb = new StringBuilder();

            if (RepositoryInfo.IsDeletedExist)
            {
                var specialParameter = RepositoryInfo.SpecialOptions.Parameters.First();
                var specialMethodParameter = specialParameter.TypeName + "? " + specialParameter.Name.FirstSymbolToLower() + " = " + specialParameter.DefaultValue;

                // Synchronous method
                sb.AppendLine("public IEnumerable<" + RepositoryInfo.ClassFullName + "> GetAll(" + specialMethodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine("return " + CacheRepositoryField + ".GetAll(" + specialParameter.Name.FirstSymbolToLower() + ");");
                sb.AppendLine("}");

                // Asynchronous method
                sb.AppendLine("public async Task<IEnumerable<" + RepositoryInfo.ClassFullName + ">> GetAllAsync(" + specialMethodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine("return await " + CacheRepositoryField + ".GetAllAsync(" + specialParameter.Name.FirstSymbolToLower() + ");");
                sb.AppendLine("}");
            }
            else
            {
                // Synchronous method
                sb.AppendLine("public IEnumerable<" + RepositoryInfo.ClassFullName + "> GetAll()");
                sb.AppendLine("{");
                sb.AppendLine("return " + CacheRepositoryField + ".GetAll();");
                sb.AppendLine("}");

                // Asynchronous method
                sb.AppendLine("public async Task<IEnumerable<" + RepositoryInfo.ClassFullName + ">> GetAllAsync()");
                sb.AppendLine("{");
                sb.AppendLine("return await " + CacheRepositoryField + ".GetAllAsync();");
                sb.AppendLine("}");
            }

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateGetByPrimaryKey(MethodImplementationInfo method)
        {
            var filter = method.FilterInfo;
            var sb = new StringBuilder();
            var methodParameters = string.Join(", ", filter.Parameters.Select(k => k.TypeName + " " + k.Name.FirstSymbolToLower()));
            var methodParameterNames = string.Join(", ", filter.Parameters.Select(k => k.Name.FirstSymbolToLower()));

            if (RepositoryInfo.IsDeletedExist)
            {
                var specialParameter = RepositoryInfo.SpecialOptions.Parameters.First();
                var specialMethodParameter = specialParameter.TypeName + "? " + specialParameter.Name.FirstSymbolToLower() + " = " + specialParameter.DefaultValue;
                var specialMethodParameterName = specialParameter.Name.FirstSymbolToLower();
                
                // Synchronous method
                sb.AppendLine("public " + RepositoryInfo.ClassFullName + " GetBy" + filter.Key + "(" + methodParameters + ", " + specialMethodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine("return " + CacheRepositoryField + ".GetBy" + filter.Key + "(" + methodParameterNames + ", " + specialMethodParameterName + ");");
                sb.AppendLine("}");

                // Asynchronous method
                sb.AppendLine("public async Task<" + RepositoryInfo.ClassFullName + "> GetBy" + filter.Key + "Async(" + methodParameters + ", " + specialMethodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine("return await " + CacheRepositoryField + ".GetBy" + filter.Key + "Async(" + methodParameterNames + ", " + specialMethodParameterName + ");");
                sb.AppendLine("}");
            }
            else
            {
                // Synchronous method
                sb.AppendLine("public " + RepositoryInfo.ClassFullName + " GetBy" + filter.Key + "(" + methodParameters + ")");
                sb.AppendLine("{");
                sb.AppendLine("return " + CacheRepositoryField + ".GetBy" + filter.Key + "(" + methodParameterNames + ");");
                sb.AppendLine("}");

                // Asynchronous method
                sb.AppendLine("public async Task<" + RepositoryInfo.ClassFullName + "> GetBy" + filter.Key + "Async(" + methodParameters + ")");
                sb.AppendLine("{");
                sb.AppendLine("return await " + CacheRepositoryField + ".GetBy" + filter.Key + "Async(" + methodParameterNames + ");");
                sb.AppendLine("}");
            }

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateGetByFilterKey(MethodImplementationInfo method)
        {
            var filter = method.FilterInfo;
            var sb = new StringBuilder();
            var methodParameters = string.Join(", ", filter.Parameters.Select(k => k.TypeName + " " + k.Name.FirstSymbolToLower()));
            var methodParameterNames = string.Join(", ", filter.Parameters.Select(k => k.Name.FirstSymbolToLower()));

            if (RepositoryInfo.IsDeletedExist)
            {
                var specialParameter = RepositoryInfo.SpecialOptions.Parameters.First();
                var specialMethodParameter = specialParameter.TypeName + "? " + specialParameter.Name.FirstSymbolToLower() + " = " + specialParameter.DefaultValue;
                var specialMethodParameterName = specialParameter.Name.FirstSymbolToLower();

                // Synchronous method
                sb.AppendLine("public IEnumerable<" + RepositoryInfo.ClassFullName + "> GetBy" + filter.Key + "(" + methodParameters + ", " + specialMethodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine("return " + CacheRepositoryField + ".GetBy" + filter.Key + "(" + methodParameterNames + ", " + specialMethodParameterName + ");");
                sb.AppendLine("}");

                // Asynchronous method
                sb.AppendLine("public async Task<IEnumerable<" + RepositoryInfo.ClassFullName + ">> GetBy" + filter.Key + "Async(" + methodParameters + ", " + specialMethodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine("return await " + CacheRepositoryField + ".GetBy" + filter.Key + "Async(" + methodParameterNames + ", " + specialMethodParameterName + ");");
                sb.AppendLine("}");
            }
            else
            {
                // Synchronous method
                sb.AppendLine("public IEnumerable<" + RepositoryInfo.ClassFullName + "> GetBy" + filter.Key + "(" + methodParameters + ")");
                sb.AppendLine("{");
                sb.AppendLine("return " + CacheRepositoryField + ".GetBy" + filter.Key + "(" + methodParameterNames + ");");
                sb.AppendLine("}");

                // Asynchronous method
                sb.AppendLine("public async Task<IEnumerable<" + RepositoryInfo.ClassFullName + ">> GetBy" + filter.Key + "Async(" + methodParameters + ")");
                sb.AppendLine("{");
                sb.AppendLine("return await " + CacheRepositoryField + ".GetBy" + filter.Key + "Async(" + methodParameterNames + ");");
                sb.AppendLine("}");
            }

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateGetByVersionKey(MethodImplementationInfo method)
        {
            var filter = method.FilterInfo;
            var sb = new StringBuilder();
            var methodParameters = string.Join(", ", filter.Parameters.Select(k => k.TypeName + " " + k.Name.FirstSymbolToLower()));
            var methodParameterNames = string.Join(", ", filter.Parameters.Select(k => k.Name.FirstSymbolToLower()));

            if (RepositoryInfo.IsDeletedExist)
            {
                var specialParameter = RepositoryInfo.SpecialOptions.Parameters.First();
                var specialMethodParameter = specialParameter.TypeName + "? " + specialParameter.Name.FirstSymbolToLower() + " = " + specialParameter.DefaultValue;
                var specialMethodParameterName = specialParameter.Name.FirstSymbolToLower();

                // Synchronous method
                sb.AppendLine("public " + RepositoryInfo.ClassFullName + " GetBy" + filter.Key + "(" + methodParameters + ", " + specialMethodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine("return " + VersionRepositoryField + ".GetBy" + filter.Key + "(" + methodParameterNames + ", " + specialMethodParameterName + ");");
                sb.AppendLine("}");

                // Asynchronous method
                sb.AppendLine("public async Task<" + RepositoryInfo.ClassFullName + "> GetBy" + filter.Key + "Async(" + methodParameters + ", " + specialMethodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine("return await " + VersionRepositoryField + ".GetBy" + filter.Key + "Async(" + methodParameterNames + ", " + specialMethodParameterName + ");");
                sb.AppendLine("}");
            }
            else
            {
                // Synchronous method
                sb.AppendLine("public " + RepositoryInfo.ClassFullName + " GetBy" + filter.Key + "(" + methodParameters + ")");
                sb.AppendLine("{");
                sb.AppendLine("return " + VersionRepositoryField + ".GetBy" + filter.Key + "(" + methodParameterNames + ");");
                sb.AppendLine("}");

                // Asynchronous method
                sb.AppendLine("public async Task<" + RepositoryInfo.ClassFullName + "> GetBy" + filter.Key + "Async(" + methodParameters + ")");
                sb.AppendLine("{");
                sb.AppendLine("return await " + VersionRepositoryField + ".GetBy" + filter.Key + "Async(" + methodParameterNames + ");");
                sb.AppendLine("}");
            }

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateInsert(MethodImplementationInfo method)
        {
            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var methodParameter = RepositoryInfo.ClassFullName + " " + parameterName;
            var updateMethods = RepositoryInfo.Many2ManyInfo.Select(info => "Update" + info.ManyToManyRepositoryInfo.ClassName + "(" + parameterName + ");").ToList();

            var sb = new StringBuilder();

            // If should not return identifier
            if (RepositoryInfo.PrimaryKeys.Count != 1)
            {
                // synchronous method
                sb.AppendLine("public void Insert(" + methodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine(parameterName + ".Modified = DateTimeOffset.Now;");
                sb.AppendLine(parameterName + "." + RepositoryInfo.VersionKeyName + " = Guid.NewGuid();");
                sb.AppendLine(VersionRepositoryField + ".Insert(" + parameterName + ");");
                sb.AppendLine(CacheRepositoryField + ".Insert(" + parameterName + ");");

                foreach (var m in updateMethods)
                {
                    sb.AppendLine(m);
                }

                sb.AppendLine("}");

                // Asynchronous method
                sb.AppendLine("public async Task InsertAsync(" + methodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine(parameterName + ".Modified = DateTimeOffset.Now;");
                sb.AppendLine(parameterName + "." + RepositoryInfo.VersionKeyName + " = Guid.NewGuid();");
                sb.AppendLine("await " + VersionRepositoryField + ".InsertAsync(" + parameterName + ");");
                sb.AppendLine("await " + CacheRepositoryField + ".InsertAsync(" + parameterName + ");");

                // if async
                // var updateMethodsAsync = RepositoryInfo.Many2ManyInfo.Select(info => "Update" + info.ManyToManyRepositoryInfo.ClassName + "Async(" + parameterName + ");");

                foreach (var m in updateMethods)
                {
                    sb.AppendLine(m);
                }

                sb.AppendLine("}");
            }

            else
            {
                var returnType = RepositoryInfo.PrimaryKeys.First().TypeName;

                // synchronous method
                sb.AppendLine("public " + returnType + " Insert(" + methodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine(parameterName + ".Modified = DateTimeOffset.Now;");
                sb.AppendLine(parameterName + "." + RepositoryInfo.VersionKeyName + " = Guid.NewGuid();");
                sb.AppendLine(VersionRepositoryField + ".Insert(" + parameterName + ");");
                sb.AppendLine("var res = " + CacheRepositoryField + ".Insert(" + parameterName + ");");

                foreach (var m in updateMethods)
                {
                    sb.AppendLine(m);
                }

                sb.AppendLine("return (" + returnType + ")res;");

                sb.AppendLine("}");

                // Asynchronous method
                sb.AppendLine("public async Task<" + returnType + "> InsertAsync(" + methodParameter + ")");
                sb.AppendLine("{");
                sb.AppendLine(parameterName + ".Modified = DateTimeOffset.Now;");
                sb.AppendLine(parameterName + "." + RepositoryInfo.VersionKeyName + " = Guid.NewGuid();");
                sb.AppendLine("await " + VersionRepositoryField + ".InsertAsync(" + parameterName + ");");
                sb.AppendLine("var res = await " + CacheRepositoryField + ".InsertAsync(" + parameterName + ");");

                // if async
                // var updateMethodsAsync = RepositoryInfo.Many2ManyInfo.Select(info => "Update" + info.ManyToManyRepositoryInfo.ClassName + "Async(" + parameterName + ");");

                foreach (var m in updateMethods)
                {
                    sb.AppendLine(m);
                }

                sb.AppendLine("return (" + returnType + ")res;");

                sb.AppendLine("}");
            }


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
            sb.AppendLine(parameterName + ".Modified = DateTimeOffset.Now;");
            sb.AppendLine(parameterName + "." + RepositoryInfo.VersionKeyName + " = Guid.NewGuid();");
            sb.AppendLine(VersionRepositoryField + ".Insert(" + parameterName + ");");
            sb.AppendLine(CacheRepositoryField + ".UpdateBy" + filter.Key + "(" + parameterName + ");");

            var updateMethods = RepositoryInfo.Many2ManyInfo.Select(info=> "Update" + info.ManyToManyRepositoryInfo.ClassName + "(" + parameterName + ");").ToList();

            foreach (var m in updateMethods)
            {
                sb.AppendLine(m);
            }

            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task UpdateBy" + filter.Key + "Async(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(parameterName + ".Modified = DateTimeOffset.Now;");
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
                var manyToManyCacheRepositoryName = info.ManyToManyEntytyType + _cacheRepository;
                var manyToManyVersionRepositoryName = info.ManyToManyEntytyType + _versionRepository;
                var manyToManyCacheRepositoryFieldName = "_" + manyToManyCacheRepositoryName.FirstSymbolToLower();
                var manyToManyVersionRepositoryFieldName = "_" + manyToManyVersionRepositoryName.FirstSymbolToLower();

                var entityCacheRepositoryName = "_" + info.EntityRepositoryInfo.ClassName.FirstSymbolToLower() + _cacheRepository;

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

                sb.AppendLine("var listOf" + manyToManyEntityName + " = " + parameterName + "." + propertyName + ".Select(ids => new " + manyToManyEntityName + "()");
                sb.AppendLine("{");
                sb.AppendLine(primaryKeyName2 + " = ids,");
                sb.AppendLine(versionKeyName2 + " = " + entityCacheRepositoryName + ".GetBy" + primaryKeyName2 + "(ids)." + versionKeyName2 + ",");
                sb.AppendLine(primaryKeyName + " = " + parameterName + "." + primaryKeyName + ",");
                sb.AppendLine(versionKeyName + " = " + parameterName + "." + versionKeyName + ",");
                sb.AppendLine("}).ToList();");

                sb.AppendLine(manyToManyCacheRepositoryFieldName + ".RemoveBy" + primaryKeyName + "(" + parameterName + "." + primaryKeyName + ");");

                sb.AppendLine("foreach (var mt in listOf" + manyToManyEntityName + ")");
                sb.AppendLine("{");
                sb.AppendLine("mt.Modified = DateTimeOffset.Now;");
                sb.AppendLine("mt.ModifiedBy = "+ parameterName + ".ModifiedBy;");
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

            // Remove by entity (use filter key)
            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var methodParameter = RepositoryInfo.ClassFullName + " " + parameterName;
            
            // Synchronous method
            sb.AppendLine("public void RemoveBy" + filter.Key + "(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(parameterName + ".IsDeleted = true;");
            sb.AppendLine(CacheRepositoryField + ".UpdateBy"+ filter.Key + "(" + parameterName + ");");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task RemoveBy" + filter.Key + "Async(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(parameterName + ".IsDeleted = true;");
            sb.AppendLine("await " + CacheRepositoryField + ".UpdateBy"+ filter.Key + "Async(" + parameterName + ");");
            sb.AppendLine("}");

            // Remove by filter key
            var parameter2 = filter.Parameters.First();
            var parameter2Name = parameter2.Name.FirstSymbolToLower();
            var methodParameter2 = parameter2.TypeName + " " + parameter2Name;

            // Synchronous method
            sb.AppendLine("public void RemoveBy" + filter.Key + "(" + methodParameter2 + ")");
            sb.AppendLine("{");
            sb.AppendLine("var result = "+ CacheRepositoryField + ".GetBy"+filter.Key+"("+parameter2Name+");");
            sb.AppendLine("result.IsDeleted = true;");
            sb.AppendLine("UpdateBy"+ filter.Key + "(result);");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task RemoveBy" + filter.Key + "Async(" + methodParameter2 + ")");
            sb.AppendLine("{");
            sb.AppendLine("var result = await " + CacheRepositoryField + ".GetBy" + filter.Key + "Async(" + parameter2Name + ");");
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

        #endregion

        #region Heplers 

        private IEnumerable<KeyValuePair<string, string>> GetAllFields()
        {
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(CacheRepositoryType, CacheRepositoryField),
                new KeyValuePair<string, string>(VersionRepositoryType, VersionRepositoryField)
            };

            var many2ManyRepositories = RepositoryInfo.Many2ManyInfo
                .SelectMany(i => new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(i.ManyToManyEntytyType + _cacheRepository, "_" + i.ManyToManyEntytyType.FirstSymbolToLower() + _cacheRepository),
                    new KeyValuePair<string, string>(i.ManyToManyEntytyType + _versionRepository, "_" + i.ManyToManyEntytyType.FirstSymbolToLower() + _versionRepository),
                    new KeyValuePair<string, string>(i.EntityType + _cacheRepository, "_" + i.EntityType.FirstSymbolToLower() + _cacheRepository)
                });

            list.AddRange(many2ManyRepositories);
            return list;
        }

        private IEnumerable<string> GetNamespases()
        {
            // add namespace of repositories which added from relation many to many 
            return RepositoryInfo.Many2ManyInfo.SelectMany(i => i.RepositoryNamespaces);
        }


        #endregion
    }
}