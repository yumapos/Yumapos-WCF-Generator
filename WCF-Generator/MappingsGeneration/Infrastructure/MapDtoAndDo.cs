using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WCFGenerator.Common.Infrastructure;

namespace WCFGenerator.MappingsGeneration.Infrastructure
{
    public class MapDtoAndDo
    {
        public ClassCompilerInfo DOClass { get; set; }
        public ClassCompilerInfo DtoClass { get; set; }
        public INamedTypeSymbol DOInterface { get; set; }
        public INamedTypeSymbol DtoInterface { get; set; }
        public IEnumerable<MapPropertiesDtoAndDo> MapPropertiesDtoAndDo { get; set; }
        public IPropertySymbol[] IsIgnoreDOProperties { get; set; }
        public IPropertySymbol[] IsIgnoreDTOProperties { get; set; }

        public bool IsClassesIntersects { get; set; }
    }
}
