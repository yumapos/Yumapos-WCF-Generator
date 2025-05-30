using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFGenerator.Common.Infrastructure;

namespace WCFGenerator.MappingsGeneration.Infrastructure
{
    public class MapEnumDtoAndDo
    {
        public EnumCompilerInfo DoEnum { get; set; }
        public EnumCompilerInfo DtoEnum { get; set; }
        public MapEnumField[] MapEnumFields { get; set; }
        public IFieldSymbol DefaultDoEnumField { get; set; }
        public IFieldSymbol DefaultDtoEnumField { get; set; }
    }

    public class MapEnumField
    {
        public IFieldSymbol DoField { get; set; }
        public IFieldSymbol DtoField { get; set; }
    }
}
