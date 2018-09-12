using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFGenerator.Common;
using WCFGenerator.MappingsGeneration.Configuration;
using WCFGenerator.MappingsGeneration.Infrastructure;
using WCFGenerator.MappingsGenerator.Analysis;

namespace WCFGenerator.MappingsGeneration
{
    public class MappingGenerator
    {
        private readonly MappingConfiguration _configuration;
        private readonly GeneratorWorkspace _generatorWorkspace;

        public MappingGenerator(MappingConfiguration configuration, GeneratorWorkspace generatorWorkspace)
        {
            _configuration = configuration;
            _generatorWorkspace = generatorWorkspace;
        }

        public async Task Generate()
        {
            var analyser = new MappingAnalyser(_configuration, _generatorWorkspace);
            await analyser.Run();
            var code = GetFullCode(analyser.ListOfSimilarClasses.ToArray(), analyser.ClassesWithoutPair.ToArray());
            _generatorWorkspace.SetTargetProject(_configuration.ProjectForGeneratedCode);
            _generatorWorkspace.UpdateFileInTargetProject("MappingExtension.g.cs", "Generation", code);
            await _generatorWorkspace.ApplyTargetProjectChanges(true);
        }

        private string GetFullCode(MapDtoAndDo[] similarClasses, ClassCompilerInfo[] classesWithoutPair)
        {
            var sb = new StringBuilder();
            foreach (PrefixString prefixString in _configuration.PrefixStrings)
            {
                sb.AppendLine(prefixString.Text);
            }
            sb.AppendLine("");
            sb.AppendLine("namespace " + _configuration.MapExtensionNameSpace);
            sb.AppendLine("{");
            sb.AppendLine("public static class " + _configuration.MapExtensionClassName);
            sb.AppendLine("{");
            foreach (var classWithoutPair in classesWithoutPair)
            {
                sb.AppendLine("//" + classWithoutPair.NamedTypeSymbol.GetFullName());
            }

            foreach (var similarClass in similarClasses)
            {
                sb.AppendLine("");
                var doClassName = similarClass.DtoClass != null ? similarClass.DOClass.NamedTypeSymbol.GetFullName() : similarClass.DOInterface.GetFullName();
                var dtoClassName = similarClass.DtoClass != null ? similarClass.DtoClass.NamedTypeSymbol.GetFullName() : similarClass.DtoInterface.GetFullName();
                sb.AppendLine("public static " + dtoClassName + " MapToDto (this " + doClassName + " item)");
                sb.AppendLine("{");
                foreach (var prop in similarClass.IsIgnoreDOProperties)
                {
                    sb.AppendLine("//itemDo." + prop.Name);
                }
                sb.AppendLine("}");
                sb.AppendLine("");
                sb.AppendLine("public static " + doClassName + " MapFromDto (this " + dtoClassName + " itemDto)");
                sb.AppendLine("{");
                foreach (var prop in similarClass.IsIgnoreDTOProperties)
                {
                    sb.AppendLine("//itemDto." + prop.Name);
                }
                sb.AppendLine("}");
            }

            sb.AppendLine("}");
            sb.AppendLine("}");

            return sb.ToString();
        }
        /*
         *         public static <#= similarClass.DtoClass != null ? similarClass.DtoClass.FullName : similarClass.DtoInterface.FullName #> MapToDto (this <#= similarClass.DtoClass != null ? similarClass.DOClass.FullName : similarClass.DOInterface.FullName #> item)
                {  
        <#+         foreach (var prop in isIgnoreDOProperties)
                    {
        #>            //itemDo.<#= prop.Name #>
        <#+			}
        #>
                    if (item == null) return null;

                    var itemDto = new <#= similarClass.DtoClass.FullName #> ();
        <#+         foreach(MapPropertiesDtoAndDo property in similarClass.MapPropertiesDtoAndDo)
                    { 
                        if(property.ToDtoFunction != "" ) 
                        {
        #>                <#= property.ToDtoFunction #>;
        <#+            }
                    } #>

                    return itemDto;
                }

                public static <#= similarClass.DOClass != null ? similarClass.DOClass.FullName : similarClass.DOInterface.FullName #> MapFromDto (this <#= similarClass.DtoClass != null ? similarClass.DtoClass.FullName : similarClass.DtoInterface.FullName #> itemDto)
                {  
        <#+         foreach (var prop in isIgnoreDTOProperties)
                    {
        #>            //itemDto.<#= prop.Name #>
        <#+			}
        #>
                    if (itemDto == null) return null;

                    var item = new <#= similarClass.DOClass.FullName #> ();
        <#+          foreach(MapPropertiesDtoAndDo property in similarClass.MapPropertiesDtoAndDo)
                    { 
                        if(property.FromDtoFunction != "" )
                        {
        #>                <#= property.FromDtoFunction #>;
        <#+             }
                    } #>

                    return item;
                }
            <#+  } #>
         */
    }
}
