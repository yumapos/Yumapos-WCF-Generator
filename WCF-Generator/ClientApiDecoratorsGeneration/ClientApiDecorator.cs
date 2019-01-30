using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using WCFGenerator.Common;
using WCFGenerator.Common.ApiDecoration;

namespace WCFGenerator.ClientApiDecoratorsGeneration
{
    public abstract class ClientApiDecorator : BaseDecorator
    {
        protected override void GenerateTemplate(StringBuilder sb, GenerationConfig config)
        {
            const string template = @"
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
        }

        protected override void GenerateMethods(StringBuilder sb, GenerationConfig config)
        {
            var methods = config.ToDecorate.GetMembers().Where(m => m.Kind == SymbolKind.Method).Cast<IMethodSymbol>().Where(m =>
                m.MethodKind != MethodKind.PropertyGet && m.MethodKind != MethodKind.PropertySet);
            foreach (var method in methods)
            {
                var sign = method.GetSignature();
                sb.AppendLine("public " + (method.CanBeAwaited() ? " async " : "") + sign);
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
        }

        protected override void GenerateUsings(StringBuilder sb)
        {
            const string usings = @"using System;
            using YumaPos.Shared.Terminal.Infrastructure;
            using YumaPos.FrontEnd.Infrastructure.CommandProcessing;";
            sb.AppendLine(usings);
        }
    }
}
