namespace VersionedRepositoryGeneration.Generator.Core
{
    internal class CodeClassGeneratorĐacheRepository : CodeClassGeneratorRepository
    {
        #region Overrides of BaseCodeClassGeneratorRepository

        public override string RepositoryName
        {
            get { return RepositoryInfo.ClassName + "Đache" + RepositoryInfo.RepositorySuffix; }
        }

        public override string GetClassDeclaration()
        {
            return "public partial class " + RepositoryName + " : RepositoryBase";
        }

        #endregion
    }
}