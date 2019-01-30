using System.Text;
using Microsoft.CodeAnalysis;
using WCFGenerator.Common;

namespace WCFGenerator.CustomerApiDecoratorsGeneration.Generation
{
    public class CustomerApiSecuirityDecorator : CustomerApiDecorator
    {
        public override string ClassName => "ApiSecurityDecorator";
        protected override void GenerateMethodBody(StringBuilder sb, IMethodSymbol toDecorate)
        {
            sb.AppendLine("var response = await _actor." + toDecorate.GetMethodCall() + ';');
            const string returnTemplate = @"if (response?.PostprocessingType != null)
            {
                throw new CustomerException(response.Context, response.PostprocessingType.Value, response.Errors);
            }
            return response;";

            sb.AppendLine(returnTemplate);
        }
    }
}
