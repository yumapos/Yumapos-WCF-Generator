using WCFGenerator.RepositoriesGeneration.Infrastructure;

namespace WCFGenerator.RepositoriesGeneration.Core
{
    internal class CodeClassGeneratorCacheRepository : CodeClassGeneratorRepository
    {
        public static string RepositoryKind = "Cache";

        #region Overrides of BaseCodeClassGeneratorRepository

        public override string RepositoryName
        {
            get { return RepositoryInfo.ClassName + RepositoryKind + RepositoryInfo.RepositorySuffix; }
        }

        #region Overrides of BaseCodeClassGeneratorRepository

        public override string RepositoryKindName
        {
            get { return RepositoryKind; }
        }

        #endregion

        public override RepositoryType RepositoryType
        {
            get { return RepositoryType.Cache; }
        }

        public override string GetClassDeclaration()
        {
            return "internal partial class " + RepositoryName + " : " + RepositoryInfo.RepositoryBaseTypeName;
        }

        #endregion
    }
}