using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFGenerator.Common;
using WCFGenerator.Common.Configuration;
using WCFGenerator.Common.Infrastructure;
using WCFGenerator.MappingsGeneration.Configuration;
using WCFGenerator.MappingsGenerator.Analysis;
using WCFGenerator.ResponseDtoGeneration.Analyser;
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
            var analyser = new ResponseDtoGenerationAnalyser(_configuration, _generatorWorkspace);
            await analyser.Run();
            var code = GetFullCode(analyser.Classes);
            _generatorWorkspace.SetTargetProject(_configuration.ProjectForGeneratedCode);
            _generatorWorkspace.UpdateFileInTargetProject("GenerateResponseDto.g.cs", "Generation", code);
            await _generatorWorkspace.ApplyTargetProjectChanges(true);
        }

        private string GetFullCode(ClassCompilerInfo[] classes)
        {
            var sb = new StringBuilder();
            foreach (PrefixString prefixString in _configuration.PrefixStrings)
            {
                sb.AppendLine(prefixString.Text);
            }

            sb.AppendLine("");
            sb.AppendLine("namespace " + _configuration.MapExtensionNameSpace);
            sb.AppendLine("{");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}