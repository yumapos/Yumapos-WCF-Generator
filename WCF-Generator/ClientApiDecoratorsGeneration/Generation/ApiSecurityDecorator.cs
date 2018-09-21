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
    public class ApiSecurityDecorator : BaseDecorator
    {
        public override string ClassName => "ApiSecurityDecorator";
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
    }
}
