using System;

namespace WCFGenerator.RepositoriesGeneration.Yumapos.Infrastructure.Clone.Attributes
{
    internal class DataOne2ManyAttribute : Attribute
    {
        public string ManyToManyEntytyType { get; set; }
        public string EntityKey { get; set; }
    }
}   