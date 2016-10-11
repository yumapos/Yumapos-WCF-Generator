using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WCFGenerator.SerializeGeneration.Models
{
    public class SourceElements
    {
        public ClassDeclarationSyntax CurrentClass { get; set; }
        public List<PropertyDeclarationSyntax> Properties { get; set; }
        public List<PropertyDeclarationSyntax> AllPublicProperties { get; set; }
        public List<FieldDeclarationSyntax> Fields { get; set; }
        public ClassDeclarationSyntax MappingClass { get; set; }
        public List<PropertyDeclarationSyntax> MappingProperties { get; set; }
    }
}
