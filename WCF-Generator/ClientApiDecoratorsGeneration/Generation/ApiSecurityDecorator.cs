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
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using YumaPos.Shared.Terminal.Infrastructure;");
            sb.AppendLine("using YumaPos.FrontEnd.Infrastructure.CommandProcessing;");

            string template = @"
        namespace {2}
        {{
            public sealed class {1} : {0}
            {{
            private readonly {0} _actor;

		    #region Properties

		    public ExecutionContext ExecutionContext {{
                get {{ return _actor.ExecutionContext; }}
                set {{ _actor.ExecutionContext = value; }}
            }}

		    #endregion

		    public {1}({0} actor)
		    {{
			    if (actor == null) throw new ArgumentNullException(nameof(actor));

		        _actor = actor;
		    }}";

            sb.AppendFormat(template, config.SourceInterface, ClassName, config.TargetNamespace).AppendLine();

            var methods = toDecorate.GetMembers().Where(m => m.Kind == SymbolKind.Method);

            sb.AppendLine("}");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
