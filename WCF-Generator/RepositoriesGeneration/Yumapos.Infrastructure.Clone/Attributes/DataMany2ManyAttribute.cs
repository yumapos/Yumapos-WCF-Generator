using System;

namespace WCFGenerator.RepositoriesGeneration.Yumapos.Infrastructure.Clone.Attributes
{
    internal class DataMany2ManyAttribute : Attribute
    {
        public string EntityType { get; set; }
        public string ManyToManyEntityType { get; set; }
    }
}