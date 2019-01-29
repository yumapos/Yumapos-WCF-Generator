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
            var returnTemplate = @"if (response?.PostprocessingType != null){
                throw new CustomerException(response.Context, response.PostprocessingType.Value, response.Errors);
            }
            return response;";
                sb.AppendLine(returnTemplate);
           
        }
    }
}
