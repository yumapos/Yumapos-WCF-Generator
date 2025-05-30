using System;

namespace WCFGenerator.RepositoriesGeneration.Yumapos.Infrastructure.Clone.Attributes
{
    public class DataMany2ManyAttribute : Attribute
    {
        public string EntityType { get; set; }
        public string ManyToManyEntityType { get; set; }
    }
}