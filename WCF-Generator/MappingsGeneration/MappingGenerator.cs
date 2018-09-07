using WCFGenerator.Common;
using WCFGenerator.MappingsGeneration.Configuration;
using WCFGenerator.MappingsGenerator.Analysis;

namespace WCFGenerator.MappingsGeneration
{
    public class MappingGenerator
    {
        private readonly MappingConfiguration _configuration;
        private readonly GeneratorWorkspace _generatorWorkspace;

        public MappingGenerator(MappingConfiguration configuration, GeneratorWorkspace generatorWorkspace)
        {
            _configuration = configuration;
            _generatorWorkspace = generatorWorkspace;
        }

        public void Generate()
        {
            var analyser = new MappingAnalyser(_configuration, _generatorWorkspace);
        }
    }
}
