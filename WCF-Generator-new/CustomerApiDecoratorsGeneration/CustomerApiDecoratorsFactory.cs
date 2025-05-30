using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using WCFGenerator.Common;
using WCFGenerator.Common.ApiDecoration;

namespace WCFGenerator.CustomerApiDecoratorsGeneration
{
    public class CustomerApiDecoratorsFactory
    {
        private readonly GeneratorWorkspace _generatorWorkspace;
        private readonly ApiDecoratorsConfiguration[] _configs;

        public CustomerApiDecoratorsFactory(GeneratorWorkspace generatorWorkspace, ApiDecoratorsConfiguration[] configs)
        {
            _generatorWorkspace = generatorWorkspace;
            _configs = configs;
        }

        public async Task GenerateAll()
        {
            var groupedConfigs = _configs.GroupBy(g => g.SourceProject);
            foreach (var groupedConfig in groupedConfigs)
            {
                var project = _generatorWorkspace.Solution.Projects.First(x => x.Name == groupedConfig.First().SourceProject);
                var compilation = (CSharpCompilation)(await project.GetCompilationAsync());
                foreach (var groupedConfigItem in groupedConfig)
                {
                    var interfaceInfo = compilation.GetClass(groupedConfigItem.SourceInterface);
                    var partialClass = string.IsNullOrEmpty(groupedConfigItem.PartialClass) ? null : compilation.GetClass(groupedConfigItem.PartialClass);
                    await new CustomerApiDecoratorsGenerator(_generatorWorkspace,
                        new GenerationConfig(groupedConfigItem.SourceInterface, partialClass, interfaceInfo, groupedConfigItem.TargetNamespace, groupedConfigItem.TargetProject,
                            groupedConfigItem.TargetFolder)).Generate();
                }
            }
        }
    }
}
