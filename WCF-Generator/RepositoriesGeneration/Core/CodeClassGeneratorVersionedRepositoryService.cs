using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using VersionedRepositoryGeneration.Generator.Heplers;
using VersionedRepositoryGeneration.Generator.Infrastructure;

namespace VersionedRepositoryGeneration.Generator.Core
{
    internal class CodeClassGeneratorVersionedRepositoryService : BaseCodeClassGeneratorRepository
    {
        private readonly List<KeyValuePair<string, string>> _fieldsList = new List<KeyValuePair<string, string>>();

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

        private List<KeyValuePair<string, string>> AllFields
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

        #endregion

        #region Overrides of BaseCodeClassGeneratorRepository

        public override string GetUsings()
        {
            var compilation = CSharpCompilation.Create("compilation").AddSyntaxTrees(RepositoryInfo.RepositoryInterface.SyntaxTree);
            var model = compilation.GetSemanticModel(RepositoryInfo.RepositoryInterface.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(RepositoryInfo.RepositoryInterface);
            var fullName = symbol.ToString();
            var nameSpace = fullName.Replace("." + RepositoryInfo.RepositoryInterface.Identifier.Text, "");

            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using YumaPos.Server.Data.Sql;");
            sb.AppendLine("using " + nameSpace + ";");

            return sb.ToString();
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

            foreach (var methodInfo in RepositoryInfo.MethodImplementationInfo)
            {
                var commented = !methodInfo.RequiresImplementation;
                var codeText = "";

                switch (methodInfo.Method)
                {
                    case RepositoryMethod.GetAll:
                        codeText = GenerateGetAll();
                        break;
                    case RepositoryMethod.Insert:
                        codeText = GenerateInsert();
                        break;
                    case RepositoryMethod.GetBy:
                        codeText = GenerateGetBy();
                        break;
                    case RepositoryMethod.UpdateBy:
                        codeText = GenerateUpdate();
                        break;
                    case RepositoryMethod.RemoveBy:
                        codeText = GenerateRemove();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                codeText = commented ? codeText.SurroundWithComments() : codeText;
                sb.AppendLine(codeText);
            }

            return sb.ToString();
        }

        #region Generation

        private string GenerateGetAll()
        {
            var sb = new StringBuilder();
            sb.AppendLine("public IEnumerable<" + RepositoryInfo.ClassFullName + "> GetAll(bool? isDeleted = false)");
            sb.AppendLine("{");
            sb.AppendLine("return " + CacheRepositoryField + ".GetAll();");
            sb.AppendLine("}");
            return sb.ToString();
        }
    
        private string GenerateInsert()
        {
            var sb = new StringBuilder();

            sb.AppendLine("public Guid Insert(" + RepositoryInfo.ClassFullName + " " + RepositoryInfo.ParameterName + ")");
            sb.AppendLine("{");
            sb.AppendLine(RepositoryInfo.ParameterName + ".Modified = DateTimeOffset.Now;");
            sb.AppendLine(RepositoryInfo.ParameterName + ".ItemVersionId = " + VersionRepositoryField + ".Insert(" + RepositoryInfo.ParameterName + ");");
            sb.AppendLine(CacheRepositoryField + ".Insert(" + RepositoryInfo.ParameterName + ");");

            // TODO append many to many repository

            sb.AppendLine("return " + RepositoryInfo.ParameterName + ".ItemVersionId;");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateGetBy()
        {
            var sb = new StringBuilder();

         //   sb.AppendLine("public " + RepositoryInfo.ClassFullName + " GetBy(System.Guid " + RepositoryInfo.Keys[0] + ", bool? isDeleted = false)");
         //   sb.AppendLine("{");
          //  sb.AppendLine("return " + CacheRepositoryField + " GetBy(System.Guid " + RepositoryInfo.Keys[0] + ");");
          //  sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateUpdate()
        {
            var sb = new StringBuilder();

            sb.AppendLine("public Guid Update(" + RepositoryInfo.ClassFullName + " " + RepositoryInfo.ParameterName + ";");
            sb.AppendLine("{");
            sb.AppendLine(RepositoryInfo.ParameterName + ".Modified = DateTimeOffset.Now;");
            sb.AppendLine(RepositoryInfo.ParameterName + ".ItemVersionId = " + VersionRepositoryField + "Insert(" + RepositoryInfo.ParameterName + ");");
            sb.AppendLine(CacheRepositoryField + ".Update(" + RepositoryInfo.ParameterName + ");"); 

            // TODO append many to many repository

            sb.AppendLine("return " + RepositoryInfo.ParameterName + ".ItemVersionId;");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRemove()
        {
            var sb = new StringBuilder();

            sb.AppendLine("public Guid Remove(" + RepositoryInfo.ClassFullName + " " + RepositoryInfo.ParameterName + ";");
            sb.AppendLine("{");
            sb.AppendLine(RepositoryInfo.ParameterName + ".IsDeleted = true;");
            sb.AppendLine(CacheRepositoryField + ".Update(" + RepositoryInfo.ParameterName + ");");
            
            return sb.ToString();
        }

        #endregion


        #endregion


        #region Private 

        private List<KeyValuePair<string, string>> GetAllFields()
        {
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(CacheRepositoryType, CacheRepositoryField),
                new KeyValuePair<string, string>(VersionRepositoryType, VersionRepositoryField)
                // TODO append many to many repository
            };

            return list;
        }

        #endregion
    }
}