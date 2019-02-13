using WCFGenerator.Common;
using WCFGenerator.Common.ApiDecoration;
using WCFGenerator.CustomerApiDecoratorsGeneration.Generation;

namespace WCFGenerator.CustomerApiDecoratorsGeneration
{
    public class CustomerApiDecoratorsGenerator : DecoratorsGenerator
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
