namespace VersionedRepositoryGeneration.Generator.Infrastructure
{
    internal interface ICodeClassGeneratorRepository : ICodeClassGenerator
    {
        string RepositoryName { get; }
        
        /// <summary>
        ///    Errors arising in the analysis of repository models
        /// </summary>
        string RepositoryAnalysisError { get; set; }

    }

    internal interface ICodeClassGenerator
    {
        string GetUsings();
        string GetNamespaceDeclaration();
        string GetClassDeclaration();
        string GetFields();
        string GetProperties();
        string GetConstructors();
        string GetMethods();
        string GetFullCode();
    }
}