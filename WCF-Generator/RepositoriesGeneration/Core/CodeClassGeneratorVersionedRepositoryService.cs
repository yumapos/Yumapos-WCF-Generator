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

        private readonly string _cacheRepository = CodeClassGenerator—acheRepository.RepositoryKind + "Repository";
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
            return "public partial class " + RepositoryName + " : RepositoryBase" + (RepositoryInfo.RepositoryInterfaceName != null ? "," + RepositoryInfo.RepositoryInterfaceName : "");
        }

        public override string GetConstructors()
        {
            var sb = new StringBuilder();

            sb.AppendLine("public "+ RepositoryName + "(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService)");
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
                .Aggregate("", (s, method) => GenerateGetAll(method));

            if (!string.IsNullOrEmpty(getAllMethods))
                sb.AppendLine(getAllMethods);

            // RepositoryMethod.GetBy
            var getByMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.GetBy)
                .Aggregate("", (s, method) => GenerateGetBy(method));

            if (!string.IsNullOrEmpty(getByMethods))
                sb.AppendLine(getByMethods);

            // RepositoryMethod.Insert
            var insertMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.Insert)
                .Aggregate("", (s, method) => GenerateInsert(method));

            if (!string.IsNullOrEmpty(insertMethods))
                sb.AppendLine(insertMethods);

            // RepositoryMethod.UpdateBy
            var updateByMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.UpdateBy)
                .Aggregate("", (s, method) => GenerateUpdate(method));

            if (!string.IsNullOrEmpty(updateByMethods))
                sb.AppendLine(updateByMethods);

            // Update many to many
            var updateMethods = GenerateUpdateManyToMany();

            if (!string.IsNullOrEmpty(updateMethods))
                sb.AppendLine(updateMethods);

            // RepositoryMethod.RemoveBy
            var removeByMethods = RepositoryInfo.MethodImplementationInfo
                .Where(m => m.Method == RepositoryMethod.RemoveBy)
                .Aggregate("", (s, method) => GenerateRemove(method));

            if (!string.IsNullOrEmpty(removeByMethods))
                sb.AppendLine(removeByMethods);

            return sb.ToString();
        }

        #endregion

        #region Private

        private string GenerateGetAll(MethodImplementationInfo method)
        {
            var specialParameter = RepositoryInfo.SpecialOptions.Parameters.First();
            var specialMethodParameter = specialParameter.TypeName + "? " + specialParameter.Name.FirstSymbolToLower() + " = " + specialParameter.DefaultValue;

            var sb = new StringBuilder();

            #region Synchronous method

            sb.AppendLine("public IEnumerable<" + RepositoryInfo.ClassFullName + "> GetAll(" + specialMethodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine("return " + CacheRepositoryField + ".GetAll(" + specialParameter.Name.FirstSymbolToLower() + ");");
            sb.AppendLine("}");

            #endregion

            #region Asynchronous method

            sb.AppendLine("public async Task<IEnumerable<" + RepositoryInfo.ClassFullName + ">> GetAllAsync(" + specialMethodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine("return await " + CacheRepositoryField + ".GetAllAsync("+ specialParameter.Name.FirstSymbolToLower() + ");");
            sb.AppendLine("}");

            #endregion

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateGetBy(MethodImplementationInfo method)
        {
            var methodParameters = string.Join(", ", method.Parameters.Select(k => k.TypeName + " " + k.Name.FirstSymbolToLower()));
            var methodParameterNames = string.Join(", ", method.Parameters.Select(k => k.Name.FirstSymbolToLower()));
            var specialParameter = RepositoryInfo.SpecialOptions.Parameters.First();
            var specialMethodParameter = specialParameter.TypeName + "? " + specialParameter.Name.FirstSymbolToLower() + " = " + specialParameter.DefaultValue;
            var specialMethodParameterName = specialParameter.Name.FirstSymbolToLower();

            var sb = new StringBuilder();

            #region Synchronous method

            sb.AppendLine("public IEnumerable<" + RepositoryInfo.ClassFullName + "> GetBy" + method.Key + "(" + methodParameters + ", " + specialMethodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine("return " + CacheRepositoryField + ".GetBy" + method.Key + "(" + methodParameterNames + ", " + specialMethodParameterName + ");");
            sb.AppendLine("}");

            #endregion

            #region Asynchronous method

            sb.AppendLine("public async Task<IEnumerable<" + RepositoryInfo.ClassFullName + ">> GetBy" + method.Key + "Async(" + methodParameters + ", " + specialMethodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine("return await " + CacheRepositoryField + ".GetBy" + method.Key + "Async(" + methodParameterNames + ", " + specialMethodParameterName + ");");
            sb.AppendLine("}");

            #endregion

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateInsert(MethodImplementationInfo method)
        {
            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var methodParameter = RepositoryInfo.ClassFullName + " " + parameterName;

            var sb = new StringBuilder();

            #region Synchronous method

            sb.AppendLine("public void Insert(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(parameterName + ".Modified = DateTimeOffset.Now;");
            sb.AppendLine(parameterName + "." + RepositoryInfo.VersionKey + " = " + VersionRepositoryField + ".Insert(" + parameterName + ");");
            sb.AppendLine(CacheRepositoryField + ".Insert(" + parameterName + ");");

            var updateMethods = RepositoryInfo.Many2ManyInfo.Select(info => "Update" + info.ManyToManyRepositoryInfo.ClassName + "(" + parameterName + ");").ToList();

            foreach (var m in updateMethods)
            {
                sb.AppendLine(m);
            }

            sb.AppendLine("}");

            #endregion

            #region Asynchronous method

            sb.AppendLine("public async Task InsertAsync(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(parameterName + ".Modified = DateTimeOffset.Now;");
            sb.AppendLine(parameterName + "." + RepositoryInfo.VersionKey + " = await " + VersionRepositoryField + ".InsertAsync(" + parameterName + ");");
            sb.AppendLine("await " + CacheRepositoryField + ".InsertAsync(" + parameterName + ");");

            // if async
            // var updateMethodsAsync = RepositoryInfo.Many2ManyInfo.Select(info => "Update" + info.ManyToManyRepositoryInfo.ClassName + "Async(" + parameterName + ");");

            foreach (var m in updateMethods)
            {
                sb.AppendLine(m);
            }

            sb.AppendLine("}");

            #endregion

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateUpdate(MethodImplementationInfo method)
        {
            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var methodParameter = RepositoryInfo.ClassFullName + " " + parameterName;

            var sb = new StringBuilder();

            // Synchronous method
            sb.AppendLine("public void UpdateBy" + method.Key + "(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(parameterName + ".Modified = DateTimeOffset.Now;");
            sb.AppendLine(parameterName + "." + RepositoryInfo.VersionKey + " = " + VersionRepositoryField + ".Insert(" + parameterName + ");");
            sb.AppendLine(CacheRepositoryField + ".UpdateBy" + method.Key + "(" + parameterName + ");");

            var updateMethods = RepositoryInfo.Many2ManyInfo.Select(info=> "Update" + info.ManyToManyRepositoryInfo.ClassName + "(" + parameterName + ");").ToList();

            foreach (var m in updateMethods)
            {
                sb.AppendLine(m);
            }

            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task UpdateBy" + method.Key + "Async(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(parameterName + ".Modified = DateTimeOffset.Now;");
            sb.AppendLine(parameterName + "." + RepositoryInfo.VersionKey + " = await " + VersionRepositoryField + ".InsertAsync(" + parameterName + ");");
            sb.AppendLine("await " + CacheRepositoryField + ".UpdateBy" + method.Key + "Async(" + parameterName + ");");

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
                var versionKeyName = RepositoryInfo.VersionKey;
                var primaryKeyName2 = info.EntityRepositoryInfo.PrimaryKeyName;
                var versionKeyName2 = info.EntityRepositoryInfo.VersionKey;
                var propertyName = info.PropertyName;

                // sync method
                sb.AppendLine("private void Update" + manyToManyEntityName + "(" + methodParameter + ")");
                sb.AppendLine("{");

                sb.AppendLine("if (" + parameterName + "." + propertyName + " == null)");
                sb.AppendLine(parameterName + "." + propertyName + " = " + manyToManyCacheRepositoryFieldName + ".Get" + propertyName + "By" + primaryKeyName + "(" + parameterName + "." + primaryKeyName + ");");

                sb.AppendLine("var listOf" + manyToManyEntityName + " = " + parameterName + "." + propertyName + ".Select(ids => new " + manyToManyEntityName + "()");
                sb.AppendLine("{");
                sb.AppendLine(primaryKeyName2 + " = ids,");
                sb.AppendLine(versionKeyName2 + " = " + entityCacheRepositoryName + ".GetBy" + primaryKeyName2 + "(ids)." + versionKeyName2 + ",");
                sb.AppendLine(primaryKeyName + " = " + parameterName + "." + primaryKeyName + ",");
                sb.AppendLine(versionKeyName + " = " + parameterName + "." + versionKeyName + ",");
                sb.AppendLine("}).ToList();");

                sb.AppendLine(entityCacheRepositoryName + ".RemoveBy" + primaryKeyName + "(" + parameterName + "." + propertyName + ");");

                sb.AppendLine("foreach (var mt in listOf" + manyToManyEntityName + ")");
                sb.AppendLine("{");
                sb.AppendLine("mt.Modified = DateTimeOffset.Now;");
                sb.AppendLine("mt.ModifiedBy = menuItem.ModifiedBy;");
                sb.AppendLine(manyToManyCacheRepositoryFieldName + ".Insert(mt);");
                sb.AppendLine(manyToManyVersionRepositoryFieldName + ".Insert(mt);");
                sb.AppendLine("}");
                sb.AppendLine("}");

                /*  Async method
                sb.AppendLine();

                sb.AppendLine("private Task Update" + manyToManyEntityName + "Async(" + methodParameter ")");
                sb.AppendLine("{");
                
                sb.AppendLine("if (" + parameterName + "." + propertyName + " == null)");
                sb.AppendLine(parameterName + "." + propertyName + " = await " + manyToManyCacheRepositoryFieldName + ".Get" + primaryKeyName2 + "By" + primaryKeyName + "Async(" + parameterName + "." + primaryKeyName + ");");

                sb.AppendLine("var listOf" + manyToManyEntityName + " = " + parameterName + "." + propertyName + ".Select(ids => new " + manyToManyEntityName + "()");
                sb.AppendLine("{");
                sb.AppendLine(primaryKeyName2 + " = ids,");
                sb.AppendLine(versionKeyName2 + " = await " + entityCacheRepositoryName + ".GetBy" + primaryKeyName2 + "Async(ids)." + versionKeyName2 + ",");
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


        private string GenerateRemove(MethodImplementationInfo method)
        {
            var sb = new StringBuilder();

            // Remove by entity (use filter key)
            var parameterName = RepositoryInfo.ClassName.FirstSymbolToLower();
            var methodParameter = RepositoryInfo.ClassFullName + " " + parameterName;
            
            // Synchronous method
            sb.AppendLine("public void RemoveBy" + method.Key + "(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(parameterName + ".IsDeleted = true;");
            sb.AppendLine(CacheRepositoryField + ".UpdateBy"+ method.Key + "(" + parameterName + ");");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task RemoveBy" + method.Key + "Async(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(parameterName + ".IsDeleted = true;");
            sb.AppendLine("await " + CacheRepositoryField + ".UpdateBy"+ method.Key + "Async(" + parameterName + ");");
            sb.AppendLine("}");

            // Remove by filter key
            var parameter2 = method.Parameters.First();
            var parameter2Name = parameter2.Name.FirstSymbolToLower();
            var methodParameter2 = parameter2.TypeName + " " + parameter2Name;

            // Synchronous method
            sb.AppendLine("public void RemoveBy" + method.Key + "(" + methodParameter2 + ")");
            sb.AppendLine("{");
            sb.AppendLine("var result = "+ CacheRepositoryField + ".GetBy"+method.Key+"("+parameter2Name+");");
            sb.AppendLine("foreach (var item in result)");
            sb.AppendLine("{");
            sb.AppendLine("item.IsDeleted = true;");
            sb.AppendLine("UpdateBy"+ method.Key + "(item);");
            sb.AppendLine("}");
            sb.AppendLine("}");

            // Asynchronous method
            sb.AppendLine("public async Task RemoveBy" + method.Key + "Async(" + methodParameter2 + ")");
            sb.AppendLine("{");
            sb.AppendLine("var result = await " + CacheRepositoryField + ".GetBy" + method.Key + "Async(" + parameter2Name + ");");
            sb.AppendLine("foreach (var item in result)");
            sb.AppendLine("{");
            sb.AppendLine("item.IsDeleted = true;");
            sb.AppendLine("await UpdateBy" + method.Key + "Async(item);");
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