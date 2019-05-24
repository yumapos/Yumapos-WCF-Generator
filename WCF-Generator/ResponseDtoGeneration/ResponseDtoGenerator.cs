using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFGenerator.Common;
using WCFGenerator.MappingsGeneration.Configuration;
using WCFGenerator.MappingsGenerator.Analysis;
using WCFGenerator.ResponseDtoGeneration.Configuration;


namespace WCFGenerator.ResponseDtoGeneration
{
    public class ResponseDtoGenerator
    {
        private readonly ResponseDtoConfiguration _configuration;
        private readonly GeneratorWorkspace _generatorWorkspace;

        public ResponseDtoGenerator(ResponseDtoConfiguration configuration, GeneratorWorkspace generatorWorkspace)
        {
            _configuration = configuration;
            _generatorWorkspace = generatorWorkspace;
        }

        public async Task Generate()
        {
        }
    }
}
