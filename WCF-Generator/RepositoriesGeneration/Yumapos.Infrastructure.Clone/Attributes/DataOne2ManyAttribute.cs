using System;

namespace WCFGenerator.RepositoriesGeneration.Yumapos.Infrastructure.Clone.Attributes
{
    internal class DataOne2ManyAttribute : Attribute
    {
        public string OneToManyEntityType { get; set; }
        public string EntityKey { get; set; }
    }
}   