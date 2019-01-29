using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using WCFGenerator.ClientApiDecoratorsGeneration.Configuration;
using WCFGenerator.Common;

namespace WCFGenerator.ClientApiDecoratorsGeneration.Generation
{
    public abstract class BaseDecorator : IDecoratorClass
    {
        public abstract string ClassName { get; }
        public string GetFullText(INamedTypeSymbol toDecorate, ClientApiDecoratorsConfiguration config, INamedTypeSymbol partialClassInfo = null)
        {
            var sb = new StringBuilder();
            GetUsings(sb);

            string template = @"
        namespace {2}
        {{
            public sealed partial class {1} : {0}
            {{
            private readonly {3} _actor;

		    public {1}({0} actor)
		    {{
			    if (actor == null) throw new ArgumentNullException(nameof(actor));

		        _actor = actor;
		    }}";
            var baseInterface = toDecorate.AllInterfaces.FirstOrDefault();
            sb.AppendFormat(template, config.SourceInterface, ClassName, config.TargetNamespace, baseInterface.MetadataName ?? config.SourceInterface).AppendLine();
            
            var methods = baseInterface.GetMembers().Where(m => m.Kind == SymbolKind.Method).Cast<IMethodSymbol>().Where(m => 
                m.MethodKind != MethodKind.PropertyGet && m.MethodKind != MethodKind.PropertySet );
            var partialMethods = partialClassInfo?.GetMembers().Where(m => m.Locations.Any(l => !l.SourceTree.FilePath.Contains(".g.cs"))) ?? Enumerable.Empty<ISymbol>();
            foreach (var method in methods)
            {
                if (partialMethods.Any(m => m.Name == method.Name))
                {
                    continue;
                }
                var sign = method.GetSignature();
                sb.AppendLine("public " + (method.CanBeAwaited()?" async ":"") + sign);
                sb.AppendLine("{");
                if (method.ReturnsVoid)
                {
                    sb.AppendLine("_actor." + method.GetMethodCall() + ";");
                }
                else
                {
                    GenerateMethodBody(sb, method);
                }
                sb.AppendLine("}");
                sb.AppendLine();
            }

            sb.AppendLine("}");
            sb.AppendLine("}");
            return sb.ToString();
        }

        protected virtual void GetUsings(StringBuilder sb)
        {
            var usings = @"using System;
                        using YumaPos.Customer.Shared.Infrastructure.Services;
                        using YumaPos.Shared.Online.Infrastructure;
                        using YumaPos.Customer.Shared.Infrastructure.Exceptions;";
            sb.AppendLine(usings);
        }

        protected abstract void GenerateMethodBody(StringBuilder sb, IMethodSymbol toDecorate);
    }
}
