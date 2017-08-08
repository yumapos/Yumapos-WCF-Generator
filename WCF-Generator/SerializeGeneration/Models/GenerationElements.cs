using System.Collections.Generic;
using WCFGenerator.Common;

namespace WCFGenerator.SerializeGeneration.Models
{
    public class GenerationElements
    {
        public string ClassName { get; set; }

        public string GeneratedClassName { get; set; }

        public List<GenerationProperty> Properties { get; set; }

        public string MapClassName { get; set; }

        public List<GenerationProperty> MapProperties { get; set; }

        public string Namespace { get; set; }

        public IEnumerable<string> UsingNamespaces { get; set; }

        public string ClassAccessModificator { get; set; }

        public bool IsPropertyEquals { get; set; }

        public string SerializableBaseClassName { get; set; }

        public CollectionComparisonResult<GenerationProperty> ChangeWithPreviousProperties { get; set; }

    }
}
