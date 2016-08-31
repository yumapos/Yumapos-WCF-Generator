using System.Collections.Generic;
using VersionedRepositoryGeneration.Generator.Infrastructure;

namespace WCFGenerator.RepositoriesGeneration.Infrastructure
{
    internal class Many2ManyInfo
    {
        public string TableName { get; set; }
        public string EntityType { get; set; }
        public string FullEntityType { get; set; }
        public List<string> RepositoryNamespaces { get; set; }

        public Many2ManyInfo(string tableName, string entityType)
        {
            TableName = tableName;
            EntityType = entityType;
        }
    }
}
