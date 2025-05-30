using Microsoft.CodeAnalysis;

namespace WCFGenerator.Common.ApiDecoration
{
    public class GenerationConfig
    {
        public string SourceInterface { get; }

        public INamedTypeSymbol PartialClass { get; }

        public INamedTypeSymbol ToDecorate { get; }

        public string TargetNamespace { get; }

        public string TargetProject { get; }

        public string TargetFolder { get; }

        public GenerationConfig(string sourceInterface, INamedTypeSymbol partialClass, INamedTypeSymbol toDecorate, string targetNamespace, string targetProject, string targetFolder)
        {
            SourceInterface = sourceInterface;
            PartialClass = partialClass;
            ToDecorate = toDecorate;
            TargetNamespace = targetNamespace;
            TargetProject = targetProject;
            TargetFolder = targetFolder;
        }
    }
}
