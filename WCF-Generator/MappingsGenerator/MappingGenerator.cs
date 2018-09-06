using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFGenerator.Common;
using WCFGenerator.MappingsGenerator.Configuration;

namespace WCFGenerator.MappingsGenerator
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

        }
    }
}
