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
        public string GetFullText(INamedTypeSymbol toDecorate, ClientApiDecoratorsConfiguration config)
        {
            var sb = new StringBuilder();
            GetUsings(sb);

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

            var methods = toDecorate.GetMembers().Where(m => m.Kind == SymbolKind.Method).Cast<IMethodSymbol>().Where(m => 
                m.MethodKind != MethodKind.PropertyGet && m.MethodKind != MethodKind.PropertySet );
            foreach (var method in methods)
            {
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
            sb.AppendLine("using System;");
            sb.AppendLine("using YumaPos.Shared.Terminal.Infrastructure;");
            sb.AppendLine("using YumaPos.FrontEnd.Infrastructure.CommandProcessing;");
        }

        protected abstract void GenerateMethodBody(StringBuilder sb, IMethodSymbol toDecorate);
    }
}
