using System;

namespace TestRepositoryGeneration.Infrastructure
{
    internal class DataMany2ManyAttribute : Attribute
    {
        public string EntityType { get; set; }
        public string ManyToManyEntytyType { get; set; }
    }
}