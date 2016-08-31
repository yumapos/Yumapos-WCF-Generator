namespace VersionedRepositoryGeneration.Generator.Core
{
    internal class CodeClassGenerator—acheRepository : CodeClassGeneratorRepository
    {
        #region Overrides of BaseCodeClassGeneratorRepository

        public override string RepositoryName
        {
            get { return RepositoryInfo.ClassName + "—ache" + RepositoryInfo.RepositorySuffix; }
        }

        public override string GetClassDeclaration()
        {
            return "public partial class " + RepositoryName + " : RepositoryBase";
        }

        #endregion
    }
}