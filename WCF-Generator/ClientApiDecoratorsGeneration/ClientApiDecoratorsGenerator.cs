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
        private readonly INamedTypeSymbol _partialClassInfo;

        public ClientApiDecoratorsGenerator(GeneratorWorkspace generatorWorkspace, ClientApiDecoratorsConfiguration config, INamedTypeSymbol typeInfo, INamedTypeSymbol partialClassInfo = null)
        {
            _generatorWorkspace = generatorWorkspace;
            _config = config;
            _typeInfo = typeInfo;
            _partialClassInfo = partialClassInfo;
        }

        public async Task Generate()
        {
            _generatorWorkspace.SetTargetProject(_config.TargetProject);
            if (_config.ApiSecurityEnabled)
            {
                GenerateByDecorator(new ApiSecurityDecorator());
            }
            if (_config.ServerRuntimeErrorEnabled)
            {
                GenerateByDecorator(new ServerRuntimeErrorDecorator());
            }
            if (_config.UnauthorizeErrorApiEnabled)
            {
                GenerateByDecorator(new UnauthorizeErrorApiDecorator());
            }
            await _generatorWorkspace.ApplyTargetProjectChanges(true);
        }

        private void GenerateByDecorator(IDecoratorClass decorator)
        {
            var code = decorator.GetFullText(_typeInfo, _config, _partialClassInfo);
            _generatorWorkspace.UpdateFileInTargetProject(decorator.ClassName + ".g.cs", _config.TargetFolder, code);
        }
    }
}
