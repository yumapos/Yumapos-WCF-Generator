using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using WCFGenerator.ClientApiDecoratorsGeneration.Configuration;

namespace WCFGenerator.Common.ApiDecoration
{
    public class DecoratorsGenerator
    {
        protected readonly GeneratorWorkspace GeneratorWorkspace;
        protected readonly GenerationConfig Config;

        protected DecoratorsGenerator(GeneratorWorkspace generatorWorkspace, GenerationConfig config)
        {
            GeneratorWorkspace = generatorWorkspace;
            Config = config;
        }

        protected IDecoratorClass[] Decorators;

        public async Task Generate()
        {
            GeneratorWorkspace.SetTargetProject(Config.TargetProject);
            foreach (var decorator in Decorators)
            {
                var code = decorator.GetFullText(Config);
                GeneratorWorkspace.UpdateFileInTargetProject(decorator.ClassName + ".g.cs", Config.TargetFolder, code);
            }
            await GeneratorWorkspace.ApplyTargetProjectChanges(true);
        }
    }
}
