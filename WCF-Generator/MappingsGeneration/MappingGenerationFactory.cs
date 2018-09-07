using System.Threading.Tasks;
using WCFGenerator.Common;
using WCFGenerator.MappingsGeneration.Configuration;

namespace WCFGenerator.MappingsGeneration
{
    public class MappingGenerationFactory
    {
        private readonly GeneratorWorkspace _generatorWorkspace;
        private readonly MappingConfiguration[] _configs;

        public MappingGenerationFactory(GeneratorWorkspace generatorWorkspace, MappingConfiguration[] configs)
        {
            _generatorWorkspace = generatorWorkspace;
            _configs = configs;
        }

        public async Task GenerateAll()
        {
            foreach (var mappingConfiguration in _configs)
            {
                await new MappingGenerator(mappingConfiguration, _generatorWorkspace).Generate();
            }
        }
    }
}
