using WCFGenerator.Common;
using WCFGenerator.Common.Infrastructure;

namespace WCFGenerator.RepositoriesGeneration.Infrastructure
{
    internal interface ICodeClassGeneratorRepository : ICodeClassGenerator
    {
        string RepositoryName { get; }

        string FileName { get; }

        /// <summary>
        ///    Errors arising in the analysis of repository models
        /// </summary>
        string RepositoryAnalysisError { get; set; }
    }
}