using System.Collections.Generic;

namespace WCFGenerator.RepositoriesGeneration.Infrastructure
{
    internal class Many2ManyInfo
    {
        public string PropertyName { get; set; }
        public string ManyToManyEntytyType { get; set; }
        public RepositoryInfo ManyToManyRepositoryInfo { get; set; }
        public string EntityType { get; set; }
        public RepositoryInfo EntityRepositoryInfo { get; set; }
        public List<string> RepositoryNamespaces { get; set; }

        public Many2ManyInfo(string propertyName, string manyToManyEntytyType, string entityType)
        {
            PropertyName = propertyName;
            ManyToManyEntytyType = manyToManyEntytyType;
            EntityType = entityType;
        }
    }
}
