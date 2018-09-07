using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WCFGenerator.MappingsGeneration.Infrastructure
{
    public class MapDtoAndDo
    {
        public ClassDeclarationSyntax DOClass { get; set; }
        public ClassDeclarationSyntax DtoClass { get; set; }
        public InterfaceDeclarationSyntax DOInterface { get; set; }
        public InterfaceDeclarationSyntax DtoInterface { get; set; }
        public IEnumerable<MapPropertiesDtoAndDo> MapPropertiesDtoAndDo { get; set; }
    }
}
