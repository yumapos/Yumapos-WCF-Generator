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

        public void GenerateAll()
        {
            foreach (var mappingConfiguration in _configs)
            {
                new MappingGenerator(mappingConfiguration, _generatorWorkspace).Generate();
            }
        }
    }
}
