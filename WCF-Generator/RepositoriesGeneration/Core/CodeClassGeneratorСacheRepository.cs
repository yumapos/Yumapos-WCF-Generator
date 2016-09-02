namespace VersionedRepositoryGeneration.Generator.Core
{
    internal class CodeClassGeneratorCacheRepository : CodeClassGeneratorRepository
    {
        public static string RepositoryKind = "Cache";

        #region Overrides of BaseCodeClassGeneratorRepository

        public override string RepositoryName
        {
            get { return RepositoryInfo.ClassName + RepositoryKind + RepositoryInfo.RepositorySuffix; }
        }

        public override string GetClassDeclaration()
        {
            return "public partial class " + RepositoryName + " : RepositoryBase";
        }

        #endregion
    }
}