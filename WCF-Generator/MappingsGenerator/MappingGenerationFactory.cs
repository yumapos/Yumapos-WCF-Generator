using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFGenerator.Common;
using WCFGenerator.MappingsGenerator.Configuration;

namespace WCFGenerator.MappingsGenerator
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
