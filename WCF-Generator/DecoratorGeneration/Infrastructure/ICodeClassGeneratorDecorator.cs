using WCFGenerator.Common.Infrastructure;

namespace WCFGenerator.DecoratorGeneration.Infrastructure
{
    internal interface ICodeClassDecoratorGenerator : ICodeClassGenerator
    {
        string FileName { get; }

        /// <summary>
        ///    Errors arising in the analysis
        /// </summary>
        string AnalysisError { get; set; }
    }
}