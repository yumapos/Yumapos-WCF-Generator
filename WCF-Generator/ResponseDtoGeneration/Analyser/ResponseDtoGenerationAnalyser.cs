using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFGenerator.Common;
using WCFGenerator.Common.Infrastructure;
using WCFGenerator.ResponseDtoGeneration.Configuration;

namespace WCFGenerator.ResponseDtoGeneration.Analyser
{
    public class ResponseDtoGenerationAnalyser
    {
        private readonly ResponseDtoConfiguration _configuration;
        private readonly GeneratorWorkspace _generatorWorkspace;

        public ClassCompilerInfo[] Classes { get; private set; }

        public ResponseDtoGenerationAnalyser(ResponseDtoConfiguration configuration, GeneratorWorkspace generatorWorkspace)
        {
            _configuration = configuration;
            _generatorWorkspace = generatorWorkspace;
        }

        public async Task Run()
        {
            var cls = await _generatorWorkspace.Solution.GetAllClasses(_configuration.ProjectForGeneratedCode, false, "GenerateResponseDto");
            Classes = cls.ToArray();
        }
    }
}
