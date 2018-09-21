using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WCFGenerator.ClientApiDecoratorsGeneration.Configuration;
using WCFGenerator.ClientApiDecoratorsGeneration.Generation;
using WCFGenerator.Common;

namespace WCFGenerator.ClientApiDecoratorsGeneration
{
    public class ClientApiDecoratorsGenerator
    {
        private readonly GeneratorWorkspace _generatorWorkspace;
        private readonly ClientApiDecoratorsConfiguration _config;
        private readonly INamedTypeSymbol _typeInfo;

        private readonly IDecoratorClass[] _decorators = new IDecoratorClass[]
        {
            new ApiSecurityDecorator(),
            new ServerRuntimeErrorDecorator(), 
            new UnauthorizeErrorApiDecorator(), 
        };

        public ClientApiDecoratorsGenerator(GeneratorWorkspace generatorWorkspace, ClientApiDecoratorsConfiguration config, INamedTypeSymbol typeInfo)
        {
            _generatorWorkspace = generatorWorkspace;
            _config = config;
            _typeInfo = typeInfo;
        }

        public async Task Generate()
        {
            _generatorWorkspace.SetTargetProject(_config.TargetProject);
            foreach (var decorator in _decorators)
            {
                var code = decorator.GetFullText(_typeInfo, _config);
                _generatorWorkspace.UpdateFileInTargetProject(decorator.ClassName + ".g.cs", _config.TargetFolder, code);
            }
            await _generatorWorkspace.ApplyTargetProjectChanges(true);
        }
    }
}
