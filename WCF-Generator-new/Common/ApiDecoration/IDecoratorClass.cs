using Microsoft.CodeAnalysis;
using WCFGenerator.ClientApiDecoratorsGeneration.Configuration;

namespace WCFGenerator.Common.ApiDecoration
{
    public interface IDecoratorClass
    {
        string ClassName { get; }
        string GetFullText(GenerationConfig config);
    }
}
