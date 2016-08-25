namespace VersionedRepositoryGeneration.Generator.Core
{
    internal class CodeClassGeneratorCashRepository : CodeClassGeneratorRepository
    {
        #region Overrides of BaseCodeClassGeneratorRepository

        public override string RepositoryName
        {
            get { return RepositoryInfo.ClassName + "Cash" + RepositoryInfo.RepositorySuffix; }
        }

        #endregion
    }
}