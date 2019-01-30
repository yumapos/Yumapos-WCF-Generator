using Microsoft.CodeAnalysis;
using WCFGenerator.ClientApiDecoratorsGeneration.Configuration;
using WCFGenerator.Common;
using WCFGenerator.Common.ApiDecoration;
using WCFGenerator.CustomerApiDecoratorsGeneration.Generation;

namespace WCFGenerator.CustomerApiDecoratorsGeneration
{
    public class CustomerApiDecoratorsGenerator : DecaratorsGenerator
    {
        public CustomerApiDecoratorsGenerator(GeneratorWorkspace generatorWorkspace, GenerationConfig config) : base(generatorWorkspace, config)
        {
            Decorators = new IDecoratorClass[]
            {
                new CustomerApiSecuirityDecorator()
            };
        }
    }
}
