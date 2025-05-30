using WCFGenerator.Common;

namespace WCFGenerator.SerializeGeneration.Models
{
    public class MigrationElements
    {
        public CollectionComparisonResult<GenerationProperty> ChangeWithPreviousProperties { get; set; }
        public string ClassNameWithNameSpace { get; set; }
    }
}
