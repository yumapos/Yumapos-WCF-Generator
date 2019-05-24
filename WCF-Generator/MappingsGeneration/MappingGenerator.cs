using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFGenerator.Common;
using WCFGenerator.Common.Configuration;
using WCFGenerator.Common.Infrastructure;
using WCFGenerator.MappingsGeneration.Configuration;
using WCFGenerator.MappingsGeneration.Infrastructure;
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

        public async Task Generate()
        {
            var analyser = new MappingAnalyser(_configuration, _generatorWorkspace);
            await analyser.Run();
            var code = GetFullCode(analyser.ListOfSimilarClasses.ToArray(), analyser.ClassesWithoutPair.ToArray());
            _generatorWorkspace.SetTargetProject(_configuration.ProjectForGeneratedCode);
            _generatorWorkspace.UpdateFileInTargetProject("MappingExtension.g.cs", "Generation", code);
            await _generatorWorkspace.ApplyTargetProjectChanges(true);
        }

        private string GetFullCode(MapDtoAndDo[] similarClasses, ClassCompilerInfo[] classesWithoutPair)
        {
            var sb = new StringBuilder();
            foreach (PrefixString prefixString in _configuration.PrefixStrings)
            {
                sb.AppendLine(prefixString.Text);
            }
            sb.AppendLine("");
            sb.AppendLine("namespace " + _configuration.MapExtensionNameSpace);
            sb.AppendLine("{");
            sb.AppendLine("public static class " + _configuration.MapExtensionClassName);
            sb.AppendLine("{");
            foreach (var classWithoutPair in classesWithoutPair)
            {
                sb.AppendLine("//" + classWithoutPair.NamedTypeSymbol.GetFullName());
            }

            foreach (var similarClass in similarClasses)
            {
                sb.AppendLine("");
                var doClassName = similarClass.DtoClass != null ? similarClass.DOClass.NamedTypeSymbol.GetFullName() : similarClass.DOInterface.GetFullName();
                var dtoClassName = similarClass.DtoClass != null ? similarClass.DtoClass.NamedTypeSymbol.GetFullName() : similarClass.DtoInterface.GetFullName();
                sb.AppendLine("public static " + dtoClassName + " MapToDto (this " + doClassName + " item)");
                sb.AppendLine("{");
                foreach (var prop in similarClass.IsIgnoreDOProperties)
                {
                    sb.AppendLine("//itemDo." + prop.Name);
                }
                sb.AppendLine("");
                sb.AppendLine("if (item == null) return null;");
                sb.AppendLine("");
                sb.AppendLine("var itemDto = new " + dtoClassName + "();");
                foreach (var property in similarClass.MapPropertiesDtoAndDo)
                {
                    if (!String.IsNullOrEmpty(property.ToDtoFunction))
                    {
                        sb.AppendLine(property.ToDtoFunction + ";");
                    }
                }
                sb.AppendLine("");
                sb.AppendLine("return itemDto;");
                sb.AppendLine("}");
                sb.AppendLine("");
                sb.AppendLine("public static " + doClassName + " MapFromDto (this " + dtoClassName + " itemDto)");
                sb.AppendLine("{");
                foreach (var prop in similarClass.IsIgnoreDTOProperties)
                {
                    sb.AppendLine("//itemDto." + prop.Name);
                }
                sb.AppendLine("");
                sb.AppendLine("if (itemDto == null) return null;");
                sb.AppendLine("");
                sb.AppendLine("var item = new " + doClassName + "();");
                foreach (var property in similarClass.MapPropertiesDtoAndDo)
                {
                    if (!String.IsNullOrEmpty(property.FromDtoFunction))
                    {
                        sb.AppendLine(property.FromDtoFunction + ";");
                    }
                }
                sb.AppendLine("");
                sb.AppendLine("return item;");
                sb.AppendLine("}");
            }

            sb.AppendLine("}");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
