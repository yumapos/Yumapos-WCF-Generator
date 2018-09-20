using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WCFGenerator.ClientApiDecoratorsGeneration.Configuration;
using WCFGenerator.Common;

namespace WCFGenerator.ClientApiDecoratorsGeneration
{
    public class ClientApiDecoratorsGenerator
    {
        private readonly GeneratorWorkspace _generatorWorkspace;
        private readonly ClientApiDecoratorsConfiguration _config;
        private readonly INamedTypeSymbol _typeInfo;

        public ClientApiDecoratorsGenerator(GeneratorWorkspace generatorWorkspace, ClientApiDecoratorsConfiguration config, INamedTypeSymbol typeInfo)
        {
            _generatorWorkspace = generatorWorkspace;
            _config = config;
            _typeInfo = typeInfo;
        }

        public void Generate()
        {
            var methods = _typeInfo.GetMembers().Where(m => m.Kind == SymbolKind.Method);
        }
    }
}
