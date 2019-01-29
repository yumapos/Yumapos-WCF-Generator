using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using WCFGenerator.ClientApiDecoratorsGeneration.Configuration;

namespace WCFGenerator.ClientApiDecoratorsGeneration.Generation
{
    public interface IDecoratorClass
    {
        string ClassName { get; }
        string GetFullText(INamedTypeSymbol toDecorate, ClientApiDecoratorsConfiguration config, INamedTypeSymbol partialClassInfo = null);
    }
}
