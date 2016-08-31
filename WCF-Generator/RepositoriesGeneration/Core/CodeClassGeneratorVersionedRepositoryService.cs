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

        #endregion

        #region Properties

        private string CacheRepositoryType
        {
            get { return RepositoryInfo.ClassName + "ÑacheRepository"; }
        }

        private string CacheRepositoryField
        {
            get { return "_" + CacheRepositoryType.FirstSymbolToLower(); }
        }

        private string VersionRepositoryType
        {
            get { return RepositoryInfo.ClassName + "VersionRepository"; }
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
            var specialParameter = RepositoryInfo.SpecialOptions.Parameters.First();
            var specialMethodParameter = specialParameter.TypeName + "? " + specialParameter.Name.FirstSymbolToLower() + " = " + specialParameter.DefaultValue;

            var sb = new StringBuilder();

            #region Synchronous method

            sb.AppendLine("public IEnumerable<" + RepositoryInfo.ClassFullName + "> GetBy" + method.Key + "(" + methodParameters + ", " + specialMethodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine("return " + CacheRepositoryField + ".GetBy" + method.Key + "(" + methodParameters + ", " + specialMethodParameter + ");");
            sb.AppendLine("}");

            #endregion

            #region Asynchronous method

            sb.AppendLine("public async Task<IEnumerable<" + RepositoryInfo.ClassFullName + ">> GetBy" + method.Key + "Async(" + methodParameters + ", " + specialMethodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine("return await " + CacheRepositoryField + ".GetBy" + method.Key + "Async(" + methodParameters + ", " + specialMethodParameter + ");");
            sb.AppendLine("}");

            #endregion

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateInsert(MethodImplementationInfo method)
        {
            var parameter = method.Parameters.First();
            var methodParameter = parameter.TypeName + " " + parameter.Name.FirstSymbolToLower();

            var sb = new StringBuilder();

            #region Synchronous method

            sb.AppendLine("public void Insert(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(RepositoryInfo.ParameterName + ".Modified = DateTimeOffset.Now;");
            sb.AppendLine(RepositoryInfo.ParameterName + "." + RepositoryInfo.VersionKey + " = " + VersionRepositoryField + ".Insert(" + RepositoryInfo.ParameterName + ");");
            sb.AppendLine(CacheRepositoryField + ".Insert(" + RepositoryInfo.ParameterName + ");");

            // TODO append many to many repository

            sb.AppendLine("}");

            #endregion

            #region Asynchronous method

            sb.AppendLine("public async Task InsertAsync(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(RepositoryInfo.ParameterName + ".Modified = DateTimeOffset.Now;");
            sb.AppendLine(RepositoryInfo.ParameterName + "." + RepositoryInfo.VersionKey + " = " + VersionRepositoryField + ".InsertAsync(" + RepositoryInfo.ParameterName + ");");
            sb.AppendLine("await " + CacheRepositoryField + ".InsertAsync(" + RepositoryInfo.ParameterName + ");");

            // TODO append many to many repository

            sb.AppendLine("}");

            #endregion

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateUpdate(MethodImplementationInfo method)
        {
            var methodParameter = RepositoryInfo.ClassName + " " + RepositoryInfo.ParameterName;

            var sb = new StringBuilder();

            #region Synchronous method

            sb.AppendLine("public void UpdateBy" + method.Key + "(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(RepositoryInfo.ParameterName + ".Modified = DateTimeOffset.Now;");
            sb.AppendLine(RepositoryInfo.ParameterName + "." + RepositoryInfo.VersionKey + " = " + VersionRepositoryField + "Insert(" + RepositoryInfo.ParameterName + ");");
            sb.AppendLine(CacheRepositoryField + ".UpdateBy" + method.Key + "(" + RepositoryInfo.ParameterName + ");");
            sb.AppendLine("}");

            #endregion

            #region Asynchronous method

            sb.AppendLine("public async Task UpdateBy" + method.Key + "Async(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(RepositoryInfo.ParameterName + ".Modified = DateTimeOffset.Now;");
            sb.AppendLine(RepositoryInfo.ParameterName + "." + RepositoryInfo.VersionKey + " = " + VersionRepositoryField + "Insert(" + RepositoryInfo.ParameterName + ");");
            sb.AppendLine(CacheRepositoryField + ".UpdateBy" + method.Key + "Async(" + RepositoryInfo.ParameterName + ");");
            sb.AppendLine("}");

            #endregion

            return method.RequiresImplementation ? sb.ToString() : sb.ToString().SurroundWithComments();
        }

        private string GenerateRemove(MethodImplementationInfo method)
        {
            var sb = new StringBuilder();
            var methodParameter = RepositoryInfo.ClassName + " " + RepositoryInfo.ParameterName;

            #region Synchronous method

            sb.AppendLine("public void RemoveBy" + method.Key + "(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(RepositoryInfo.ParameterName + ".IsDeleted = true;");
            sb.AppendLine(CacheRepositoryField + ".Update"+ method.Key + "(" + RepositoryInfo.ParameterName + ");");
            sb.AppendLine("}");

            #endregion

            #region Asynchronous method

            sb.AppendLine("public async Task RemoveBy" + method.Key + "Async(" + methodParameter + ")");
            sb.AppendLine("{");
            sb.AppendLine(RepositoryInfo.ParameterName + ".IsDeleted = true;");
            sb.AppendLine(CacheRepositoryField + ".Update"+ method.Key + "Async(" + RepositoryInfo.ParameterName + ");");
            sb.AppendLine("}");

            #endregion


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
                    new KeyValuePair<string, string>(i.TableName + "ÑacheRepository", "_" + i.TableName.FirstSymbolToLower()+"ÑacheRepository"),
                    new KeyValuePair<string, string>(i.TableName + "VersionRepository", "_" + i.TableName.FirstSymbolToLower()+"VersionRepository"),
                    new KeyValuePair<string, string>(i.EntityType + "CashRepository", "_" +i.EntityType.FirstSymbolToLower()+"ÑacheRepository")
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