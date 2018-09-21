using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using WCFGenerator.ClientApiDecoratorsGeneration.Configuration;

namespace WCFGenerator.ClientApiDecoratorsGeneration.Generation
{
    public class ApiSecurityDecorator : BaseDecorator
    {
        public override string ClassName => "ApiSecurityDecorator";
        protected override void GenerateMethodBody(StringBuilder sb, IMethodSymbol toDecorate)
        {

        }
    }
}
