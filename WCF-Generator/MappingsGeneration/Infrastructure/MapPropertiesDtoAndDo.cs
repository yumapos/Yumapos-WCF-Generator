using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WCFGenerator.MappingsGeneration.Infrastructure
{
    public class MapPropertiesDtoAndDo
    {
        public PropertyDeclarationSyntax DOPropertyName { get; set; }
        public PropertyDeclarationSyntax DTOPropertyName { get; set; }
        public TypeOfExpressionSyntax DOPropertyType { get; set; }
        public TypeOfExpressionSyntax DtoropertyType { get; set; }
        public string ToDtoFunction { get; set; }
        public string FromDtoFunction { get; set; }
        public KindOfProperty KindOMapfProperty { get; set; }
    }
}
