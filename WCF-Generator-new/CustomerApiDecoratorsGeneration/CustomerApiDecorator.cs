using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using WCFGenerator.Common;
using WCFGenerator.Common.ApiDecoration;

namespace WCFGenerator.CustomerApiDecoratorsGeneration
{
    public abstract class CustomerApiDecorator : BaseDecorator
    {
        protected override void GenerateClassDeclaration(StringBuilder sb, GenerationConfig config)
        {
            var template = @"
        namespace {2}
        {{
            public" + (config.PartialClass == null ? "" : " partial") + @" class {1} : {0}
            {{
            private readonly {3} _actor;

		    public {1}({3} actor)
		    {{
			    if (actor == null) throw new ArgumentNullException(nameof(actor));
		        _actor = actor;
		    }}";

            sb.AppendFormat(template, config.SourceInterface, ClassName, config.TargetNamespace, config.ToDecorate.AllInterfaces.FirstOrDefault()).AppendLine();
        }

        protected override void GenerateMethods(StringBuilder sb, GenerationConfig config)
        {
            var baseInterface = config.ToDecorate.AllInterfaces.FirstOrDefault();
            var methods = baseInterface.GetMembers().Where(m => m.Kind == SymbolKind.Method).Cast<IMethodSymbol>().Where(m =>
                m.MethodKind != MethodKind.PropertyGet && m.MethodKind != MethodKind.PropertySet);
            var partialMethods = (config.PartialClass?.GetMembers().Where(m => m.Locations.Any(l => !l.SourceTree.FilePath.Contains(".g.cs"))) ?? Enumerable.Empty<ISymbol>()).ToList();
            foreach (var method in methods)
            {
                var isCommented = partialMethods.Any(m => m.Name == method.Name);
                if (isCommented)
                {
                    sb.AppendLine("/*");
                }
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
                if (isCommented)
                {
                    sb.AppendLine("*/");
                }
                sb.AppendLine();
            }
        }

        protected override void GenerateUsings(StringBuilder sb)
        {
            const string usings = @"using System;
            using YumaPos.Customer.Shared.Infrastructure.Exceptions;";
            sb.AppendLine(usings);
        }
    }
}
