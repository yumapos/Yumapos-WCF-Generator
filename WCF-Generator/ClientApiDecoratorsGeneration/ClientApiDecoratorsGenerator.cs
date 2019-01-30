using WCFGenerator.ClientApiDecoratorsGeneration.Generation;
using WCFGenerator.Common;
using WCFGenerator.Common.ApiDecoration;

namespace WCFGenerator.ClientApiDecoratorsGeneration
{
    public class ClientApiDecoratorsGenerator : DecaratorsGenerator
    {
        public ClientApiDecoratorsGenerator(GeneratorWorkspace generatorWorkspace, GenerationConfig config) : base(generatorWorkspace, config)
        {
            Decorators = new IDecoratorClass[]
            {
                new TerminalApiSecurityDecorator(), 
                new ServerRuntimeErrorDecorator(),
                new UnauthorizeErrorApiDecorator(),
            };
        }
    }
}
