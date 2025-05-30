using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using WCFGenerator.Common;
using WCFGenerator.Common.ApiDecoration;

namespace WCFGenerator.ClientApiDecoratorsGeneration.Generation
{
    public class TerminalApiLogOfCallsDecorator : ClientApiDecorator
    {
        public override string ClassName => "ApiLogOfCallsDecorator";

        #region Overrides of ClientApiDecorator

        protected override void GenerateClassDeclaration(StringBuilder sb, GenerationConfig config)
        {
            sb.AppendLine($"namespace {config.TargetNamespace}");
            sb.AppendLine("{");

            sb.AppendLine($"public sealed class {ClassName} : {config.SourceInterface}");
            sb.AppendLine("{");
            // fields
            sb.AppendLine($"private readonly {config.SourceInterface} _actor;");
            sb.AppendLine($"private readonly Action<string> _logger;");
            sb.AppendLine($"private readonly Func<object, string> _serializer;");
            sb.AppendLine($"private readonly Func<bool> _logEnabledProvider;");

            //properties
            sb.AppendLine("#region Properties");
            sb.AppendLine("public ExecutionContext ExecutionContext");
            sb.AppendLine("{");
            sb.AppendLine("get { { return _actor.ExecutionContext; } }");
            sb.AppendLine("set { { _actor.ExecutionContext = value; } }");
            sb.AppendLine("}");
            sb.AppendLine("#endregion");

            // constructor
            sb.AppendLine($"public {ClassName}({config.SourceInterface} actor, Func<bool> logEnabledProvider, Action<string> logger, Func<object, string> serializer)");
            sb.AppendLine("{");
            sb.AppendLine("if (actor == null) throw new ArgumentNullException(nameof(actor));");
            sb.AppendLine("if (logger == null) throw new ArgumentNullException(nameof(logger));");
            sb.AppendLine("if (serializer == null) throw new ArgumentNullException(nameof(serializer));");
            sb.AppendLine("if (logEnabledProvider == null) throw new ArgumentNullException(nameof(logEnabledProvider));");
            sb.AppendLine("_actor = actor;");
            sb.AppendLine("_logger = logger;");
            sb.AppendLine("_serializer = serializer;");
            sb.AppendLine("_logEnabledProvider = logEnabledProvider;");
            sb.AppendLine("}");

            sb.AppendLine("private void LogRequest(string method, object requestDto)");
            sb.AppendLine("{");
            sb.AppendLine("if (_logEnabledProvider())");
            sb.AppendLine("{");
            sb.AppendLine("_logger($\"[API] [REQUEST] {method}\\nExecutionContext: {_serializer(ExecutionContext)}\\nRequest data: {_serializer(requestDto)}\");");
            sb.AppendLine("}");
            sb.AppendLine("}");

            sb.AppendLine("private void LogResponse(string method, object responseDto)");
            sb.AppendLine("{");
            sb.AppendLine("if (_logEnabledProvider())");
            sb.AppendLine("{");
            sb.AppendLine("_logger($\"[API] [RESPONSE] {method}\\nExecutionContext: {_serializer(ExecutionContext)}\\nResponse data: {_serializer(responseDto)}\");");
            sb.AppendLine("}");
            sb.AppendLine("}");
        }

        #endregion

        protected override void GenerateMethodBody(StringBuilder sb, IMethodSymbol toDecorate)
        {
            var returnTemplate = @"if (!response.Context.IsEmpty() || response.PostprocessingType != null){
                throw new ServerSecurityException(response.Context, response.PostprocessingType, response.Errors, response.ServerInfo);
            }
            return response;";

            var returnValueTempalte = @"if (!response.Context.IsEmpty() || response.PostprocessingType != null){
                throw new ServerSecurityException(response.Context, response.PostprocessingType, response.Errors, response.ServerInfo){Value = response.Value};
            }
            return response;";

            sb.AppendLine("var parameters = new {");
            foreach (var parameter in toDecorate.Parameters)
            {
                sb.AppendLine($"{parameter.Name},");
            }
            sb.AppendLine("};");
            
            sb.AppendLine($"LogRequest(\"{toDecorate.ContainingType.Name}.{toDecorate.Name}\", parameters);");
            sb.AppendLine("var response = await _actor." + toDecorate.GetMethodCall() + ';');
            sb.AppendLine($"LogResponse(\"{toDecorate.ContainingType.Name}.{toDecorate.Name}\", response);");

            if (toDecorate.ReturnType.GetGenericArguments()[0].GetFullName() == "YumaPos.Shared.API.ResponseDtos.ResponseDto")
            {
                sb.AppendLine(returnTemplate);
            }
            else
            {
                sb.AppendLine(returnValueTempalte);
            }
        }
    }
}
