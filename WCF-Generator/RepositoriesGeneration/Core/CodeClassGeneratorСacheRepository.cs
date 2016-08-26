namespace VersionedRepositoryGeneration.Generator.Core
{
    internal class CodeClassGenerator—acheRepository : CodeClassGeneratorRepository
    {
        #region Overrides of BaseCodeClassGeneratorRepository

        public override string RepositoryName
        {
            get { return RepositoryInfo.ClassName + "—ache" + RepositoryInfo.RepositorySuffix; }
        }

        #endregion
    }
}