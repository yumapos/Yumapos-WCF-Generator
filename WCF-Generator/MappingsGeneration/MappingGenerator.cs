using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFGenerator.Common;
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
            return sb.ToString();
        }
    }
}
