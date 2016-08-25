using System;

namespace VersionedRepositoryGeneration.Models.Attributes
{
    internal class DataMany2ManyAttribute : Attribute
    {
        public object Type { get; set; }
        public string Name { get; set; }
    }
}