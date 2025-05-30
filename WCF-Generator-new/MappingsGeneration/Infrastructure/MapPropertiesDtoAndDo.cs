using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WCFGenerator.MappingsGeneration.Infrastructure
{
    public class MapPropertiesDtoAndDo
    {
        public IPropertySymbol DOPropertyName { get; set; }
        public IPropertySymbol DTOPropertyName { get; set; }
        public ITypeSymbol DOPropertyType { get; set; }
        public ITypeSymbol DtoropertyType { get; set; }
        public string ToDtoFunction { get; set; }
        public string FromDtoFunction { get; set; }
        public KindOfProperty KindOMapfProperty { get; set; }
    }
}
