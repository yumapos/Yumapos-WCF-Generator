using System;

namespace VersionedRepositoryGeneration.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class DataFilterAttribute : Attribute
    {
        public object DefaultValue { get; set; }
    }
}