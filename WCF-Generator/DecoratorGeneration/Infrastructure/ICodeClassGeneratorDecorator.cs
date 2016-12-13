using WCFGenerator.Common.Infrastructure;

namespace WCFGenerator.DecoratorGeneration.Infrastructure
{
    internal interface ICodeClassGeneratorDecorator : ICodeClassGenerator
    {
        string FileName { get; }

        /// <summary>
        ///    Errors arising in the analysis
        /// </summary>
        string AnalysisError { get; set; }
    }
}