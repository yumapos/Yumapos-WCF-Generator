using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using WCFGenerator.ClientApiDecoratorsGeneration.Configuration;

namespace WCFGenerator.ClientApiDecoratorsGeneration.Generation
{
    public class ApiSecurityDecorator : IDecoratorClass
    {
        public string ClassName => "ApiSecurityDecorator";

        public string GetFullText(INamedTypeSymbol toDecorate, ClientApiDecoratorsConfiguration config)
        {
            var methods = toDecorate.GetMembers().Where(m => m.Kind == SymbolKind.Method);
        }
    }
}
