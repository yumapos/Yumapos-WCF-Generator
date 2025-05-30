using System.Text;
using Microsoft.CodeAnalysis;
using WCFGenerator.Common;
using WCFGenerator.Common.ApiDecoration;

namespace WCFGenerator.ClientApiDecoratorsGeneration.Generation
{
    public class TerminalApiSecurityDecorator : ClientApiDecorator
    {
        public override string ClassName => "ApiSecurityDecorator";

        protected override void GenerateClassDeclaration(StringBuilder sb, GenerationConfig config)
        {
            var template = @"
        namespace {2}
        {{
            public sealed" + (config.PartialClass == null ? "" : " partial") + @" class {1} : {0}
            {{
            private readonly {0} _actor;
            private Func<IEnumerable<ResponseErrorDto>, Task> _warningsHandler;
		    
            private async Task HandleWarnings(IEnumerable<ResponseErrorDto> responseWarnings)
            {{
                if (_warningsHandler != null)
                {{
                    await _warningsHandler(responseWarnings);
			    }}
		    }}

#region Properties
		    public ExecutionContext ExecutionContext {{
                get {{ return _actor.ExecutionContext; }}
                set {{ _actor.ExecutionContext = value; }}
            }}
		    #endregion
		    public {1}({0} actor, Func<IEnumerable<ResponseErrorDto>, Task> warningHandler)
		    {{
			    if (actor == null) throw new ArgumentNullException(nameof(actor));
		        _actor = actor;
                _warningsHandler = warningHandler;
		    }}";

            sb.AppendFormat(template, config.SourceInterface, ClassName, config.TargetNamespace).AppendLine();
        }

        protected override void GenerateMethodBody(StringBuilder sb, IMethodSymbol toDecorate)
        {
            sb.AppendLine("var response = await _actor." + toDecorate.GetMethodCall() + ';');
            var returnTemplate = @"if (!response.Context.IsEmpty() || response.PostprocessingType != null){
                throw new ServerSecurityException(response.Context, response.PostprocessingType, response.Errors, response.ServerInfo);
            }
            return response;";

            var returnValueTempalte = @"if (!response.Context.IsEmpty() || response.PostprocessingType != null){
                throw new ServerSecurityException(response.Context, response.PostprocessingType, response.Errors, response.ServerInfo){Value = response.Value};
            }

            if (response.Warnings != null && response.Warnings.Any())
            {
				await HandleWarnings(response.Warnings);
            }

            return response;";

            if (toDecorate.ReturnType.GetGenericArguments()[0].GetFullName() == "YumaPos.Shared.API.ResponseDtos.ResponseDto")
            {
                sb.AppendLine(returnTemplate);
            }
            else
            {
                sb.AppendLine(returnValueTempalte);
            }
        }
        
        protected override void GenerateUsings(StringBuilder sb)
        {
            const string usings = @"using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Threading.Tasks;
            using YumaPos.Shared.Terminal.Infrastructure;
            using YumaPos.Shared.API.ResponseDtos;
            using YumaPos.FrontEnd.Infrastructure.CommandProcessing;";
            sb.AppendLine(usings);
        }
    }
}
