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
            string code;
            if (_configuration.UseResponseDtoAsBaseClass)
            {
                code = GetFullCodeUsingResponseDtoAsBaseClass(analyser.Classes);
            }
            else
            {
                code = GetFullCode(analyser.Classes);
            }
            _generatorWorkspace.SetTargetProject(_configuration.ProjectForGeneratedCode);
            _generatorWorkspace.UpdateFileInTargetProject("GenerateResponseDto.g.cs", "Generation", code);
            await _generatorWorkspace.ApplyTargetProjectChanges(true);
        }

        private string GetFullCode(ClassCompilerInfo[] classes)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"
using System.Collections.Generic;
using System.Runtime.Serialization;
using YumaPos.FrontEnd.Infrastructure.CommandProcessing;
using YumaPos.Shared.API.ResponseDtos;
");
            foreach (PrefixString prefixString in _configuration.PrefixStrings)
            {
                sb.AppendLine(prefixString.Text);
            }

            sb.AppendLine("");
            sb.AppendLine("namespace " + _configuration.MapExtensionNameSpace);
            sb.AppendLine("{");

            foreach (var classCompilerInfo in classes)
            {
                var className = classCompilerInfo.NamedTypeSymbol.Name;
                string nameWithoutDto = className;
                if (className.EndsWith("Dto"))
                {
                    nameWithoutDto = className.Substring(0, className.Length - 3);
                }
                sb.AppendLine(@"[DataContract]
        public class " + nameWithoutDto + @"ResponseDto
        {
            [DataMember]
            public " + className + @" Value { get; set; }
            [DataMember]
            public virtual IEnumerable<ResponseErrorDto> Errors { get; set; }
            [DataMember]
            public virtual IEnumerable<ResponseErrorDto> Warnings { get; set; }
            [DataMember]
            public virtual ResponseServerInfoDto ServerInfo { get; set; }
            [DataMember]
            public string Context { get; set; }
            [DataMember]
            public CommandPostprocessingType? PostprocessingType { get; set; }
        }

        [DataContract]
        public class " + nameWithoutDto + @"ListResponseDto
        {
            [DataMember]
            public IEnumerable<" + className + @"> Value{get;set;}
            [DataMember]
            public virtual IEnumerable<ResponseErrorDto> Errors { get; set; }
            [DataMember]
            public virtual IEnumerable<ResponseErrorDto> Warnings { get; set; }
            [DataMember]
            public virtual ResponseServerInfoDto ServerInfo { get; set; }
            [DataMember]
            public string Context { get; set; }
            [DataMember]
            public CommandPostprocessingType? PostprocessingType { get; set; }
        }");
                sb.AppendLine("");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GetFullCodeUsingResponseDtoAsBaseClass(ClassCompilerInfo[] classes)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"
using System.Collections.Generic;
using System.Runtime.Serialization;
using YumaPos.FrontEnd.Infrastructure.CommandProcessing;
using YumaPos.Shared.API.ResponseDtos;
");
            foreach (PrefixString prefixString in _configuration.PrefixStrings)
            {
                sb.AppendLine(prefixString.Text);
            }

            sb.AppendLine("");
            sb.AppendLine("namespace " + _configuration.MapExtensionNameSpace);
            sb.AppendLine("{");

            foreach (var classCompilerInfo in classes)
            {
                var className = classCompilerInfo.NamedTypeSymbol.Name;
                string nameWithoutDto = className;
                if (className.EndsWith("Dto"))
                {
                    nameWithoutDto = className.Substring(0, className.Length - 3);
                }
                sb.AppendLine(@"[DataContract]
        public class " + nameWithoutDto + @"ResponseDto : ResponseDto
        {
            [DataMember]
            public " + className + @" Value { get; set; }
        }

        [DataContract]
        public class " + nameWithoutDto + @"ListResponseDto : ResponseDto
        {
            [DataMember]
            public IEnumerable<" + className + @"> Value{get;set;}
        }");
                sb.AppendLine("");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}