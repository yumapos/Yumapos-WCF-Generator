using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using WCFGenerator.Common;

namespace WCFGenerator.ClientApiDecoratorsGeneration.Generation
{
    public class UnauthorizeErrorApiDecorator : BaseDecorator
    {
        public override string ClassName => "UnauthorizeErrorApiDecorator";
        protected override void GenerateMethodBody(StringBuilder sb, IMethodSymbol toDecorate)
        {
            sb.AppendLine("try");
            sb.AppendLine("{");
            sb.AppendLine("return await _actor." + toDecorate.GetMethodCall() + ";");
            sb.AppendLine("}");
            var returnTemplate = @"catch (FaultException<UnauthorizedFault>)
			{
			    throw new UnauthorizedException();
			}
			catch (FaultException ex)
            {
                if (ex.Code.Name.Equals(""401"")) throw new UnauthorizedException();
                throw;
            }";
            sb.AppendLine(returnTemplate);
        }

        protected override void GetUsings(StringBuilder sb)
        {
            var usings = @"using System;
            using System.Linq;
            using System.ServiceModel;
            using YumaPos.Shared.Terminal.Infrastructure;
            using YumaPos.FrontEnd.Infrastructure.CommandProcessing;
            using YumaPos.Shared.Exceptions;
            using YumaPos.Shared.API.Faults;";
            sb.AppendLine(usings);
        }
    }
}
