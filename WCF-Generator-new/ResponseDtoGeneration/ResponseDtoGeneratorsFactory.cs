using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFGenerator.Common;
using WCFGenerator.ResponseDtoGeneration.Configuration;

namespace WCFGenerator.ResponseDtoGeneration
{
    public class ResponseDtoGeneratorsFactory
    {
        private readonly GeneratorWorkspace _generatorWorkspace;
        private readonly ResponseDtoConfiguration[] _configs;

        public ResponseDtoGeneratorsFactory(GeneratorWorkspace generatorWorkspace, ResponseDtoConfiguration[] configs)
        {
            _generatorWorkspace = generatorWorkspace;
            _configs = configs;
        }

        public async Task GenerateAll()
        {
            foreach (var cfg in _configs)
            {
                await new ResponseDtoGenerator(cfg, _generatorWorkspace).Generate();
            }
        }
}
}
