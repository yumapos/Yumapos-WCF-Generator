using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WCFGenerator.ClientApiDecoratorsGeneration.Configuration;
using WCFGenerator.Common;

namespace WCFGenerator.ClientApiDecoratorsGeneration
{
    public class ClientApiGenerationFactory
    {
        private readonly GeneratorWorkspace _generatorWorkspace;
        private readonly ClientApiDecoratorsConfiguration[] _configs;

        public ClientApiGenerationFactory(GeneratorWorkspace generatorWorkspace, ClientApiDecoratorsConfiguration[] configs)
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
                var compilation = (CSharpCompilation) (await project.GetCompilationAsync());
                foreach (var groupedConfigItem in groupedConfig)
                {
                    var interfaceInfo = compilation.GetClass(groupedConfigItem.SourceInterface);
                    new ClientApiDecoratorsGenerator(_generatorWorkspace, groupedConfigItem, interfaceInfo).Generate();
                }
            }
        }
    }
}
