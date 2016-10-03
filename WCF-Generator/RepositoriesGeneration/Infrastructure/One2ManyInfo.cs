using System.Collections.Generic;

namespace WCFGenerator.RepositoriesGeneration.Infrastructure
{
    internal class One2ManyInfo
    {
        public string PropertyName { get; set; }
        public string OneToManyEntytyType { get; set; }
        public RepositoryInfo OneToManyRepositoryInfo { get; set; }
        public string EntityKey { get; set; }

        public One2ManyInfo(string propertyName, string oneToManyEntytyType, string entityKey)
        {
            PropertyName = propertyName;
            OneToManyEntytyType = oneToManyEntytyType;
            EntityKey = entityKey;
        }
    }
}